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
    /// Logique d'interaction pour Convolution_Menu.xaml
    /// </summary>
    public partial class Convolution_Menu : Window
    {
        public Convolution_Menu()
        {
            InitializeComponent();
        }

        private void Contour_button_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage Contour = new MyImage(filename);
                Contour.Contour();
            }
        }

        private void Flou_Button_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage Flou = new MyImage(filename);
                Flou.Flou();
            }
        }

        private void Repoussage_Button_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage Repoussage = new MyImage(filename);
                Repoussage.Repoussage();
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            var Page1 = new MainWindow();
            Page1.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
