using System.Windows;
using System.Windows.Controls;

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
        }

        private void MessageBoxButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
