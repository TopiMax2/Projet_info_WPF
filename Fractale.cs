using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_info_WPF
{
    class Fractale
    {
        private MyImage fractale;

        public Fractale(int hauteur, int largeur)
        {
            this.fractale = new MyImage(largeur, hauteur);
        }

        public MyImage Mandelbrot(Fractale a)
        {
            Pixel[,] newMatrix = a.fractale.RGB_matrice;
            int imax = 100;
            double x1 = -2;
            double x2 = 0.5;
            double y1 = -1.25;
            double y2 = 1.25;
            double zoom = 1;
            double image_x = (x2 - x1) * zoom;
            double image_y = (y2 - y1) * zoom;
            for (int i = 0; i < newMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < newMatrix.GetLength(1); j++)
                {
                    double cx = (i * (x2 - x1) / a.fractale.Largeur + x1);
                    double cy = (j * (y1 - y2) / a.fractale.Hauteur + y2);
                    double xn = 0;
                    double yn = 0;
                    int m = 0;
                    while ((xn * xn + yn * yn) < 4 && (m < imax))
                    {
                        double tmp_x = xn;
                        double tmp_y = yn;
                        xn = tmp_x * tmp_x - tmp_y * tmp_y + cx;
                        yn = 2 * tmp_x * tmp_y + cy;
                        m++;
                    }

                    if (m == imax)
                    {
                        byte[] b = new byte[3] { 255, 255, 255 };
                        Pixel blanc = new Pixel(b);
                        newMatrix[j, i] = blanc;
                    }
                    else
                    {
                        byte[] c = new byte[3] { (byte)((3 * m) % 256), (byte)((1 * m) % 256), (byte)((10 * m) % 256) };
                        Pixel couleur = new Pixel(c);
                        newMatrix[j, i] = couleur;

                    }
                }
            }
            a.fractale.RGB_matrice = newMatrix;
            return a.fractale;

        }

        public MyImage Julia(Fractale a)
        {
            Pixel[,] newMatrix = a.fractale.RGB_matrice;
            int largeur = a.fractale.Largeur; // Pref = 800
            int hauteur = a.fractale.Hauteur; // Pref = 600
            double zoom = 0.75;
            int maxiter = 255;
            double moveX = 0.5;
            double moveY = 0;
            double cX = -0.7;
            double cY = 0.27015;
            double zx, zy, tmp;
            int m;

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    zx = 1.5 * (i - largeur / 2) / (0.5 * zoom * largeur) + moveX;
                    zy = 1.0 * (j - hauteur / 2) / (0.5 * zoom * hauteur) + moveY;
                    m = maxiter;
                    while (zx * zx + zy * zy < 4 && m > 1)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        m--;
                    }
                    byte[] b = new byte[3] { (byte)((m >> 5) * 36), (byte)((m >> 3) * 36), (byte)(m * 85) };
                    Pixel couleur = new Pixel(b);
                    newMatrix[i, j] = couleur;
                }
            }
            a.fractale.RGB_matrice = newMatrix;
            return a.fractale;
        }
    }
}
