using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Projet_info_WPF
{
    class QRCode
    {
        public void QRcode(string txt)
        {
            txt = txt.ToUpper();
            int v = 1;
            if (txt.Length > 25)
            {
                v = 2;
            }
            MyImage qrcode = new MyImage(17 + (v * 4), 17 + (v * 4));

            int[] t = TailleEnByte(txt);
            string teb = "";
            for (int i = 0; i < t.Length; i++)
            {
                teb += Convert.ToString(t[i]);
            }

            string mode = "0010";


            //Console.WriteLine("\n\n\nAnalyse et Encodage des données en binaire : \n\n");
            string ctb = CharToByte(txt);

            //Console.WriteLine("\n\nTotal de bits sans rajout de 0 : \n\n");
            string tot = TotalBits(mode, teb, ctb);
            //Console.WriteLine(tot);
            string tot_with_term = Terminaison(tot, v);

            //Console.WriteLine("\n" + tot_with_term);
            string vraitot = MultipleHuit(tot_with_term);
            //Console.WriteLine("\n" + vraitot);

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

            //Console.WriteLine(vraitot+"\n\n\n");


            byte[] bytestxt = new byte[vraitot.Length / 8];
            for (int i = 0; i < bytestxt.Length; i++)
            {
                bytestxt[i] = Convert.ToByte(vraitot.Substring(i * 8, 8), 2);
            }

            string cor = Correction(txt, bytestxt);
            //Console.WriteLine("\n\nCodage de correction d'erreur : \n\n");
            //Console.WriteLine(cor);
            vraitot += cor;

            //Console.WriteLine("\n" + vraitot);
            int index = 1;
            for (int i = 0; i < vraitot.Length; i = i + 8)
            {
                Console.Write(vraitot.Substring(i, 8) + " ");
                if (index == 9)
                {
                    Console.WriteLine();
                    index = 0;
                }
                index++;
            }



            //finder patterns + separators + timing patterns + dark module + alignment pattern
            int[,] inqrcode = new int[21 + ((v - 1) * 4), 21 + ((v - 1) * 4)];
            int temp = inqrcode.GetLength(0) - 5;
            //int[,] align = { { 6, 6 }, { 6, 18 }, { 18, 6 }, { 18, 18 } };
            for (int i = 0; i < inqrcode.GetLength(0); i++)
            {
                for (int j = 0; j < inqrcode.GetLength(1); j++)
                {
                    //inqrcode[i, j] = 4;
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


                    //séparateurs verticals
                    if ((j == 7 && (i < 8 || i > inqrcode.GetLength(0) - 8)) || (j == inqrcode.GetLength(1) - 8 && i < 8))
                    {
                        inqrcode[i, j] = 2;
                    }

                    //séparateurs horizontals
                    if ((i == 7 && (j < 8 || j > inqrcode.GetLength(1) - 8)) || (i == inqrcode.GetLength(0) - 8 && j < 8))
                    {
                        inqrcode[i, j] = 2;
                    }

                    //modèles d'alignement
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

                    //modèles de synchronisation
                    if (i == 6 && j == 6)
                    {
                        for (int sync = 8; sync < inqrcode.GetLength(1) - 7; sync++)
                        {
                            if (sync % 2 == 0)
                            {
                                inqrcode[sync, 6] = 3;
                                inqrcode[6, sync] = 3;
                            }
                            else if (sync % 2 != 0)
                            {
                                inqrcode[sync, 6] = 2; //blanc 
                                inqrcode[6, sync] = 2; //blanc 
                            }
                        }
                    }

                    //dark module
                    if (i == (4 * v) + 9 && j == 8)
                    {
                        inqrcode[i, j] = 3;
                    }

                    //zone d'info
                    if ((i == 8 && (j < 9 || j > temp - 4)) || (j == 8 && (i < 9 || i > temp - 4)))
                    {
                        if (inqrcode[i, j] != 3)
                        {
                            inqrcode[i, j] = 4;
                        }
                    }
                }
            }



            //Conversion de notre tableau en byte
            /*byte[,] myqr = new byte[inqrcode.GetLength(0), inqrcode.GetLength(1)];
            for(int i = 0; i < myqr.GetLength(0); i++)
            {
                for(int j = 0; j < myqr.GetLength(1); j++)
                {
                    myqr[i, j] = (byte)inqrcode[i, j];
                }
            }*/



            //data in qrcode
            bool gauche = false;
            bool monte = true;
            int x = inqrcode.GetLength(0) - 1;
            int y = inqrcode.GetLength(1) - 1;

            for (int i = 0; i < vraitot.Length; i++)
            {
                if (monte == true)
                {
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
                                    x--;
                                    ind++;
                                }
                                if (x < 0)
                                {
                                    x = 0;
                                    y -= 2;
                                    i--;
                                    monte = false;
                                    break;
                                }
                            }
                            if (monte != false && inqrcode[x, y] == 0)
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
                                /*if (x < 0)
                                {
                                    x = 0;
                                    y -= 2;
                                    i--;
                                    monte = false;
                                    break;
                                }*/
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
                                /*if (x < 0)
                                {
                                    x = 0;
                                    y -= 2;
                                    i--;
                                    monte = false;
                                    break;
                                }*/
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


            //masque n°0
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



            //format
            string info = "111011111000100";
            int lim = inqrcode.GetLength(0) - 1;
            int compt = 0;
            for (int i = 0; i < 15; i++)
            {
                //0 à 7 horizontal en haut à gauche
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



            //Convertir en Pixel 
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
            qrcode.Agrandir(100);

        }

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


        public string Correction(string txt, byte[] bytes)
        {
            Encoding u8 = Encoding.UTF8;
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


        public int[] TailleEnByte(string txt)
        {
            int[] resul = ConvertToByte(txt.Length, 9);
            /*for (int i = 0; i < resul.Length; i++)
            {
                Console.Write(resul[i]);
            }*/
            return resul;
        }


        public string TotalBits(string mode, string taille, string données)
        {
            //string tot = "";
            /*for(int i = 0; i < tot.Length; i++)
            {
                if (i < mode.Length)
                {
                    tot += mode[i];
                }
                if(i>= mode.Length && i < taille.Length)
                {
                    tot += taille[i - mode.Length];
                }
                if(i>=taille.Length && i < données.Length)
                {
                    tot += données[i - taille.Length];
                }
                if(i>=données.Length && i< correction.Length)
                {
                    tot += correction[i - données.Length];
                }
            }*/

            return mode + taille + données;
        }


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


        public string MultipleHuit(string bytes)
        {
            do
            {
                bytes += "0";
            }
            while (bytes.Length % 8 != 0);
            return bytes;
        }





    }
}