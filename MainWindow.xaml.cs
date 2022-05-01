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
    /// Menu principal de notre projet, on y accède au traitement d'image basique, la création d'image, la convolution, et bien plus
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Programme lancer au cliquement du bouton "Noir et Blanc"
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

        private void Convolution_Infos_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Fractales_Infos_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
