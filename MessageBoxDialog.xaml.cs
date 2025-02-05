using System.Windows;

namespace DotNetPad
{
    public partial class MessageBoxDialog : Window
    {
        public MessageBoxDialog(string title, string message)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBoxOKButton.Focus();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }

        private void MessageBoxButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}