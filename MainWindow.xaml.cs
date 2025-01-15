using DotNetPad;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Windows.ApplicationModel.VoiceCommands;

namespace DotNetPad
{
    public partial class MainWindow : Window
    {
        //
        // In-app user settings
        //
        int ZoomValue;
        bool SpellCheck = false;
        bool WordCount = true;
        private static System.Timers.Timer MyTimer = new();
        public string AppName = ".NETpad for Windows 11";

        //
        // New variables for tabs
        //
        public TabItem CurrentTab = new();
        public List<DocumentTab> dts = [];

        public MainWindow()
        {
            // Fix menus
            SetDropDownMenuToBeRightAligned();

            // Create a new DocumentTab object first
            DocumentTab dt = new();

            InitializeComponent();

            // Add the new DocumentTab object to the DocumentTabs list
            dts.Add(dt);

            // The following document state is available
            // dts[0].TextHasChanged = false
            // dts[0].DocumentIsSaved = false
            // dts[0].DocumentName = "Untitled.txt"
            // dts[0].LineNumber = 0;
            // dts[0].FindTextString = "";
            // dts[0].FindLastIndexFound = 0;

            // Set the first (and only) tab in the app as the CurrentTab
            // CurrentTab = (TabItem)MyTabControl.Items[MyTabControl.SelectedIndex + 1];
            // CurrentTab.Header = dts[0].DocumentName;
            TitleText.Text = dts[0].DocumentName;

            // Get user settings
            LoadSettings();

            // Settings pane labels

            AppNameLabel.Content = AppName;
            // AppNameLabel.Content = Assembly.GetExecutingAssembly().GetName().Name;
            
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            VersionLabel.Content = "Version " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()
                                   + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            CopyrightLabel.Content = "Copyright © 2025 Paul Thurrott";

            // Select the text box so the user can start typing
            TextBox1.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Don't let the app window just close
            // First save settings, then see if the document needs to be saved
            // Will need to be updated to support multiple documents in next version
            e.Cancel = true;

            SaveSettings();

            if (dts[0].TextHasChanged)
            {
                DisplayConfirmationDialog("ConfirmationSaveButton");
                switch (App.Choice)
                {
                    case "ConfirmationSaveButton":
                        Save();
                        e.Cancel = false;
                        break;
                    case "ConfirmationDontSaveButton":
                        e.Cancel = false;
                        break;
                    default: break;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If viewing settings, auto flow the layout as needed
            if (SettingsScrollViewer.Visibility == Visibility.Visible)
            {
                if (AppWindow.Width > 950)
                {
                    AboutGrid.SetValue(Grid.RowProperty, 1);
                    AboutGrid.SetValue(Grid.ColumnProperty, 1);
                }
                else
                {
                    AboutGrid.SetValue(Grid.RowProperty, 2);
                    AboutGrid.SetValue(Grid.ColumnProperty, 0);
                }
            }
        }

        //
        // Timer
        //
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Required for saving an existing document (not Save as)
            // Otherwise, Save condition triggers a crash
            this.Dispatcher.Invoke(() =>
            {
                if (dts[0].TextHasChanged)
                {
                    // Stop the timer, otherwise this will keep firing and could keep display more SaveAs dialogs
                    MyTimer.Stop();

                    Save();

                    // Restart the timer when we're done
                    MyTimer.Start();
                }
            });
        }

        //
        // Settings
        //
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable timer so a Save as window won't appear
            MyTimer.Enabled = false;

            // Clear the Outer grid column definitions
            OuterAppGrid.ColumnDefinitions.Clear();

            // Select the correct item in the Font style combo box
            ////// Not clear why this is necessary
            if (TextBox1.FontStyle == FontStyles.Normal)
            {
                FontStyleComboBox.SelectedIndex = 0;
                FontExampleLabel.FontStyle = FontStyles.Normal;
                FontExampleLabel.FontWeight = FontWeights.Normal;
            }
            if (TextBox1.FontStyle == FontStyles.Italic)
            {
                FontStyleComboBox.SelectedIndex = 1;
                FontExampleLabel.FontStyle = FontStyles.Italic;
                FontExampleLabel.FontWeight = FontWeights.Normal;
            }
            if (TextBox1.FontWeight == FontWeights.Bold)
            {
                FontStyleComboBox.SelectedIndex = 2;
                FontExampleLabel.FontStyle = FontStyles.Normal;
                FontExampleLabel.FontWeight = FontWeights.Bold;
            }
            if (TextBox1.FontWeight == FontWeights.Bold && TextBox1.FontStyle == FontStyles.Italic)
            {
                FontStyleComboBox.SelectedIndex = 3;
                FontExampleLabel.FontStyle = FontStyles.Italic;
                FontExampleLabel.FontWeight = FontWeights.Bold;
            }

            // Hide the Main grid, and display the Back button and the Settings grid
            BackButton.Visibility = Visibility.Visible;
            SettingsScrollViewer.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Hidden;
            SettingsGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            SettingsGrid.Visibility = Visibility.Visible;
            // TitleText.Text = App.Current.MainWindow.GetType().Assembly.GetName().Name;
            TitleText.Text = AppName;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the Outer grid column definitions on exit
            OuterAppGrid.ColumnDefinitions.Clear();

            SaveSettings();

            // Unexpand all the Expander controls on exit
            AppThemeExpander.IsExpanded = false;
            FontExpander.IsExpanded = false;

            // Re-enable the timer on exit - that way, a Save as dialog will appear again if needed
            MyTimer.Enabled = true;

            // Hide the Back button and the Settings grid on exit and display the Main grid
            BackButton.Visibility = Visibility.Collapsed;
            SettingsScrollViewer.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Hidden;
            MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            MainGrid.Visibility = Visibility.Visible;
            TitleText.Text = Path.GetFileNameWithoutExtension(dts[0].DocumentName);

            TextBox1.Focus();
        }

