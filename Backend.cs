using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace DotNetPad
{
    public partial class MainWindow : Window
    {
        //
        // Settings
        //
        private void LoadSettings()
        {
            // App theme
            string t = Settings.Default.MyTheme;
            Application.Current.ThemeMode = t switch
            {
                "System" => ThemeMode.System,
                "Light" => ThemeMode.Light,
                "Dark" => ThemeMode.Dark,
                _ => ThemeMode.System,
            };
            if (Application.Current.ThemeMode == ThemeMode.Light)
            {
                LightThemeRadio.IsChecked = true;
                DarkThemeRadio.IsChecked = false;
                SystemThemeRadio.IsChecked = false;
            }
            else if (Application.Current.ThemeMode == ThemeMode.Dark) 
            {
                LightThemeRadio.IsChecked = false;
                DarkThemeRadio.IsChecked = true;
                SystemThemeRadio.IsChecked = false;
            }
            else
            {
                LightThemeRadio.IsChecked = false;
                DarkThemeRadio.IsChecked = false;
                SystemThemeRadio.IsChecked = true;
            }

            // App window location and size
            Application.Current.MainWindow.Left = Settings.Default.MyLocationX;
            Application.Current.MainWindow.Top = Settings.Default.MyLocationY;

            if (Settings.Default.MyMaximized == true)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;

            Application.Current.MainWindow.Width = Settings.Default.MyWidth;
            Application.Current.MainWindow.Height = Settings.Default.MyHeight;

            // Font family
            string fontName = Settings.Default.MyFontFamily;
            TextBox1.FontFamily = new System.Windows.Media.FontFamily(fontName);

            // Font styles
            if (Settings.Default.MyFontBold)
                TextBox1.FontWeight = FontWeights.Bold;
            else
                TextBox1.FontWeight = FontWeights.Normal;
            if (Settings.Default.MyFontItalic)
                TextBox1.FontStyle = FontStyles.Italic;
            else
                TextBox1.FontStyle = FontStyles.Normal;

            // Font size
            TextBox1.FontSize = App.FontSizeInSettings = Settings.Default.MyFontSize;

            // Font example combo box in settings
            fontName = Settings.Default.MyFontFamily;
            FontExampleLabel.FontFamily = new System.Windows.Media.FontFamily(fontName);
            FontExampleLabel.FontWeight = Settings.Default.MyFontBold ? FontWeights.Bold : FontWeights.Normal;
            FontExampleLabel.FontStyle = Settings.Default.MyFontItalic ? FontStyles.Italic : FontStyles.Normal;
            FontExampleLabel.FontSize = Settings.Default.MyFontSize;

            for (int x = 0; x <= Fonts.SystemFontFamilies.Count - 1; x++)
            {
                if (FontFamilyComboBox.Items[x].ToString() == FontExampleLabel.FontFamily.ToString())
                {
                    FontFamilyComboBox.SelectedItem = FontFamilyComboBox.Items[x];
                    break;
                }
            }

            List<string> fontStyle =
            [
                "Normal", "Italic", "Bold", "Bold Italic"
            ];
            FontStyleComboBox.DataContext = fontStyle;

            if (FontExampleLabel.FontStyle == FontStyles.Normal)
                FontSizeComboBox.SelectedIndex = 0;
            if (FontExampleLabel.FontStyle == FontStyles.Italic)
                FontSizeComboBox.SelectedIndex = 1;
            if (FontExampleLabel.FontWeight == FontWeights.Bold)
                FontSizeComboBox.SelectedIndex = 2;
            if (FontExampleLabel.FontWeight == FontWeights.Bold && FontExampleLabel.FontStyle == FontStyles.Italic)
                FontSizeComboBox.SelectedIndex = 3;

            List<double> fontSize =
            [
                8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72
            ];
            FontSizeComboBox.DataContext = fontSize;

            for (int x = 0; x < fontSize.Count; x++)
                if (fontSize[x].ToString() == FontExampleLabel.FontSize.ToString())
                {
                    FontSizeComboBox.SelectedIndex = x;
                    break;
                }

            // Spell check
            SpellCheck = Settings.Default.MySpellCheck;
            TextBox1.SpellCheck.IsEnabled = SpellCheck;
            SpellCheckToggleButton.IsChecked = SpellCheck;

            if (SpellCheck)
            {
                SpellCheckToggleButton.Content = "On";
            }
            else
            {
                SpellCheckToggleButton.Content = "Off";
            }

            // Word wrap
            if (Settings.Default.MyWordWrap == true)
            {
                WordWrapMenu.IsChecked = true;
                WordWrapToggleButton.IsChecked = true;
                WordWrapToggleButton.Content = "On";
                TextBox1.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                WordWrapMenu.IsChecked = false;
                WordWrapToggleButton.IsChecked = false;
                WordWrapToggleButton.Content = "Off";
                TextBox1.TextWrapping = TextWrapping.NoWrap;
            }

            // Status bar
            if (Settings.Default.MyStatusBar == true)
            {
                StatusBar1.Visibility = Visibility.Visible;
                StatusBarMenu.IsChecked = true;
            }
            else
            {
                StatusBar1.Visibility = Visibility.Collapsed;
                StatusBarMenu.IsChecked = false;
            }

            // Auto save with timer
            App.AutoSave = Settings.Default.MyAutoSave;
            MyTimer = new System.Timers.Timer(30000)   // 30 seconds
            {
                AutoReset = true,
                Enabled = true,
            };
            MyTimer.Elapsed += OnTimerElapsed;

            AutoSaveMenu.IsChecked = App.AutoSave;
            AutoSaveToggleButton.IsChecked = App.AutoSave;

            if (App.AutoSave)
            {
                AutoSaveText.Text = "Auto save: On";
                MyTimer.Start();
                AutoSaveToggleButton.Content = "On";
            }
            else
            {
                AutoSaveText.Text = "Auto save: Off";
                MyTimer.Stop();
                AutoSaveToggleButton.Content = "Off";
            }

            // Zoom
            ZoomValue = Settings.Default.MyZoom;
            ZoomText.Text = ZoomValue.ToString() + "%";
            if (ZoomValue != 100)
                TextBox1.FontSize = (App.FontSizeInSettings * ZoomValue) / 100;
        }

        private void SaveSettings()
        {
            // App theme
            Settings.Default.MyTheme = Application.Current.ThemeMode.ToString();
            
            // Window location and size
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Settings.Default.MyMaximized = true;
            else
                Settings.Default.MyMaximized = false;

            Settings.Default.MyLocationX = Application.Current.MainWindow.Left;
            Settings.Default.MyLocationY = Application.Current.MainWindow.Top;

            Settings.Default.MyWidth = Application.Current.MainWindow.Width;
            Settings.Default.MyHeight = Application.Current.MainWindow.Height;

            // Save font
            Settings.Default.MyFontFamily = TextBox1.FontFamily.ToString();

            if (TextBox1.FontWeight == FontWeights.Bold)
                Settings.Default.MyFontBold = true;
            else
                Settings.Default.MyFontBold = false;
            if (TextBox1.FontStyle == FontStyles.Italic)
                Settings.Default.MyFontItalic = true;
            else
                Settings.Default.MyFontItalic = false;

            Settings.Default.MyFontSize = App.FontSizeInSettings;

            // Spell check
            Settings.Default.MySpellCheck = SpellCheck;

            // Word wrap
            Settings.Default.MyWordWrap = WordWrapMenu.IsChecked;

            // Word wrap
            Settings.Default.MyWordWrap = WordWrapMenu.IsChecked;

            // Auto save
            Settings.Default.MyAutoSave = App.AutoSave;

            // Zoom
            Settings.Default.MyZoom = ZoomValue;

            // Save changes to app settings
            Settings.Default.Save();
        }

        //
        // Save
        //
        private void Save()
        {
            if (dts[0].DocumentName != "Untitled")
            {
                try
                {
                    File.WriteAllText(dts[0].DocumentName, TextBox1.Text);
                    dts[0].TextHasChanged = false;
                    TitleText.Text = Path.GetFileNameWithoutExtension(dts[0].DocumentName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                SaveAs();
            }
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    dts[0].DocumentName = saveFileDialog.FileName;
                    TitleText.Text = Path.GetFileNameWithoutExtension(dts[0].DocumentName);
                    dts[0].TextHasChanged = false;
                    File.WriteAllText(saveFileDialog.FileName, TextBox1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private bool SaveOrSaveAs()
        {


            return true;
        }

        //
        // Auto save
        //
        private void AutoSaveEnable()
        {
            // Enable Auto save
            App.AutoSave = true;

            // Then, update the UI
            AutoSaveToggleButton.IsChecked = true;
            AutoSaveToggleButton.Content = "On";
            AutoSaveText.Text = "Auto save: On";
            AutoSaveMenu.IsChecked = true;

            // Enable timer, save the settings
            MyTimer.Enabled = true;
            SaveSettings();
        }

        private void AutoSaveDisable()
        {
            // Disable Auto save
            App.AutoSave = false;

            // Then, update the UI
            AutoSaveToggleButton.IsChecked = false;
            AutoSaveToggleButton.Content = "Off";
            AutoSaveText.Text = "Auto save: Off";
            AutoSaveMenu.IsChecked = false;

            // Disable timer, save the settings
            MyTimer.Enabled = false;
            SaveSettings();
        }

        //
        // Text functions
        //
        private void ChangePositionText()
        {
            int Line = TextBox1.GetLineIndexFromCharacterIndex(TextBox1.CaretIndex);
            int Column = TextBox1.CaretIndex - TextBox1.GetCharacterIndexFromLineIndex(Line);
            PositionText.Text = "Ln " + (Line + 1).ToString() + ", Col "
                + (Column + 1).ToString();
        }

        //
        // File operations
        //

        private void NewDocument()
        {
            TextBox1.Text = "";
            dts[0].TextHasChanged = false;
            dts[0].DocumentIsSaved = false;
            dts[0].DocumentName = "Untitled";
            TitleText.Text = dts[0].DocumentName;
            
            dts[0].FindTextString = "";
            dts[0].FindLastIndexFound = 0;
        }

        private void OpenDocument()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    TextBox1.Text = File.ReadAllText(openFileDialog.FileName);
                    TitleText.Text = Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName);
                    dts[0].TextHasChanged = false;
                    dts[0].DocumentIsSaved = true;
                    dts[0].DocumentName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            dts[0].FindTextString = "";
            dts[0].FindLastIndexFound = 0;
        }

        //
        // Confirmation dialog
        //

        private bool DisplayConfirmationDialog(string Operation)
        {
            bool Continue = true;
            
            ConfirmationDialog cd = new(dts[0].DocumentName)
            {
                Owner = this
            };
            
            cd.ShowDialog();

            switch (App.Choice)
            {
                case "ConfirmationSaveButton":
                    if (!SaveOrSaveAs())
                    {
                        Continue = false;
                        break;
                    };
                    break;
                case "ConfirmationDontSaveButton":
                    break;
                case "ConfirmationCancelButton":
                    Continue = false;
                    break;
                default: break;
            }

            if (Continue)
            {
                switch (Operation)
                {
                    case "New":
                        NewDocument();
                        break;
                    case "Open":
                        // OpenDocument();
                        break;
                    case "Close":
                        dts[0].TextHasChanged = false;
                        try
                        {
                            Application.Current.MainWindow.Close();
                        }
                        catch (Exception)
                        {
                            break;
                        }
                        break;
                    default: break;
                }
            }
            TextBox1.Focus();

            return Continue;
        }

        //
        // Find and replace
        //

        private void FindTheText()
        {
            if (dts[0].FindLastIndexFound > -1)
            {
                // Select the text
                TextBox1.Select(dts[0].FindLastIndexFound, dts[0].FindTextString.Length);
                // Navigate to that place in the text if needed
                TextBox1.ScrollToLine(TextBox1.GetLineIndexFromCharacterIndex((dts[0].FindLastIndexFound)));
                // Then, give the focus to the text box so you can see the selection
                TextBox1.Focus();
            }
            else
            {
                MessageBox.Show(this, "Cannot find " + (char)34 + dts[0].FindTextString + (char)34, Application.Current.MainWindow.GetType().Assembly.GetName().Name, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void FindTextIndex(int FindFromIndex, bool FindPreviousIndex)
        {
            if (FindPreviousIndex == false)
            {
                dts[0].FindLastIndexFound = TextBox1.Text.IndexOf(dts[0].FindTextString, FindFromIndex);
                if (dts[0].FindLastIndexFound <= 0)
                {
                    // If text is not found, try searching from the beginning
                    dts[0].FindLastIndexFound = TextBox1.Text.IndexOf(dts[0].FindTextString, 0);
                }
            }
            else
            {
                dts[0].FindLastIndexFound = TextBox1.Text.LastIndexOf(dts[0].FindTextString, FindFromIndex);
                if (dts[0].FindLastIndexFound <= -1)
                {
                    //  If text is not found, try searching from the end
                    dts[0].FindLastIndexFound = TextBox1.Text.LastIndexOf(dts[0].FindTextString, TextBox1.Text.Length - 1);
                }
            }
        }

        private void DisplayReplaceControls()
        {
            // First, call Find
            FindCommand_Executed(this, null);

            // Unhide the Replace controls
            ReplaceTextBox.Visibility = Visibility.Visible;
            ReplaceButton.Visibility = Visibility.Visible;
            ReplaceAllButton.Visibility = Visibility.Visible;

            // Configure default event handler
            FindNextButton.IsDefault = false;
            ReplaceButton.IsDefault = true;

            // Finally, select and focus the appropriate text box
            if (FindTextBox.Text != "")
            {
                ReplaceTextBox.SelectAll();
                ReplaceTextBox.Focus();
            }
        }

        //
        // Fix menu alignment on touch-screen PCs
        // Per https://www.answeroverflow.com/m/1134790256609206333
        //
        private static void SetDropDownMenuToBeRightAligned()
        {
            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Action setAlignmentValue = () =>
            {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
            };

            setAlignmentValue();

            SystemParameters.StaticPropertyChanged += (sender, e) =>
            {
                setAlignmentValue();
            };
        }
    }
}
