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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projet_info_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Menu principal de notre projet, on y accède au traitement d'image basique et autres menus disponibles
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Programme lancé au cliquement du bouton "Noir et Blanc"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images","Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage NoirEtBlanc = new MyImage(filename);
                NoirEtBlanc.Noir_et_Blanc();
            }
                
        }

        /// <summary>
        /// Programme pour l'effet "Grayshade", activé lorsque le bouton est cliqué
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage Grey = new MyImage(filename);
                Grey.gris();
            }
        }

        /// <summary>
        /// Bouton pour l'effet miroir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Miroir_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage Miroir = new MyImage(filename);
                Miroir.Miroir();
            }
        }

        /// <summary>
        /// Bouton pour l'effet Agrandissement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Agrandissement_Click(object sender, RoutedEventArgs e)
        {
            Input inputDialog = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            if (inputDialog.ShowDialog() == true)
            {
                string filename = inputDialog.Answer;
                filename = "Images/" + filename + ".bmp";
                int zoomvalue = 0;
                while((zoomvalue <=0) || (zoomvalue > 10))
                {
                    Input ZoomValue = new Input("Entrez la valeur du zoom souhaiter :", "Chiffre entre 1 et 10");
                    if (ZoomValue.ShowDialog() == true)
                    {
                        zoomvalue = int.Parse(ZoomValue.Answer);
                    }
                }
                MyImage Agrandir = new MyImage(filename);
                Agrandir.Agrandir(zoomvalue);
            }
        }

        private void Histo_button_Click(object sender, RoutedEventArgs e)
        {
            Input input = new Input("Entrez le nom de l'image a modifier (sans le .bmp) qui se trouve dans le dossier /Images", "Nom de l'image");
            string filename = "";
            if(input.ShowDialog() == true)
            {
                filename = input.Answer;
                filename = "Images/" + filename + ".bmp";
                MyImage histo = new MyImage(filename);
                Input Couleur = new Input("Choisissez une couleur à analyser pour l'histogramme :", "VERT / BLEU / ROUGE");
                string couleursel = "";
                if (Couleur.ShowDialog() == true)
                {
                    couleursel = Couleur.Answer;
                }
                couleursel = couleursel.ToUpper();
                switch (couleursel)
                {
                    case "VERT":
                        histo.hist(1);
                        break;
                    case "BLEU":
                        histo.hist(2);
                        break;
                    case "ROUGE":
                        histo.hist(3);
                        break;
                    default:
                        MessageBox.Show("Erreur dans l'entrée de la couleur, veuillez recommencer", "ERREUR");
                        break;
                }
            }
        }

        private void Fractacle_menu_Opener_Click(object sender, RoutedEventArgs e)
        {
            var Page2 = new Fractale_menu();
            Page2.Show(); 
            this.Close(); 
        }

        private void Convolution_Menu_Opener_Click(object sender, RoutedEventArgs e)
        {
            var Page2 = new Convolution_Menu();
            Page2.Show();
            this.Close();
        }

        private void QRCode_menu_Opener_Click(object sender, RoutedEventArgs e)
        {
            var Page2 = new QRCode_Menu();
            Page2.Show();
            this.Close();
        }
    }
}
