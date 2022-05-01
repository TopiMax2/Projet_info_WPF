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

namespace Projet_info_WPF
{
    /// <summary>
    /// Logique d'interaction pour Input.xaml
    /// Nous permet de récupérer le nom de l'image a modifier
    /// </summary>
    public partial class Input : Window
    {
        public Input(string question, string txtanswer)
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = txtanswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }
    }
}
