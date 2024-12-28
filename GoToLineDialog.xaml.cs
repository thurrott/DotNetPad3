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
        }
        private void GoToLineButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
