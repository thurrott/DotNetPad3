using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DotNetPad
{

    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialog(string DocumentName)
        {
            InitializeComponent();

            ConfirmTextBlock.Text = "Do you want to save the changes to " +
                DocumentName + "?";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfirmationSaveButton.Focus();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }

        private void ConfirmationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button button = (Button)sender;
                string s = button.Name.ToString();
                App.Choice = s;
            }

            this.Close();
        }
    }
}
