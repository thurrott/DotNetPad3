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
    public partial class GoToLineDialog : Window
    {
        public int LineNumber;

        public GoToLineDialog(int ln)
        {
            InitializeComponent();
            LineNumber = ln;
            GoToTextBox.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GoToTextBox.Text = LineNumber.ToString();
            GoToTextBox.Focus();
            GoToTextBox.SelectAll();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }
        private void GoToLineButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
