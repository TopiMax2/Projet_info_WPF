using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projet_info_WPF
{
    class MyImage
    {
        private string Format_Image;
        private int file_size;
        private int Offset_file;
        private int Offset_image;
        private int largeur;
        private int hauteur;
        private int hresolution;
        private int vresolution;
        private Pixel[,] RGB_matrix;

        public int Largeur
        {
            get { return largeur; }
            set { largeur = value; }
        }

        public int Hauteur
        {
            get { return hauteur; }
            set { hauteur = value; }
        }

        public int FileSize
        {
            get { return file_size; }
            set { file_size = value; }
        }

        public Pixel[,] RGB_matrice
        {
            get { return RGB_matrix; }
            set { this.RGB_matrix = value; }
        }

        /// <summary>
        /// Constructeur principal a partir d'un fichier
        /// </summary>
        /// <param name="myfile"></param>
        public MyImage(string myfile)
        {
            // Création du tableau de bytes de l'image
            byte[] image = File.ReadAllBytes(myfile);
            Console.WriteLine("Informations de l'image : " + myfile);

            #region Lecture du Header
            //Léctures des métadonnées du fichier
            Console.WriteLine("\n Header \n");
            for (int i = 0; i < 14; i++)
            {
                // Lecture du format de l'image
                if (i == 0)
                {
                    byte[] format = new byte[2];
                    format[0] = image[0];
                    format[1] = image[1];
                    string str = Encoding.Default.GetString(format); // passage dans la base décimale
                    this.Format_Image = str;
                }
                Console.Write(image[i] + " ");
            }

            // Taille du fichier
            byte[] size_octals = new byte[4];   // Tableau contenant les bytes du poids du fichier
            for (int i = 2; i < 6; i++)
            {
                size_octals[i - 2] = image[i];
            }
            this.file_size = Convertir_Endian_To_Int(size_octals);


            Console.Write('\n');
            // Calcul de la taille du header du fichier (Offset total)
            byte[] Offset_image_bytes = new byte[4];
            for (int i = 10; i < 14; i++)
            {
                Offset_image_bytes[i - 10] = image[i];
            }
            this.Offset_file = Convertir_Endian_To_Int(Offset_image_bytes);

            // Calcul de la taille du header de l'image (Offset image)
            byte[] header_size_byte = new byte[4];
            for (int i = 14; i < 18; i++)
            {
                header_size_byte[i - 14] = image[i];
            }
            this.Offset_image = Convertir_Endian_To_Int(header_size_byte);

            //Lécture des métadonnées de l'image
            Console.WriteLine("\n HEADER INFO \n");
            for (int i = 14; i < this.Offset_image; i++)
            {
                Console.Write(image[i] + " ");
                // Calcul de la largeur
                if (i == 18)
                {
                    byte[] largeur_bytes = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        largeur_bytes[j] = image[j + 18];
                    }
                    this.largeur = Convertir_Endian_To_Int(largeur_bytes);
                }

                //Calcul de la hauteur
                if (i == 22)
                {
                    byte[] hauteur_bytes = new byte[4];
                    for (int j = 0; j < 4; j++)
                    {
                        hauteur_bytes[j] = image[j + 22];
                    }
                    this.hauteur = Convertir_Endian_To_Int(hauteur_bytes);
                }
            }
            byte[] hres_endian = new byte[4];
            byte[] vres_endian = new byte[4];
            for (int h = 0; h < 4; h++)
            {
                hres_endian[h] = image[h + 38];
                vres_endian[h] = image[h + 42];
            }
            this.hresolution = Convertir_Endian_To_Int(hres_endian);
            this.vresolution = Convertir_Endian_To_Int(vres_endian);

            Console.Write('\n');
            #endregion
            #region Création de la matrice de Pixel
            // Création de la matrice de bytes contenant les bytes RGB
            this.RGB_matrix = new Pixel[this.hauteur, this.largeur];
            int index = this.Offset_file;
            int padding = (4 - ((this.largeur * 3) % 4)) % 4; // calcul du padding
            for (int i = RGB_matrix.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < RGB_matrix.GetLength(1); j++)
                {
                    byte[] RGB_bytes = new byte[3];
                    RGB_bytes[0] = image[index];
                    RGB_bytes[1] = image[index + 1];
                    RGB_bytes[2] = image[index + 2];
                    this.RGB_matrix[i, j] = new Pixel(RGB_bytes);
                    index += 3;
                }
                if (padding != 0)
                {
                    for (int k = 0; k < padding; k++)
                    {
                        index += 1;
                    }
                }
            }
            #endregion

            #region Affichage Matrice RGB
            /*
            // Affichage de la matrice pour vérifier les erreurs
            Console.WriteLine("IMAGE RGB BYTES :");
            for(int i = 0; i < RGB_matrix.GetLength(0);i++)
            {
                for(int j = 0; j < RGB_matrix.GetLength(1); j++)
                {
                    Console.WriteLine(RGB_matrix[i, j].Affichage_byte());
                }
                Console.Write('\n');
            }

            Console.Write('\n');*/
            #endregion
            #region Affichage des informations générales pour vérification
            //Affichage des informations générales (afin de vérifier que tout est correct)
            Console.WriteLine("INFORMATIONS : ");
            Console.WriteLine("[*] File type : " + this.Format_Image);
            Console.WriteLine("[*] File size : " + this.file_size);
            Console.WriteLine("[*] Offset fichier : " + this.Offset_file);
            Console.WriteLine("[*] Offset de l'image: " + this.Offset_image);
            Console.WriteLine("[*] Largeur de l'image : " + this.largeur);
            Console.WriteLine("[*] Hauteur de l'image : " + this.hauteur);
            #endregion
        }

        /// <summary>
        /// Constructeur qui permet de copier une image
        /// </summary>
        /// <param name="im"></param>
        public MyImage(MyImage im)
        {
            this.Format_Image = im.Format_Image;
            this.file_size = im.file_size;
            this.largeur = im.largeur;
            this.hauteur = im.hauteur;
            this.Offset_file = im.Offset_file;
            this.Offset_image = im.Offset_image;
            this.hresolution = im.hresolution;
            this.vresolution = im.vresolution;


            this.RGB_matrix = new Pixel[im.RGB_matrix.GetLength(0), im.RGB_matrix.GetLength(1)];
            for (int i = 0; i < im.RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < im.RGB_matrix.GetLength(1); j++)
                {
                    this.RGB_matrix[i, j] = im.RGB_matrix[i, j];
                }
            }
        }

        public MyImage(int largeur, int hauteur)
        {
            this.Format_Image = "BM";
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.RGB_matrix = new Pixel[hauteur, largeur];
            for (int i = 0; i < RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < RGB_matrix.GetLength(1); j++)
                {
                    byte[] a = new byte[3] { 0, 0, 0 };
                    Pixel noir = new Pixel(a);
                    this.RGB_matrix[i, j] = noir;
                }
            }
            this.Offset_file = 54;
            this.file_size = (3 * (hauteur * largeur)) + this.Offset_file;
            this.Offset_image = 40;
            this.hresolution = 1;
            this.vresolution = 1;
        }

        public static int Convertir_Endian_To_Int(byte[] tab)
        {
            double calcul = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                calcul += tab[i] * Math.Pow(256, i);
            }
            int sortie = Convert.ToInt32(calcul);
            return sortie;
        }

        static public byte[] Convertir_int_To_Endian(int number, int arraylength)
        {
            byte[] result = new byte[arraylength];
            for (int i = 0; i < arraylength; i++)
            {
                result[i] = (byte)(((uint)number >> i * 8) & 0xFF);   //Shift right i*8 bits and add a 0 if necessary
            }
            return result;
        }

        /// <summary>
        /// Fonction principale qui permet de transformer un objet de notre classe MyImage en un ficher bmp executable
        /// on entre simplement le nom du fichier a sauvegarder car toutes les informations du header et autres sont directement intégrées à l'objet
        /// on reconstruit tout d'abord le header dans un tableau de byte grâce aux propriétés de l'objet de classe
        /// on écrit ensuite du bas vers le haut les bytes qui compose l'image puis on écrit tout les bytes dans un fichier .bmp
        /// 
        /// Bonus : a la fin on affiche comme quoi la fonction a bien enregistrer l'image et l'affiche a l'utilisateur sur une fenêtre de résultat
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool From_Image_To_File(string filename)
        {
            byte[] result = new byte[this.file_size];
            #region Ecriture du header

            /// HEADER DU FICHIER


            // FORMAT IMAGE
            byte[] format_image_bytes = Encoding.ASCII.GetBytes(Format_Image);

            // Taille du fichier
            byte[] TailleFichier = Convertir_int_To_Endian(file_size, 4);

            // Taille du header (total)
            byte[] Offset_fichier = Convertir_int_To_Endian(Offset_file, 4);

            /// HEADER DE L'IMAGE

            // Taille du header (image)
            byte[] Offset_image_byte = Convertir_int_To_Endian(Offset_image, 4);

            // Largeur de l'image
            byte[] Largeur_fichier = Convertir_int_To_Endian(largeur, 4);

            // Hauteur de l'image
            byte[] hauteur_fichier = Convertir_int_To_Endian(hauteur, 4);

            result[0] = format_image_bytes[0];
            result[1] = format_image_bytes[1];
            // Pour les infos du header codées sur 4 bits
            for (int j = 0; j < 4; j++)
            {
                result[j + 2] = TailleFichier[j];
                result[j + 6] = 0; // par défaut ces valeurs sont égales a 0
                result[j + 10] = Offset_fichier[j];
                result[j + 14] = Offset_image_byte[j];
                result[j + 18] = Largeur_fichier[j];
                result[j + 22] = hauteur_fichier[j];

                byte[] compression_method = Convertir_int_To_Endian(0, 4); // Nous n'utilisons pas de compression donc, on met les bits a 0
                result[j + 30] = compression_method[j];

                int image_size = this.file_size - this.Offset_file;
                byte[] size = Convertir_int_To_Endian(image_size, 4);
                result[j + 34] = size[j];

                byte[] hres_endian = Convertir_int_To_Endian(this.hresolution, 4);
                result[j + 38] = hres_endian[j];

                byte[] vres_endian = Convertir_int_To_Endian(this.vresolution, 4);
                result[j + 42] = vres_endian[j];

                result[j + 46] = (byte)0;

                result[j + 50] = (byte)0;
            }

            // Pour les infos du header codées sur 2 bits
            for (int i = 0; i < 2; i++)
            {
                byte[] color_plane = Convertir_int_To_Endian(1, 2); // Color_plane doit toujours être égal a 1
                result[i + 26] = color_plane[i];

                byte[] number_bits_per_pixel = Convertir_int_To_Endian(24, 2); // Nombre de bits par pixel (24 par défaut)
                result[i + 28] = number_bits_per_pixel[i];
            }
            #endregion

            int index = this.Offset_file;
            int padding = (4 - ((this.largeur * 3) % 4)) % 4;
            for (int i = this.RGB_matrix.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < this.RGB_matrix.GetLength(1); j++)
                {
                    byte[] RGB_to_write = RGB_matrix[i, j].Pixel_en_bytes();
                    result[index] = RGB_to_write[0];
                    result[index + 1] = RGB_to_write[1];
                    result[index + 2] = RGB_to_write[2];
                    index += 3;
                }
                if (padding != 0)
                {
                    for (int k = 0; k < padding; k++)
                    {
                        result[index] = (byte)0;
                        index += 1;
                    }
                }
            }
            File.WriteAllBytes(filename, result); // Ecriture de l'image
            MessageBox.Show("Image correctement enregistrée dans le dossier ./Images/ sous le nom : " + filename+" !", "Success");
            Result affichage = new Result(filename);
            affichage.Show();
            return true;
        }



        #region LES EFFETS IMAGES
        /// <summary>
        /// Effet Noir et Blanc, a chaque pixel, nous faisons la moyenne de la valeur des bytes de couleur
        /// si la moyenne dépasse 128 on met un pixel blanc et dans l'autre cas un pixel noir
        /// </summary>
        public void Noir_et_Blanc()
        {
            Console.WriteLine("[*] Application de l'effet Noir et Blanc...");
            MyImage NBimage = new MyImage(this);

            for (int i = 0; i < NBimage.RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < NBimage.RGB_matrix.GetLength(1); j++)
                {
                    // On calcule la moyenne de la valeur des bytes de couleur, si on est au dela de 128 le pixel sera blanc sinon il sera noir.
                    int moyenne = ((NBimage.RGB_matrix[i, j].R + NBimage.RGB_matrix[i, j].G + NBimage.RGB_matrix[i, j].B) / 3);
                    if (moyenne > 128)
                    {
                        moyenne = 255;
                    }
                    else
                    {
                        moyenne = 0;
                    }
                    byte[] retour = new byte[3] { (byte)moyenne, (byte)moyenne, (byte)moyenne };
                    NBimage.RGB_matrix[i, j] = new Pixel(retour);
                }
            }
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            NBimage.From_Image_To_File(filename);
        }
        /// <summary>
        /// Application du filtre greyshade, a chaque pixel on fait la moyenne de chaque valeur des pixels rouge, vert et bleu afin de déterminer la valeur de gris
        /// que l'on applique ensuite au pixel
        /// </summary>
        public void gris()
        {
            Console.WriteLine("[*] Application de la nuance de gris sur l'image...");
            MyImage Grisée = new MyImage(this);
            for (int i = 0; i < Grisée.RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Grisée.RGB_matrix.GetLength(1); j++)
                {
                    int valeur_gris = (Grisée.RGB_matrix[i, j].R) / 3 + (Grisée.RGB_matrix[i, j].G) / 3 + (Grisée.RGB_matrix[i, j].B) / 3;
                    byte[] retour = new byte[3] { (byte)valeur_gris, (byte)valeur_gris, (byte)valeur_gris };
                    Grisée.RGB_matrix[i, j] = new Pixel(retour);
                }
            }
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            Grisée.From_Image_To_File(filename);
        }
        /// <summary>
        /// Rotation 90 degrés vers la droite,
        /// inversion de la largeur et de la hauteur puis on écrit les lignes dans les colonnes et inversement
        /// </summary>
        public void rotation90degrésDroite()
        {
            Console.WriteLine("Retournement de l'image de 90 degrés vers la droite...");
            Pixel[,] retour = new Pixel[this.RGB_matrix.GetLength(1), this.RGB_matrix.GetLength(0)];
            int l = retour.GetLength(1) - 1;
            for (int i = 0; i < this.RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < this.RGB_matrix.GetLength(1); j++)
                {
                    retour[j, l] = this.RGB_matrix[i, j];
                }
                l--;
            }
            this.RGB_matrix = retour;
            int temp = this.hauteur;
            this.hauteur = this.largeur;
            this.largeur = temp;
            Console.WriteLine("[*] Succès !");
            Console.WriteLine("[?] Sous quel nom voulez vous sauvegarder cette image ?");
            string filename = "Images/" + Console.ReadLine() + ".bmp";
            this.From_Image_To_File(filename);
        }

        /// <summary>
        /// Effet miroir, Création d'une matrice de pixel de la même taille que celle de notre image de base
        /// on lit ensuite les pixels de la droite vers la gauche et on les écrits dans la nouvelle matrice de pixel qui nous donnera notre image avec l'effet miroir
        /// </summary>
        public void Miroir()
        {
            Console.WriteLine("[*] Application de l'effet miroir a votre image...");
            MyImage Miroir = new MyImage(this);
            int l = this.RGB_matrix.GetLength(1) - 1;
            for (int i = 0; i < Miroir.RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Miroir.RGB_matrix.GetLength(1); j++)
                {
                    Miroir.RGB_matrix[i, j] = this.RGB_matrix[i, l];
                    l--;
                }
                l = Miroir.RGB_matrix.GetLength(1) - 1;
            }
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            Miroir.From_Image_To_File(filename);
        }

        /// <summary>
        /// Fonction zoom, on entre une valeur entre 1 et 10
        /// on créer une nouvelle matrice où l'on multiplie sa taille par la valeur entrée
        /// puis dans chaque pixel nous prenons un pixel à la position divisée par le ratio de zoom dans l'ancienne image
        /// </summary>
        /// <param name="value"></param>
        public void Agrandir(int value)
        {
            Console.WriteLine("[*] Agrandissement de votre image...");
            MyImage tozoom = new MyImage(this.RGB_matrice.GetLength(1)*value, this.RGB_matrice.GetLength(0)*value);
            for (int i = 0; i < tozoom.RGB_matrice.GetLength(0); i++)
            {
                for (int j = 0; j < tozoom.RGB_matrice.GetLength(1); j++)
                {
                    tozoom.RGB_matrice[i, j] = this.RGB_matrix[(i / value), (j / value)];
                }
            }
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            tozoom.From_Image_To_File(filename);
        }


        #endregion


        #region Filtres / Convolution

        public void Convolution(int[,] conv, bool flou)
        {
            Pixel[,] imconv = new Pixel[hauteur, largeur];
            int resB, resG, resR = 0;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {

                    #region Cas particuliers sur les bords et les coins de la matrice
                    //coin supérieur gauche
                    if (i == 0 && j == 0)
                    {
                        resB = RGB_matrix[0, 0].B * conv[1, 1] + RGB_matrix[0, 1].B * conv[1, 2] +
                            RGB_matrix[1, 0].B * conv[2, 1] + RGB_matrix[1, 1].B * conv[2, 2];

                        resG = RGB_matrix[0, 0].G * conv[1, 1] + RGB_matrix[0, 1].G * conv[1, 2] +
                            RGB_matrix[1, 0].G * conv[2, 1] + RGB_matrix[1, 1].G * conv[2, 2];

                        resR = RGB_matrix[0, 0].R * conv[1, 1] + RGB_matrix[0, 1].R * conv[1, 2] +
                            RGB_matrix[1, 0].R * conv[2, 1] + RGB_matrix[1, 1].R * conv[2, 2];
                    }

                    //coin supérieur droite
                    else if (i == 0 && j == largeur - 1)
                    {
                        resB = RGB_matrix[0, largeur - 2].B * conv[1, 0] + RGB_matrix[0, largeur - 1].B * conv[1, 1] +
                            RGB_matrix[1, largeur - 2].B * conv[2, 0] + RGB_matrix[1, largeur - 1].B * conv[2, 1];

                        resG = RGB_matrix[0, largeur - 2].G * conv[1, 0] + RGB_matrix[0, largeur - 1].G * conv[1, 1] +
                            RGB_matrix[1, largeur - 2].G * conv[2, 0] + RGB_matrix[1, largeur - 1].G * conv[2, 1];

                        resR = RGB_matrix[0, largeur - 2].R * conv[1, 0] + RGB_matrix[0, largeur - 1].R * conv[1, 1] +
                            RGB_matrix[1, largeur - 2].R * conv[2, 0] + RGB_matrix[1, largeur - 1].R * conv[2, 1];
                    }

                    //coin inférieur gauche
                    else if (i == hauteur - 1 && j == 0)
                    {
                        resB = RGB_matrix[hauteur - 2, 0].B * conv[0, 1] + RGB_matrix[hauteur - 2, 1].B * conv[0, 2] +
                            RGB_matrix[hauteur - 1, 0].B * conv[1, 1] + RGB_matrix[hauteur - 1, 1].B * conv[1, 2];

                        resG = RGB_matrix[hauteur - 2, 0].G * conv[0, 1] + RGB_matrix[hauteur - 2, 1].G * conv[0, 2] +
                            RGB_matrix[hauteur - 1, 0].G * conv[1, 1] + RGB_matrix[hauteur - 1, 1].G * conv[1, 2];

                        resR = RGB_matrix[hauteur - 2, 0].R * conv[0, 1] + RGB_matrix[hauteur - 2, 1].R * conv[0, 2] +
                            RGB_matrix[hauteur - 1, 0].R * conv[1, 1] + RGB_matrix[hauteur - 1, 1].R * conv[1, 2];
                    }

                    //coin inférieur droite
                    else if (i == hauteur - 1 && j == largeur - 1)
                    {
                        resB = RGB_matrix[hauteur - 2, largeur - 2].B * conv[0, 0] + RGB_matrix[hauteur - 2, largeur - 1].B * conv[0, 1] +
                            RGB_matrix[hauteur - 1, largeur - 2].B * conv[1, 0] + RGB_matrix[hauteur - 1, largeur - 1].B * conv[1, 1];

                        resG = RGB_matrix[hauteur - 2, largeur - 2].G * conv[0, 0] + RGB_matrix[hauteur - 2, largeur - 1].G * conv[0, 1] +
                            RGB_matrix[hauteur - 1, largeur - 2].G * conv[1, 0] + RGB_matrix[hauteur - 1, largeur - 1].G * conv[1, 1];

                        resR = RGB_matrix[hauteur - 2, largeur - 2].R * conv[0, 1] + RGB_matrix[hauteur - 2, largeur - 1].R * conv[0, 2] +
                            RGB_matrix[hauteur - 1, largeur - 2].R * conv[1, 1] + RGB_matrix[hauteur - 1, largeur - 1].R * conv[1, 2];
                    }

                    //bord du haut
                    else if (i == 0)
                    {
                        resB = RGB_matrix[i, j - 1].B * conv[1, 0] + RGB_matrix[i, j].B * conv[1, 1] + RGB_matrix[i, j + 1].B * conv[1, 2] +
                        RGB_matrix[i + 1, j - 1].B * conv[2, 0] + RGB_matrix[i + 1, j].B * conv[2, 1] + RGB_matrix[i + 1, j + 1].B * conv[2, 2];

                        resG = RGB_matrix[i, j - 1].G * conv[1, 0] + RGB_matrix[i, j].G * conv[1, 1] + RGB_matrix[i, j + 1].G * conv[1, 2] +
                            RGB_matrix[i + 1, j - 1].G * conv[2, 0] + RGB_matrix[i + 1, j].G * conv[2, 1] + RGB_matrix[i + 1, j + 1].G * conv[2, 2];

                        resR = RGB_matrix[i, j - 1].R * conv[1, 0] + RGB_matrix[i, j].R * conv[1, 1] + RGB_matrix[i, j + 1].R * conv[1, 2] +
                            RGB_matrix[i + 1, j - 1].R * conv[2, 0] + RGB_matrix[i + 1, j].R * conv[2, 1] + RGB_matrix[i + 1, j + 1].R * conv[2, 2];
                    }


                    //bord de gauche
                    else if (j == 0)
                    {
                        resB = RGB_matrix[i - 1, j].B * conv[0, 1] + RGB_matrix[i - 1, j + 1].B * conv[0, 2] +
                            RGB_matrix[i, j].B * conv[1, 1] + RGB_matrix[i, j + 1].B * conv[1, 2] +
                            RGB_matrix[i + 1, j].B * conv[2, 1] + RGB_matrix[i + 1, j + 1].B * conv[2, 2];

                        resG = RGB_matrix[i - 1, j].G * conv[0, 1] + RGB_matrix[i - 1, j + 1].G * conv[0, 2] +
                            RGB_matrix[i, j].G * conv[1, 1] + RGB_matrix[i, j + 1].G * conv[1, 2] +
                            RGB_matrix[i + 1, j].G * conv[2, 1] + RGB_matrix[i + 1, j + 1].G * conv[2, 2];

                        resR = RGB_matrix[i - 1, j].R * conv[0, 1] + RGB_matrix[i - 1, j + 1].R * conv[0, 2] +
                            RGB_matrix[i, j].R * conv[1, 1] + RGB_matrix[i, j + 1].R * conv[1, 2] +
                            RGB_matrix[i + 1, j].R * conv[2, 1] + RGB_matrix[i + 1, j + 1].R * conv[2, 2];
                    }

                    //bord de droite
                    else if (j == largeur - 1)
                    {
                        resB = RGB_matrix[i - 1, j - 1].B * conv[0, 0] + RGB_matrix[i - 1, j].B * conv[0, 1] +
                            RGB_matrix[i, j - 1].B * conv[1, 0] + RGB_matrix[i, j].B * conv[1, 1] +
                            RGB_matrix[i + 1, j - 1].B * conv[2, 0] + RGB_matrix[i + 1, j].B * conv[2, 1];

                        resG = RGB_matrix[i - 1, j - 1].G * conv[0, 0] + RGB_matrix[i - 1, j].G * conv[0, 1] +
                            RGB_matrix[i, j - 1].G * conv[1, 0] + RGB_matrix[i, j].G * conv[1, 1] +
                            RGB_matrix[i + 1, j - 1].G * conv[2, 0] + RGB_matrix[i + 1, j].G * conv[2, 1];

                        resR = RGB_matrix[i - 1, j - 1].R * conv[0, 0] + RGB_matrix[i - 1, j].R * conv[0, 1] +
                            RGB_matrix[i, j - 1].R * conv[1, 0] + RGB_matrix[i, j].R * conv[1, 1] +
                            RGB_matrix[i + 1, j - 1].R * conv[2, 0] + RGB_matrix[i + 1, j].R * conv[2, 1];
                    }

                    //bord du bas
                    else if (i == hauteur - 1)
                    {
                        resB = RGB_matrix[i - 1, j - 1].B * conv[0, 0] + RGB_matrix[i - 1, j].B * conv[0, 1] + RGB_matrix[i - 1, j + 1].B * conv[0, 2] +
                        RGB_matrix[i, j - 1].B * conv[1, 0] + RGB_matrix[i, j].B * conv[1, 1] + RGB_matrix[i, j + 1].B * conv[1, 2];

                        resG = RGB_matrix[i - 1, j - 1].G * conv[0, 0] + RGB_matrix[i - 1, j].G * conv[0, 1] + RGB_matrix[i - 1, j + 1].G * conv[0, 2] +
                        RGB_matrix[i, j - 1].G * conv[1, 0] + RGB_matrix[i, j].G * conv[1, 1] + RGB_matrix[i, j + 1].G * conv[1, 2];

                        resR = RGB_matrix[i - 1, j - 1].R * conv[0, 0] + RGB_matrix[i - 1, j].R * conv[0, 1] + RGB_matrix[i - 1, j + 1].R * conv[0, 2] +
                        RGB_matrix[i, j - 1].R * conv[1, 0] + RGB_matrix[i, j].R * conv[1, 1] + RGB_matrix[i, j + 1].R * conv[1, 2];
                    }

                    #endregion
                    else
                    {

                        resB = RGB_matrix[i - 1, j - 1].B * conv[0, 0] + RGB_matrix[i - 1, j].B * conv[0, 1] + RGB_matrix[i - 1, j + 1].B * conv[0, 2] +
                            RGB_matrix[i, j - 1].B * conv[1, 0] + RGB_matrix[i, j].B * conv[1, 1] + RGB_matrix[i, j + 1].B * conv[1, 2] +
                            RGB_matrix[i + 1, j - 1].B * conv[2, 0] + RGB_matrix[i + 1, j].B * conv[2, 1] + RGB_matrix[i + 1, j + 1].B * conv[2, 2];


                        resG = RGB_matrix[i - 1, j - 1].G * conv[0, 0] + RGB_matrix[i - 1, j].G * conv[0, 1] + RGB_matrix[i - 1, j + 1].G * conv[0, 2] +
                            RGB_matrix[i, j - 1].G * conv[1, 0] + RGB_matrix[i, j].G * conv[1, 1] + RGB_matrix[i, j + 1].G * conv[1, 2] +
                            RGB_matrix[i + 1, j - 1].G * conv[2, 0] + RGB_matrix[i + 1, j].G * conv[2, 1] + RGB_matrix[i + 1, j + 1].G * conv[2, 2];


                        resR = RGB_matrix[i - 1, j - 1].R * conv[0, 0] + RGB_matrix[i - 1, j].R * conv[0, 1] + RGB_matrix[i - 1, j + 1].R * conv[0, 2] +
                            RGB_matrix[i, j - 1].R * conv[1, 0] + RGB_matrix[i, j].R * conv[1, 1] + RGB_matrix[i, j + 1].R * conv[1, 2] +
                            RGB_matrix[i + 1, j - 1].R * conv[2, 0] + RGB_matrix[i + 1, j].R * conv[2, 1] + RGB_matrix[i + 1, j + 1].R * conv[2, 2];

                    }

                    /*byte b = Convert.ToByte(resB);
                    byte g = Convert.ToByte(resG);
                    byte r = Convert.ToByte(resR);*/


                    /*Math.Abs(resB);
                    Math.Abs(resG);
                    Math.Abs(resR);*/
                    if (flou == true)
                    {
                        resB /= 10;
                        resG /= 10;
                        resR /= 10;
                    }
                    if (resB < 0) { resB = 0; }
                    if (resB > 255) { resB = 255; }

                    if (resG < 0) { resG = 0; }
                    if (resG > 255) { resG = 255; }

                    if (resR < 0) { resR = 0; }
                    if (resR > 255) { resR = 255; }

                    byte[] imbyte = { (byte)resR, (byte)resG, (byte)resB };
                    imconv[i, j] = new Pixel(imbyte);
                }
            }
            RGB_matrix = imconv;
        }
        public void Contour()
        {
            Console.WriteLine("[*] Détection de contour de l'image...");

            //filtre pour détecter les contours
            int[,] conv = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            Convolution(conv, false);
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            From_Image_To_File(filename);
        }

        public void Flou()
        {
            Console.WriteLine("[*] Floutage de l'image...");

            //filtre flou
            int[,] conv = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            Convolution(conv, true);
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            From_Image_To_File(filename);
        }


        public void Repoussage()
        {
            Console.WriteLine("[*} Repoussage de l'image...");

            //filtre pour repoussage de l'image
            int[,] conv = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            Convolution(conv, false);
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            From_Image_To_File(filename);
        }

        public void AugmenterContraste()
        {
            Console.WriteLine("[*] Augmentation du contraste de l'image...");

            //filtre pour augmenter le contraste de l'image 
            int[,] conv = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            Convolution(conv, false);
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            From_Image_To_File(filename);
        }

        public void RenforcerBord()
        {
            Console.WriteLine("[*] Renforcement des bords de l'image...");

            //filtre pour renforcer les bords d'une image
            int[,] conv = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            Convolution(conv, false);
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            From_Image_To_File(filename);
        }


        #endregion

        #region Histogramme

        public void hist(int couleur)

        {
            int[] histo = new int[256];
            for (int pix = 0; pix < 256; pix++)
            {
                histo[pix] = 0;
            }
            for (int i = 0; i < RGB_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < RGB_matrix.GetLength(1); j++)
                {
                    if (couleur == 1) //VERT
                    {
                        histo[Convert.ToInt32(RGB_matrix[i, j].B)]++;
                    }
                    if (couleur == 2) //Bleu
                    {
                        histo[Convert.ToInt32(RGB_matrix[i, j].R)]++;
                    }
                    if (couleur == 3) //ROUGE
                    {
                        histo[Convert.ToInt32(RGB_matrix[i, j].G)]++;
                    }
                }
            }
            int max = histo.Max();
            Pixel[,] histogramme = new Pixel[max, 256];
            for (int ligne = max - 1; ligne >= 0; ligne--)
            {
                for (int colonne = 0; colonne < histo.Length; colonne++)
                {
                    if (histo[colonne] != 0)
                    {
                        if (couleur == 1) //VERT
                        {
                            histogramme[ligne, colonne] = new Pixel(0, 255, 0);
                        }
                        if (couleur == 2) //Bleu
                        {
                            histogramme[ligne, colonne] = new Pixel(255, 0, 0);
                        }
                        if (couleur == 3) //ROUGE
                        {
                            histogramme[ligne, colonne] = new Pixel(0, 0, 255);
                        }
                        histo[colonne]--;
                    }
                    else
                    {
                        histogramme[ligne, colonne] = new Pixel(0, 0, 0);
                    }
                }
            }
            MyImage hist = new MyImage(256, max);
            hist.RGB_matrix = histogramme;
            Input nom = new Input("Sous quel nom voulez vous enregistrer la modification ?", "Nom du fichier (sans .bmp)");
            string filename = "./Images/";
            if (nom.ShowDialog() == true)
            {
                filename += nom.Answer;
            }
            filename += ".bmp";
            hist.From_Image_To_File(filename);

        }

        #endregion
    }
}
