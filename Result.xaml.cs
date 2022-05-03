using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logique d'interaction pour Result.xaml
    /// lorsque la fenêtre s'ouvre elle affiche l'image à l'addresse entrée
    /// ( ! ATTENTION ! : BitmapImage n'a rien a voir avec la classe BITMAP elle ne traite en rien nos images et ne sert
    /// qu'a afficher l'image retour)
    /// </summary>
    public partial class Result : Window
    {
        public Result(string filename)
        {
            InitializeComponent();
            string strUri2 = Directory.GetCurrentDirectory() + filename;
            ImgDynamic.Source = new BitmapImage(new Uri(strUri2));
        }

        /// <summary>
        /// Bouton qui referme la page d'affichage de l'image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
