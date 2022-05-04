using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Projet_info_WPF
{
    public class QRCode
    {
        public QRCode() { }

        /// <summary>
        /// Fonction qui met en place toutes les étapes pour réaliser un qrcode
        /// </summary>
        /// <param name="txt">Texte à convertir en qrcode</param>
        public void QRcode(string txt)
        {
            /* Commencer dès le début à savoir quelle version sera le qrcode   */
            txt = txt.ToUpper();
            int v = 1;
            if (txt.Length > 25)
            {
                v = 2;
            }

            /*Création d'une image de classe MyImage pour notre qrcode
             * La largeur et la hauteur de notre image qrcode est en fonction de la version
             * On passe de 4 en 4 (version 1 : 21; version 2 : 25)  
             */
            MyImage qrcode = new MyImage(17 + (v * 4), 17 + (v * 4));


            #region Structure message final (données + correction)

            /*  Mode alphanumérique seulement   */
            string mode = "0010";

            /*  Taille du texte en binaire (on le veut en string)  */
            int[] t = TailleEnByte(txt);
            string teb = "";
            for (int i = 0; i < t.Length; i++)
            {
                teb += Convert.ToString(t[i]);
            }

            /* Etapes permettant de structurer le message final
             * https://www.thonky.com/qr-code-tutorial/introduction 
             */
            string ctb = CharToByte(txt);
            string tot = TotalBits(mode, teb, ctb);
            string tot_with_term = Terminaison(tot, v);
            string vraitot = MultipleHuit(tot_with_term);

            string octet1 = "11101100";
            string octet2 = "00010001";

            if (v == 1)
            {
                do
                {
                    if (vraitot.Length < 152 - 16)
                    {
                        vraitot += octet1 + octet2;
                    }
                    else
                    {
                        vraitot += octet1;
                    }
                }
                while (vraitot.Length < 152);
            }
            else if (v == 2)
            {
                do
                {
                    if (vraitot.Length < 272 - 16)
                    {
                        vraitot += octet1 + octet2;
                    }
                    else
                    {
                        vraitot += octet1;
                    }
                }
                while (vraitot.Length < 272);
            }

            byte[] bytestxt = new byte[vraitot.Length / 8];
            for (int i = 0; i < bytestxt.Length; i++)
            {
                bytestxt[i] = Convert.ToByte(vraitot.Substring(i * 8, 8), 2);
            }


            /*  Ajout de la correction dans notre chaîne binaire  */
            string cor = Correction(txt, bytestxt);
            vraitot += cor;
            #endregion


            #region Placement des modèles de fonction
            /*  Placement des modèles de fonction dans leur zone spécifique  
             *  https://www.thonky.com/qr-code-tutorial/module-placement-matrix */

            int[,] inqrcode = new int[21 + ((v - 1) * 4), 21 + ((v - 1) * 4)]; // Matrice de int qui va nous servir à stocker les binaires 

            int temp = inqrcode.GetLength(0) - 5;
            //int[,] align = { { 6, 6 }, { 6, 18 }, { 18, 6 }, { 18, 18 } };
            for (int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for (int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    #region Modèles de recherche (les trois blocs dans les coins)
                    //patterns noir horizontal
                    if (i == 0 || i == 6)
                    {
                        if (j < 7 || j > inqrcode.GetLength(1) - 7)
                        {
                            inqrcode[i, j] = 3;
                        }
                    }
                    if (i == inqrcode.GetLength(0) - 1 || i == inqrcode.GetLength(0) - 7)
                    {
                        if (j < 7)
                        {
                            inqrcode[i, j] = 3;
                        }
                    }

                    //patterns noir verticale
                    if (j == 0 || j == 6)
                    {
                        if (i < 7 || i > inqrcode.GetLength(0) - 7)
                        {
                            inqrcode[i, j] = 3;
                        }
                    }
                    if (j == inqrcode.GetLength(1) - 1 || j == inqrcode.GetLength(1) - 7)
                    {
                        if (i < 7)
                        {
                            inqrcode[i, j] = 3;
                        }
                    }

                    //horizontale blanc

                    if (i == 1 || i == 5)
                    {
                        if ((j < 6 || j > inqrcode.GetLength(1) - 1 - 6) && j > 0)
                        {
                            if (j < inqrcode.GetLength(1) - 1)
                            {
                                inqrcode[i, j] = 2;
                            }
                        }
                    }
                    if (i == inqrcode.GetLength(0) - 1 - 1 || i == inqrcode.GetLength(0) - 6)
                    {
                        if (j < 6 && j > 0)
                        {
                            inqrcode[i, j] = 2;
                        }
                    }


                    //verticale blanc
                    if (j == 1 || j == 5)
                    {
                        if ((i < 6 || i > inqrcode.GetLength(0) - 1 - 6) && i > 0)
                        {
                            if (i < inqrcode.GetLength(0) - 1)
                            {
                                inqrcode[i, j] = 2;
                            }
                        }
                    }
                    if (j == inqrcode.GetLength(1) - 1 - 1 || j == inqrcode.GetLength(1) - 6)
                    {
                        if (i < 6 && i > 0)
                        {
                            inqrcode[i, j] = 2;
                        }
                    }

                    //carrés noirs du haut
                    if ((i == 2 || i == 3 || i == 4) && (j == 2 || j == 3 || j == 4 || j == temp || j == temp + 1 || j == temp + 2))
                    {
                        inqrcode[i, j] = 3;
                    }


                    //carré noir du bas

                    if ((i == temp || i == temp + 1 || i == temp + 2) && (j == 2 || j == 3 || j == 4))
                    {
                        inqrcode[i, j] = 3;
                    }
                    #endregion

                    /*  Placement du séparateur vertical  */
                    if ((j == 7 && (i < 8 || i > inqrcode.GetLength(0) - 8)) || (j == inqrcode.GetLength(1) - 8 && i < 8))
                    {
                        inqrcode[i, j] = 2;
                    }

                    /*  Placement du séparateur horizontal  */
                    if ((i == 7 && (j < 8 || j > inqrcode.GetLength(1) - 8)) || (i == inqrcode.GetLength(0) - 8 && j < 8))
                    {
                        inqrcode[i, j] = 2;
                    }

                    /*  Placement des modèles d'alignement  */
                    if (v == 2)
                    {
                        if ((i == 6 && j == 6) || (i == 6 & j == 18) || (i == 18 && j == 6) || (i == 18 && j == 18))
                        {
                            if (inqrcode[i, j] != 1)
                            {
                                for (int i_align = i - 2; i_align < i + 3; i_align++)
                                {
                                    for (int j_align = j - 2; j_align < j + 3; j_align++)
                                    {
                                        if (j_align == j - 2 || i_align == i - 2 || i_align == i + 2 || j_align == j + 2 || (i_align == i && j_align == j))
                                        {
                                            inqrcode[i_align, j_align] = 3;
                                        }
                                        else
                                        {
                                            inqrcode[i_align, j_align] = 2;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    /*  Placement des modèles de synchronisation    */
                    if (i == 6 && j == 6)
                    {
                        for (int sync = 8; sync < inqrcode.GetLength(1) - 7; sync++)
                        {
                            if (sync % 2 == 0)
                            {
                                inqrcode[sync, 6] = 3;//noir
                                inqrcode[6, sync] = 3;//noir
                            }
                            else if (sync % 2 != 0)
                            {
                                inqrcode[sync, 6] = 2; //blanc 
                                inqrcode[6, sync] = 2; //blanc 
                            }
                        }
                    }

                    /*  Placement du module sombre  */
                    if (i == (4 * v) + 9 && j == 8)
                    {
                        inqrcode[i, j] = 3;
                    }

                    /* Réservation de la zone pour mettre nos infos de format...  */
                    if ((i == 8 && (j < 9 || j > temp - 4)) || (j == 8 && (i < 9 || i > temp - 4)))
                    {
                        if (inqrcode[i, j] != 3)
                        {
                            inqrcode[i, j] = 4;
                        }
                    }
                }
            }

            #endregion


            #region Placement des données dans QRCode
            /*  Placement des bits de données et des bits de correction d'erreur dans le QRCode
             *  https://www.thonky.com/qr-code-tutorial/module-placement-matrix
             */
            bool gauche = false; // Pour nous aider à passer de droite à gauche dans l'écriture des données
            bool monte = true; // Permet de nous aider à savoir quand ça descend ou quand ça monte
            int x = inqrcode.GetLength(0) - 1; //Position de la ligne où se trouve notre bit à écrire
            int y = inqrcode.GetLength(1) - 1; //Position de la colonne où se trouve notre bit à écrire

            for (int i = 0; i < vraitot.Length; i++)
            {
                // Ecriture lorsque ça monte
                if (monte == true)
                {
                    if (gauche == false)
                    {
                        //Placement du bit dans notre QRCode à la position x y
                        if (inqrcode[x, y] == 0)
                        {
                            if (vraitot[i] == '1') inqrcode[x, y] = 1;
                            else inqrcode[x, y] = 0;
                            gauche = true;
                            y--; // on passe à gauche
                            continue; // bit posé donc on retourne dans la boucle for
                        }
                        else
                        {
                            /*On parcourt en zigzag jusqu'à trouver une case vide
                             *Si on sort de la matrice, on tourne à gauche et on commence à descendre
                             */
                            int ind = 1;
                            while (inqrcode[x, y] != 0)
                            {
                                if (ind % 2 != 0)
                                {
                                    //cherche à gauche si case vide
                                    y--;
                                    ind++;
                                }
                                else
                                {
                                    //cherche à droite une case au dessus si c'est une case vide
                                    y++;
                                    x--;
                                    ind++;
                                }
                                if (x < 0) //si on dépasse en haut de la matrice, on tourne à gauche pour commencer à descendre
                                {
                                    x = 0;
                                    y -= 2;
                                    i--;
                                    monte = false;
                                    break;
                                }
                            }
                            if (monte != false && inqrcode[x, y] == 0) // si après avoir chercher une case vide, on en trouve une bah on met le bit de donnée
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = true;
                                y--;
                                continue;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        //Même chose mais cette fois on est à droite
                        if (inqrcode[x, y] == 0)
                        {
                            if (x == 0)
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = false;
                                y--;
                                monte = false;
                                continue;
                            }
                            if (vraitot[i] == '1') inqrcode[x, y] = 1;
                            else inqrcode[x, y] = 0;
                            gauche = false;
                            y++;
                            x--;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0)
                            {
                                if (ind % 2 != 0)
                                {
                                    x--;
                                    y++;
                                    ind++;
                                }
                                else
                                {
                                    y--;
                                    ind++;
                                }
                            }
                            if (monte != false && inqrcode[x, y] == 0)
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = false;
                                y++;
                                x--;
                                continue;
                            }
                            continue;
                        }
                    }
                }
                // Ecriture lorsque ça descend puis même chose qu'en haut avec seulement les indices qui changent
                else
                {
                    if (y == 0 && i == inqrcode.GetLength(0) - 9)
                    {
                        break;
                    }
                    if (gauche == false)
                    {
                        if (inqrcode[x, y] == 0)
                        {
                            if (vraitot[i] == '1') inqrcode[x, y] = 1;
                            else inqrcode[x, y] = 0;
                            gauche = true;
                            y--;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0)
                            {
                                if (ind % 2 != 0)
                                {
                                    y--;
                                    ind++;
                                }
                                else
                                {
                                    y++;
                                    x++;
                                    ind++;
                                }
                                if (x > inqrcode.GetLength(0) - 1)
                                {
                                    x = inqrcode.GetLength(0) - 1;
                                    y -= 2;
                                    i--;
                                    monte = true;
                                    break;
                                }
                            }
                            if (monte != true && inqrcode[x, y] == 0)
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = true;
                                y--;
                                continue;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        if (inqrcode[x, y] == 0)
                        {
                            if (x == inqrcode.GetLength(0) - 1)
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = false;
                                y--;
                                monte = true;
                                continue;
                            }
                            if (vraitot[i] == '1') inqrcode[x, y] = 1;
                            else inqrcode[x, y] = 0;
                            gauche = false;
                            y++;
                            x++;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0)
                            {
                                if (ind % 2 != 0)
                                {
                                    x++;
                                    y++;
                                    ind++;
                                }
                                else
                                {
                                    y--;
                                    ind++;
                                }
                            }
                            if (monte != false && inqrcode[x, y] == 0)
                            {
                                if (vraitot[i] == '1') inqrcode[x, y] = 1;
                                else inqrcode[x, y] = 0;
                                gauche = false;
                                y++;
                                x++;
                                continue;
                            }
                            continue;
                        }
                    }
                }
            }

            #endregion

            #region Masque de type 0
            /*  Application du masque avec la formule donnée   
             *  https://www.thonky.com/qr-code-tutorial/mask-patterns 
             */
            for (int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for (int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    if (inqrcode[i, j] == 1 || inqrcode[i, j] == 0)
                    {
                        if ((i + j) % 2 == 0)
                        {
                            if (inqrcode[i, j] == 1)
                            {
                                inqrcode[i, j] = 0;
                            }
                            else
                            {
                                inqrcode[i, j] = 1;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Placement des bits d'infos sur le format et la version
            /*  Placement des nombres binaires dans les cases imposées  */
            string info = "111011111000100";
            int lim = inqrcode.GetLength(0) - 1;
            int compt = 0;
            for (int i = 0; i < 15; i++)
            {
                /*  De la case 0 à 8 horizontale en haut à gauche   */
                if (compt < 9)
                {
                    if (inqrcode[8, compt] == 4)
                    {
                        if (info[i] == '1')
                        {
                            inqrcode[8, compt] = 1;
                        }
                        else
                        {
                            inqrcode[8, compt] = 0;
                        }
                    }
                    else i--;
                }
                else
                {
                    /*  Case 7 à 0 verticale en haut à gauche   */
                    if (inqrcode[16 - compt, 8] == 4)
                    {
                        if (info[i] == '1')
                        {
                            inqrcode[16 - compt, 8] = 1;
                        }
                        else
                        {
                            inqrcode[16 - compt, 8] = 0;
                        }
                    }
                    else i--;
                }
                /*  Case 0 à 6 en bas à gauche   */
                if (compt < 7)
                {
                    if (inqrcode[lim - compt, 8] == 4)
                    {
                        if (info[i] == '1')
                        {
                            inqrcode[lim - compt, 8] = 1;
                        }
                        else
                        {
                            inqrcode[lim - compt, 8] = 0;
                        }
                    }
                }
                else
                {
                    /*  Case lim - 7 à lim en haut à droite
                     *  lim correspond aux cases de la dernière colonne
                     */
                    if (compt < 15)
                    {
                        if (inqrcode[8, lim - 14 + compt] == 4)
                        {
                            if (info[i] == '1')
                            {
                                inqrcode[8, lim - 14 + compt] = 1;
                            }
                            else
                            {
                                inqrcode[8, lim - 14 + compt] = 0;
                            }
                        }
                    }
                }
                compt++;
            }
            #endregion

            /*
            //Visualisation de mon tableau de int
            Console.WriteLine("\n\n\n");
            for(int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for(int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    Console.Write(inqrcode[i, j]);
                }
                Console.WriteLine();
            }*/


            #region QRCode en image avec couleur
            /*  Convertir en Pixel dont la couleur dépend selon le bit dans notre tableau de int  */
            for (int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for (int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    if (inqrcode[i, j] == 0 || inqrcode[i, j] == 2)
                    {
                        qrcode.RGB_matrice[i, j] = new Pixel(255, 255, 255);
                    }
                    if (inqrcode[i, j] == 1 || inqrcode[i, j] == 3)
                    {
                        qrcode.RGB_matrice[i, j] = new Pixel(0, 0, 0);
                    }
                    if (inqrcode[i, j] == 4)
                    {
                        qrcode.RGB_matrice[i, j] = new Pixel(255, 0, 0);
                    }
                }
            }
            #endregion

            qrcode.Agrandir(100);

        }


        /// <summary>
        /// Fonction permettant de convertir les chiffres, lettres et quelques caractères spéciaux en valeur numérique (alphanumérique)
        /// </summary>
        /// <param name="txt"> Texte entrée par l'utilisateur à convertir en alphanumérique pour avoir le qrcode </param>
        /// <returns>
        /// Retourne un tableau de nombres décimaux correspondant aux caractères de notre texte
        /// </returns>
        public int[] Encodage(string txt)
        {
            char[] carac = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                ' ', '$', '%', '*', '+', '-', '.', '/', ':' };
            int[] result = new int[txt.Length];

            for (int i = 0; i < txt.Length; i++)
            {
                for (int j = 0; j < carac.Length; j++)
                {
                    if (carac[j] == txt[i])
                    {
                        result[i] = j;
                        break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Fonction permettant de convertir un nombre décimal en binaire avec une taille pour la chaîne binaire donnée
        /// </summary>
        /// <param name="n"> Nombre décimal à convertir en binaire </param>
        /// <param name="taille"> Longueur de la chaîne binaire souhaité </param>
        /// <returns>
        /// Retourne le résultat de la conversion en un tableau de int
        /// </returns>
        public int[] ConvertToByte(int n, int taille)
        {
            int[] resul = new int[taille];
            int reste;
            int i = taille - 1;
            do
            {
                if (n == 0)
                {
                    resul[i] = 0;
                }
                reste = n % 2;
                resul[i] = reste;
                n = (n - reste) / 2;
                i--;
            }
            while (n != 0 && i >= 0);
            return resul;
        }

        /// <summary>
        /// Fonction permettant de générer les mots de code de correction pour les deux versions 1 et 2
        /// On utilise la correction d'erreur Reed-Solomon imposé
        /// </summary>
        /// <param name="txt"> Notre texte à convertir en qrcode, on l'utilise seulement pour connaitre la version de notre correction </param>
        /// <param name="bytes"> La chaîne de bits générée en amont à partir de notre donnée encodée avec le mode, la taille en binaire, 
        /// la terminaison, les 0 pour faire un multiple de 8 et les octets équivalent à 236 et 17 pour remplir la capacité maximale de bits du QRCode </param>
        /// <returns>
        /// Retourne la chaîne de bits (sous la forme d'un string) de la correction
        /// </returns>
        public string Correction(string txt, byte[] bytes)
        {
            //Encoding u8 = Encoding.UTF8;
            //byte[] bytestxt = u8.GetBytes(txt);
            string resul = "";
            if (txt.Length <= 25) // Version 1
            {
                byte[] result = ReedSolomonAlgorithm.Encode(bytes, 7, ErrorCorrectionCodeType.QRCode);
                foreach (byte val in result)
                {
                    int[] res1 = ConvertToByte(Convert.ToInt32(val), 8);
                    for (int i = 0; i < res1.Length; i++)
                    {
                        //Console.Write(res1[i]);
                        resul += Convert.ToString(res1[i]);
                    }
                    //Console.Write(" ");
                }
                //Console.WriteLine();
            }
            else
            {
                if (txt.Length <= 47) // Version 2
                {
                    byte[] result = ReedSolomonAlgorithm.Encode(bytes, 10, ErrorCorrectionCodeType.QRCode);
                    foreach (byte val in result)
                    {
                        int[] res2 = ConvertToByte(Convert.ToInt32(val), 8);
                        for (int j = 0; j < res2.Length; j++)
                        {
                            //Console.Write(res2[j]);
                            resul += Convert.ToString(res2[j]);
                        }
                        //Console.Write(" ");
                    }
                    //Console.WriteLine();
                }
            }
            return resul;

        }

        /// <summary>
        /// Fonction permettant de convertir le texte en binaire après avoir récupérer les valeurs numériques en alphanumérique
        /// de notre texte
        /// Applicable pour les deux versions et return un string car notre chaîne de donnée final sera un string
        /// Pour chaque "binôme de caractères" ou "caractère solo" (si nombre de caractères impairs) on applique la formule donnée
        /// </summary>
        /// <param name="txt">Texte entré par l'utilisateur pour être converti en qrcode</param>
        /// <returns> 
        /// retourne la chaîne de caractère en entrèe convertie en binaire sous la forme de string  
        /// </returns>
        public string CharToByte(string txt)
        {
            int[] a = Encodage(txt);
            int res;
            string result = "";
            for (int i = 0; i < a.Length; i = i + 2)
            {
                if ((i + 1) < a.Length)
                {
                    res = 45 * a[i] + a[i + 1];
                    int[] resul = ConvertToByte(res, 11);
                    for (int j = 0; j < resul.Length; j++)
                    {
                        //Console.Write(resul[j]);
                        result += Convert.ToString(resul[j]);
                    }
                }
                else
                {
                    res = a[i];
                    int[] resul = ConvertToByte(res, 6);
                    for (int k = 0; k < resul.Length; k++)
                    {
                        //Console.Write(resul[k]);
                        result += Convert.ToString(resul[k]);
                    }
                }
            }
            //Console.WriteLine("\n" + result);
            return result;
        }


        /// <summary>
        /// Fonction qui convertit en binaire la longueur de notre chaîne de caractère (grâce à notre fonction ConverTtoByte)
        /// </summary>
        /// <param name="txt"> Toujours notre chaîne de caractère à convertir en QRCode </param>
        /// <returns>
        /// Un tableau de int de notre chaîne en binaire 
        /// </returns>
        public int[] TailleEnByte(string txt)
        {
            int[] resul = ConvertToByte(txt.Length, 9);
            /*for (int i = 0; i < resul.Length; i++)
            {
                Console.Write(resul[i]);
            }*/
            return resul;
        }



        /// <summary>
        /// Fonction qui ajoute le mode, l'indicateur du nombre de caractères
        /// </summary>
        /// <param name="mode"> Chaîne binaire sous la forme de string pour le mode </param>
        /// <param name="taille"> Chaîne binaire sous la forme de string pour l'indicateur de taille </param>
        /// <param name="données"> Chaîne binaire sous la forme de string pour les données  </param>
        /// <returns>
        /// Retourne la chaîne en string des données binaires quasi total avant la terminaison
        /// </returns>
        public string TotalBits(string mode, string taille, string données)
        {
            return mode + taille + données;
        }


        /// <summary>
        /// Fonction permettant de savoir si oui ou non on applique la terminaison à notre chaîne en string des données binaires
        /// </summary>
        /// <param name="bytes"> Chaîne en string des données binaires récupérée après la fonction TotalBits </param>
        /// <param name="version"> Version 1 ou 2 </param>
        /// <returns> Retourne notre chaîne de binaire avec la terminaison s'il y a besoin (en string) </returns>
        public string Terminaison(string bytes, int version)
        {
            int longtot;
            if (version == 1)
            {
                longtot = 152;
                if (bytes.Length < longtot - 4)
                {
                    bytes += "0000";
                }
                else
                {
                    while (bytes.Length < longtot)
                    {
                        bytes += 0;
                    }
                }
            }
            else if (version == 2)
            {
                longtot = 272;
                if (bytes.Length < longtot - 4)
                {
                    bytes += "0000";
                }
                else
                {
                    while (bytes.Length < longtot)
                    {
                        bytes += 0;
                    }
                }
            }
            return bytes;
        }


        /// <summary>
        /// Fonction qui détermine s'il faut rajouter des 0 en fonction de la multiplicité par 8 de notre chaîne binaire
        /// </summary>
        /// <param name="bytes"> Chaîne binaire avec mode, indicateur de taille, donnée et terminaison </param>
        /// <returns>
        /// Retourne la chaîne binaire avec les 0 nécessaire pour avoir une multiplicité de 8
        /// </returns>
        public string MultipleHuit(string bytes)
        {
            do
            {
                bytes += "0";
            }
            while (bytes.Length % 8 != 0);
            return bytes;
        }






        #region Début LecteurQRCode
        /*  Algorithme pour lecture QRCode :
         *  Image d'un QRCode en entrée
         *  retrecir image à 21 ou 25 suivant la version
         *  enlever masque (fonction inverse)
         *  différencier les données des modèles de fonction (travail avec les indices)
         *  seul les cases pour écrire les données seront des 1 ou des 0 le reste on met 4 (chiffre aléatoire)
         *  prendre chaine de données à partir d'en bas à droite
         *  lire mode et indicateur de taille
         *  taille to int
         *  taille pair ==> taille / 2 *11  (nombre de bits pour le mot)
         *  taille impair ==>  taille-1/2 *11 + 6  
         *  couper les données totales jusqu'à nombre de bits du mot
         *  enlever mode et indicateur de taille
         *  convertir binaire to int 
         *  int%45 == 2eme lettre du pair
         *  (int - reste)/45 == 1ere lettre
         *  if impair 6 dernier bits to int to lettre
         *  
         */



        /*public string LecteurQRCode(MyImage qrcode)
        {
            int[,] inqrcode = new int[qrcode.Hauteur / 100, qrcode.Largeur / 100];
            string vraitot = "";
            int v = 1;
            if (inqrcode.GetLength(0) > 21) v = 2;
 
            for (int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for (int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    if (qrcode.RGB_matrice[i, j] == new Pixel(255, 255, 255))
                    {
                        if (v == 2)
                        {
                            if (i >= 16 && j >= 16 && i <= 20 && j <= 20)
                            {
                                inqrcode[i, j] = 4;
                            }
 
                        }
                        if ((i<=8 && j<=8) || (i>=inqrcode.GetLength(0)-8 && j <= 8) || j== 6 || i==6 || (j>= inqrcode.GetLength(1) - 8 && i<=8))
                        {
                            inqrcode[i, j] = 4;                            
                        }
                        else
                        {
                            inqrcode[i, j] = 0;
                        }
                    }
                    if (qrcode.RGB_matrice[i, j] == new Pixel(0, 0, 0))
                    {
                        if (i == (4 * v) + 9 && j == 8)
                        {
                            inqrcode[i, j] = 4;
                        }
                        if (v == 2)
                        {
                            if (i >= 16 && j >= 16 && i <= 20 && j <= 20)
                            {
                                inqrcode[i, j] = 4;
                            }
 
                        }
                        if ((i <= 8 && j <= 8) || (i >= inqrcode.GetLength(0) - 8 && j <= 8) || j == 6 || i == 6 || (j >= inqrcode.GetLength(1) - 8 && i <= 8))
                        {
                            inqrcode[i, j] = 4;
                        }
                        else
                        {
                            inqrcode[i, j] = 1;
                        }
                    }
                    else
                    {
                        inqrcode[i, j] = 4;
                    }
                }
            }
 
            bool gauche = false; // Pour nous aider à passer de droite à gauche dans l'écriture des données
            bool monte = true; // Permet de nous aider à savoir quand ça descend ou quand ça monte
            int x = inqrcode.GetLength(0) - 1; //Position de la ligne où se trouve notre bit à écrire
            int y = inqrcode.GetLength(1) - 1; //Position de la colonne où se trouve notre bit à écrire
 
            int longueur = 152;
            if (v == 2) longueur = 272;
            for (int i = 0; i < longueur; i++)
            {
                // Ecriture lorsque ça monte
                if (monte == true)
                {
                    if (gauche == false)
                    {
                        //Placement du bit dans notre QRCode à la position x y
                        if (inqrcode[x, y] == 0 || inqrcode[x,y] ==1)
                        {
                            if (inqrcode[x, y] == 1) vraitot += '1' ;
                            else vraitot += '0';
                            gauche = true;
                            y--; // on passe à gauche
                            continue; // bit posé donc on retourne dans la boucle for
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0 || inqrcode[x, y] != 1)
                            {
                                if (ind % 2 != 0)
                                {
                                    //cherche à gauche si case vide
                                    y--;
                                    ind++;
                                }
                                else
                                {
                                    //cherche à droite une case au dessus si c'est une case vide
                                    y++;
                                    x--;
                                    ind++;
                                }
                                if (x < 0) //si on dépasse en haut de la matrice, on tourne à gauche pour commencer à descendre
                                {
                                    x = 0;
                                    y -= 2;
                                    i--;
                                    monte = false;
                                    break;
                                }
                            }
                            if (monte != false && (inqrcode[x, y] == 0 || inqrcode[x, y] == 1)) // si après avoir chercher une case vide, on en trouve une bah on met le bit de donnée
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = true;
                                y--;
                                continue;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        //Même chose mais cette fois on est à droite
                        if (inqrcode[x, y] == 0 || inqrcode[x, y] == 1)
                        {
                            if (x == 0)
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = false;
                                y--;
                                monte = false;
                                continue;
                            }
                            if (inqrcode[x, y] == 1) vraitot += '1';
                            else vraitot += '0';
                            gauche = false;
                            y++;
                            x--;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0 || inqrcode[x, y] != 1)
                            {
                                if (ind % 2 != 0)
                                {
                                    x--;
                                    y++;
                                    ind++;
                                }
                                else
                                {
                                    y--;
                                    ind++;
                                }
                            }
                            if (monte != false && (inqrcode[x, y] == 0 || inqrcode[x, y] == 1))
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = false;
                                y++;
                                x--;
                                continue;
                            }
                            continue;
                        }
                    }
                }
                // Ecriture lorsque ça descend puis même chose qu'en haut avec seulement les indices qui changent
                else
                {
                    if (y == 0 && i == inqrcode.GetLength(0) - 9)
                    {
                        break;
                    }
                    if (gauche == false)
                    {
                        if (inqrcode[x, y] == 0 || inqrcode[x, y] == 1)
                        {
                            if (inqrcode[x, y] == 1) vraitot += '1';
                            else vraitot += '0';
                            gauche = true;
                            y--;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0 || inqrcode[x, y] != 1)
                            {
                                if (ind % 2 != 0)
                                {
                                    y--;
                                    ind++;
                                }
                                else
                                {
                                    y++;
                                    x++;
                                    ind++;
                                }
                                if (x > inqrcode.GetLength(0) - 1)
                                {
                                    x = inqrcode.GetLength(0) - 1;
                                    y -= 2;
                                    i--;
                                    monte = true;
                                    break;
                                }
                            }
                            if (monte != true && (inqrcode[x, y] == 0 || inqrcode[x, y] == 1))
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = true;
                                y--;
                                continue;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        if (inqrcode[x, y] == 0 || inqrcode[x, y] == 1)
                        {
                            if (x == inqrcode.GetLength(0) - 1)
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = false;
                                y--;
                                monte = true;
                                continue;
                            }
                            if (inqrcode[x, y] == 1) vraitot += '1';
                            else vraitot += '0';
                            gauche = false;
                            y++;
                            x++;
                            continue;
                        }
                        else
                        {
                            int ind = 1;
                            while (inqrcode[x, y] != 0 || inqrcode[x, y] != 1)
                            {
                                if (ind % 2 != 0)
                                {
                                    x++;
                                    y++;
                                    ind++;
                                }
                                else
                                {
                                    y--;
                                    ind++;
                                }
                            }
                            if (monte != false && (inqrcode[x, y] == 0 || inqrcode[x, y] == 1))
                            {
                                if (inqrcode[x, y] == 1) vraitot += '1';
                                else vraitot += '0';
                                gauche = false;
                                y++;
                                x++;
                                continue;
                            }
                            continue;
                        }
                    }
                }
            }
 
 
            return vraitot;
        }*/
        #endregion

    }
}