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
    /// Logique d'interaction pour Fractale_menu.xaml
    /// </summary>
    public partial class Fractale_menu : Window
    {
        public Fractale_menu()
        {
            InitializeComponent();
        }

        private void Mandelbrot_button_Click(object sender, RoutedEventArgs e)
        {
            Fractale mandelbrot = new Fractale(500, 500);
            MyImage retour = mandelbrot.Mandelbrot(mandelbrot);
            retour.From_Image_To_File("./Images/mandelbrot.bmp");
            MessageBox.Show("Fractale de Mandelbrot générée avec succès !","Success !");
        }

        private void Julia_button_Click(object sender, RoutedEventArgs e)
        {
            Fractale Julia = new Fractale(600, 800);
            MyImage retour = Julia.Julia(Julia);
            retour.From_Image_To_File("./Images/Julia.bmp");
            MessageBox.Show("Fractale de Julia générée avec succès !", "Success !");
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            var Page1 = new MainWindow();
            Page1.Show();
            this.Close();
        }
    }
}
