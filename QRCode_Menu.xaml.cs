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
    /// Logique d'interaction pour QRCode_Menu.xaml
    /// </summary>
    public partial class QRCode_Menu : Window
    {
        public QRCode_Menu()
        {
            InitializeComponent();
        }

        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            string text = Text.Text;
            bool valid_sentence = true;
            if(text.Length > 47)
            {
                MessageBox.Show("Erreur, message trop long ! (maximum de 47 charactères)", "Erreur");
                valid_sentence = false;
            }
            else if((text.Length == 0) || (text == null))
            {
                MessageBox.Show("Erreur, il faut entrer du texte !", "Erreur");
                valid_sentence = false;
            }
            if(valid_sentence == true)
            {
                QRCode qr = new QRCode();
                qr.QRcode(text);
            }
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            var Page1 = new MainWindow();
            Page1.Show();
            this.Close();
        }
    }
}