        private void LightThemeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Light;
        }

        private void DarkThemeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Dark;
        }

        private void SystemThemeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.System;
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                string? fontName = FontFamilyComboBox.SelectedItem.ToString();

                FontExampleLabel.FontFamily = new FontFamily(fontName);
                TextBox1.FontFamily = FontExampleLabel.FontFamily;
                SaveSettings();
            }
        }

        private void FontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                switch (FontStyleComboBox.SelectedItem.ToString())
                {
                    case "Normal":
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Normal;
                        SaveSettings();
                        break;
                    case "Italic":
                        FontExampleLabel.FontStyle = FontStyles.Italic;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Italic;
                        TextBox1.FontWeight = FontWeights.Normal;
                        SaveSettings();
                        break;
                    case "Bold":
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Bold;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Bold;
                        SaveSettings();
                        break;
                    case "Bold Italic":
                        FontExampleLabel.FontStyle = FontStyles.Italic;
                        FontExampleLabel.FontWeight = FontWeights.Bold;
                        TextBox1.FontStyle = FontStyles.Italic;
                        TextBox1.FontWeight = FontWeights.Bold;
                        SaveSettings();
                        break;
                    default:
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Normal;
                        SaveSettings();
                        break;
                }
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                string? size = FontSizeComboBox.SelectedItem.ToString();
                FontExampleLabel.FontSize = Convert.ToDouble(size);
                TextBox1.FontSize = Convert.ToDouble(size);
                App.FontSizeInSettings = Convert.ToDouble(size);
                SaveSettings();
            }
        }
        private void SpellCheckToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpellCheck)
            {
                // Disable Spell check
                SpellCheckToggleButton.IsChecked = false;
                SpellCheckToggleButton.Content = "Off";
            }
            else
            {
                // Enable Spell check
                SpellCheckToggleButton.IsChecked = true;
                SpellCheckToggleButton.Content = "On";
            }
            SpellCheck = !SpellCheck;
            TextBox1.SpellCheck.IsEnabled = SpellCheck;

            SaveSettings();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Font family
            string fontName = "Consolas";
            TextBox1.FontFamily = new System.Windows.Media.FontFamily(fontName);
            Settings.Default.MyFontFamily = fontName;

            // Font size next
            TextBox1.FontSize = App.FontSizeInSettings = 16;
            Settings.Default.MyFontSize = TextBox1.FontSize;

            // Font weight and style
            TextBox1.FontWeight = FontWeights.Normal;
            Settings.Default.MyFontBold = false;
            TextBox1.FontStyle = FontStyles.Normal;
            Settings.Default.MyFontItalic = false;

            // Auto save
            AutoSaveDisable();
            Settings.Default.MyAutoSave = false;
            MyTimer.Enabled = false;

            // Spell check
            Settings.Default.MySpellCheck = false;

            // Zoom
            ZoomValue = 100;
            ZoomText.Text = "100%";
            Settings.Default.MyZoom = ZoomValue;

            SaveSettings();
            BackButton_Click(sender, e);
        }

        //
        // Text box
        //
        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dts[0].TextHasChanged == false)
            {
                TitleText.Text = "▪️ " + TitleText.Text;
                dts[0].TextHasChanged = true;
            }
            DisplayCount();

            ChangePositionText();
        }

        //
        // Menu commands
        //

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dts[0].TextHasChanged)
            {
                if (DisplayConfirmationDialog("New") == false)
                    return;
                else
                    NewDocument();
            }
            else
            {
                NewDocument();
            }
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dts[0].TextHasChanged)
            {
                if (DisplayConfirmationDialog("Open") == false)
                    return;
                else
                    OpenDocument();
            }
            else
            {
                OpenDocument();
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dts[0].TextHasChanged = true;
            Save();
            // SaveOrSaveAs();
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dts[0].TextHasChanged = true;
            dts[0].DocumentIsSaved = false;
            SaveAs();
            // SaveOrSaveAs();
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = true
            };

            bool? print = printDialog.ShowDialog();
            if (print == true)
            {
                FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(TextBox1.Text)))
                {
                    ColumnWidth = printDialog.PrintableAreaWidth
                };
                IDocumentPaginatorSource iDocumentPaginatorSource = flowDocument;
                printDialog.PrintDocument(iDocumentPaginatorSource.DocumentPaginator, dts[0].DocumentName);
            }
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Handle this in Window_Closing()
            this.Close();
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    KeyEventArgs keyEventArgs = new(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Delete)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    };
                    InputManager.Current.ProcessInput(keyEventArgs);
                }
            }
        }

        private void FindCommand_Executed(object sender, ExecutedRoutedEventArgs? e)
        {
            // Set FindTextString to the text being searched for
            dts[0].FindTextString = TextBox1.SelectedText;
            // If there is selected text, put it in the Find textbox
            if (FindTextBox.Text == "")
                FindTextBox.Text = dts[0].FindTextString;

            // Hide the Replace controls
            ReplaceTextBox.Visibility = Visibility.Collapsed;
            ReplaceButton.Visibility = Visibility.Collapsed;
            ReplaceAllButton.Visibility = Visibility.Collapsed;

            // Configure default event handler
            FindNextButton.IsDefault = true;
            ReplaceButton.IsDefault = false;

            // Display Find/Replace pane if necessary
            if (FindReplaceGrid.Visibility != Visibility.Visible)
            {
                FindReplaceGrid.Visibility = Visibility.Visible;
            }

            FindTextIndex(TextBox1.SelectionStart, false);
            FindTheText();

            // Finally, select and focus the Find text box
            FindTextBox.SelectAll();
            FindTextBox.Focus();
        }

        private void FindNextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindNextButton_Click(sender, e);
        }

        private void FindPreviousCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindPreviousButton_Click(sender, e);
        }

        private void ReplaceCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReplaceButton_Click(sender, e);
        }

        private void ReplaceAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReplaceAllButton_Click(sender, e);
        }

        private void TimeDateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Obtain the time/date, insert it, and move the selection point to the end
            System.DateTime now = System.DateTime.Now;
            TextBox1.SelectedText = now.ToShortTimeString() + " " + now.ToShortDateString();
            TextBox1.SelectionStart += (now.ToShortTimeString() + " " + now.ToShortDateString()).Length;
            TextBox1.SelectionLength = 0;
        }

        private void GoToCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Get the current line number in the text box
            dts[0].LineNumber = TextBox1.GetLineIndexFromCharacterIndex(TextBox1.CaretIndex) + 1;

            GoToLineDialog gt = new(dts[0].LineNumber)
            {
                Owner = this
            };

            if (gt.ShowDialog() == true)
            {
                try
                {
                    int LineNum = System.Convert.ToInt32(gt.GoToTextBox.Text);
                    if (LineNum <= TextBox1.LineCount)
                    {
                        TextBox1.SelectionStart = TextBox1.GetCharacterIndexFromLineIndex(LineNum - 1);
                        TextBox1.SelectionLength = 1;
                        TextBox1.ScrollToLine(LineNum - 1);
                    }
                    else
                    {
                        MessageBox.Show("The line number is beyond the total number of lines", "Go to line");
                        GoToCommand_Executed(this, e);
                    }
                }
                catch (System.Exception)
                {
                    GoToCommand_Executed(this, e);
                }
            }
        }

        private void ZoomCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case "In":
                    if (ZoomValue <= 500)
                    {
                        ZoomValue += 10;
                        TextBox1.FontSize = (App.FontSizeInSettings * ZoomValue) / 100;
                        ZoomText.Text = ZoomValue.ToString() + "%";
                        SaveSettings();
                    }
                    break;
                case "Out":
                    if (ZoomValue > 10)
                    {
                        ZoomValue -= 10;
                        TextBox1.FontSize = (App.FontSizeInSettings * ZoomValue) / 100;
                        ZoomText.Text = ZoomValue.ToString() + "%";
                        SaveSettings();
                    }
                    break;
                case "Restore":
                    TextBox1.FontSize = App.FontSizeInSettings;
                    ZoomText.Text = "100%";
                    ZoomValue = 100;
                    SaveSettings();
                    break;
            }
        }


        //
        // Menu items clicks
        //
        private void NewWindowMenu_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.GetFileName(System.Environment.ProcessPath)!);
        }

        private void AutoSaveMenu_Click(object sender, RoutedEventArgs e)
        {
            AutoSaveDialog asd = new()
            {
                Owner = this
            };

            if (App.AutoSave == true)
            {
                // You are disabling Auto save
                bool? result = asd.ShowDialog();
                switch (result)
                {
                    case true:
                        AutoSaveDisable();
                        AutoSaveMenu.IsChecked = false;
                        break;
                    case false:
                        AutoSaveEnable();
                        AutoSaveMenu.IsEnabled = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // You are enabling Auto save
                bool? result = asd.ShowDialog();
                switch (result)
                {
                    case true:
                        AutoSaveEnable();
                        AutoSaveMenu.IsEnabled = true;
                        break;
                    case false:
                        AutoSaveDisable();
                        AutoSaveMenu.IsChecked = false;
                        break;
                    default:
                        break;
                }
            }
            SaveSettings();
        }

        private void SelectAllMenu_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.SelectAll();
        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            dts[0].FindTextString = FindTextBox.Text;
            dts[0].FindLastIndexFound = TextBox1.SelectionStart;

            if (dts[0].FindTextString != "")
            {
                FindTextIndex(dts[0].FindLastIndexFound + TextBox1.SelectionLength, false);
                FindTheText();
            }
        }

        private void FindPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            dts[0].FindTextString = FindTextBox.Text;
            dts[0].FindLastIndexFound = TextBox1.SelectionStart;

            if (FindReplaceGrid.Visibility == Visibility.Visible)
            {
                dts[0].FindTextString = FindTextBox.Text;
            }

            if (dts[0].FindTextString != "")
            {
                FindTextIndex(dts[0].FindLastIndexFound, true);
                FindTheText();
            }
        }

        private void CloseFindReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the Replace controls
            ReplaceTextBox.Visibility = Visibility.Collapsed;
            ReplaceButton.Visibility = Visibility.Collapsed;
            ReplaceAllButton.Visibility = Visibility.Collapsed;

            FindReplaceGrid.Visibility = Visibility.Collapsed;
            TextBox1.Focus();
        }

        private void FindReplaceGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set FindTextString to the text being searched for
            dts[0].FindTextString = TextBox1.SelectedText;
            // If there is selected text, put it in the Find textbox
            if (FindTextBox.Text == "")
                FindTextBox.Text = dts[0].FindTextString;
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // If a Replace control isn't visible, display Find/Replace with Replace controls
            if (ReplaceTextBox.Visibility == Visibility.Collapsed)
            {
                DisplayReplaceControls();
            }
            else
            {
                dts[0].FindTextString = FindTextBox.Text;
                dts[0].FindLastIndexFound = TextBox1.SelectionStart;

                // Find text from current cursor position
                FindTextIndex(TextBox1.SelectionStart, false);

                if (dts[0].FindTextString.Length != 0 && ReplaceTextBox.Text.Length != 0)
                {
                    if (dts[0].FindLastIndexFound > -1)
                    {
                        TextBox1.Text = TextBox1.Text.Substring(0, dts[0].FindLastIndexFound) + ReplaceTextBox.Text
                            + TextBox1.Text.Substring(dts[0].FindLastIndexFound + dts[0].FindTextString.Length);
                        TextBox1.Focus();
                        TextBox1.SelectionStart = dts[0].FindLastIndexFound;
                    }
                    else
                        MessageBox.Show(this, "Cannot find " + (char)34 + dts[0].FindTextString +
                            (char)34, System.Windows.Application.Current.MainWindow.GetType().Assembly.GetName().Name,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            // If a Replace control isn't visible, display Find/Replace with Replace controls
            if (ReplaceTextBox.Visibility == Visibility.Collapsed)
            {
                DisplayReplaceControls();
            }
            else
            {
                dts[0].FindTextString = FindTextBox.Text;
                dts[0].FindLastIndexFound = TextBox1.SelectionStart;

                // Find text from current cursor position
                FindTextIndex(TextBox1.SelectionStart, false);

                if (FindTextBox.Text.Length != 0 && ReplaceTextBox.Text.Length != 0)
                {
                    FindTextIndex(0, false);

                    if (dts[0].FindLastIndexFound > -1)
                    {
                        string? NewText = Microsoft.VisualBasic.Strings.Replace(TextBox1.Text, dts[0].FindTextString,
                            ReplaceTextBox.Text, 1);
                        TextBox1.Text = NewText;
                    }
                    else
                        MessageBox.Show(this, "Cannot find " + (char)34 + dts[0].FindTextString + (char)34,
                            System.Windows.Application.Current.MainWindow.GetType().Assembly.GetName().Name,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void FontMenu_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            FontExpander.IsExpanded = true;
            SettingsButton_Click(FontExpander, e);
        }

        private void StatusBarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (StatusBar1.Visibility == Visibility.Collapsed)
            {
                StatusBar1.Visibility = Visibility.Visible;
                StatusBarMenu.IsChecked = true;
            }
            else
            {
                StatusBar1.Visibility = Visibility.Collapsed;
                StatusBarMenu.IsChecked = false;
            }
            SaveSettings();
        }

        private void WordWrapMenu_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox1.TextWrapping == TextWrapping.NoWrap)
            {
                TextBox1.TextWrapping = TextWrapping.Wrap;
                WordWrapMenu.IsChecked = true;
                WordWrapToggleButton.IsChecked = true;
                WordWrapToggleButton.Content = "On";
            }
            else
            {
                TextBox1.TextWrapping = TextWrapping.NoWrap;
                WordWrapMenu.IsChecked = false;
                WordWrapToggleButton.IsChecked = false;
                WordWrapToggleButton.Content = "Off";
            }
            SaveSettings();
        }

        private void Count_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WordCount = !WordCount;
            DisplayCount();
        }

        public void DisplayCount()
        {
            if (WordCount)
            {
                int Count = System.Text.RegularExpressions.Regex.Matches(TextBox1.Text, @"[\S]+").Count;
                WordCountText.Text = Count.ToString() + " word";
                if (Count != 1)
                    WordCountText.Text += "s";
            }
            else
            {
                int Count = System.Text.RegularExpressions.Regex.Matches(TextBox1.Text, @".").Count;
                WordCountText.Text = Count.ToString() + " character";
                if (Count != 1)
                    WordCountText.Text += "s";
            }
        }

        private void SystemThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.System;
            LightThemeRadio.IsChecked = false;
            DarkThemeRadio.IsChecked = true;
            SystemThemeRadio.IsChecked = false;
            SaveSettings();
        }

        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Light; 
            LightThemeRadio.IsChecked = true;
            DarkThemeRadio.IsChecked = false;
            SystemThemeRadio.IsChecked = false;
            SaveSettings();
        }

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Dark;
            LightThemeRadio.IsChecked = true;
            DarkThemeRadio.IsChecked = false;
            SystemThemeRadio.IsChecked = false;
            SaveSettings();
        }
    }
}