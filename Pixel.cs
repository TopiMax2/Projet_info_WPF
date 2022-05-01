using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_info_WPF
{
    class Pixel
    {
        private byte Rouge;
        private byte Vert;
        private byte Bleu;

        public Pixel(byte[] a)
        {
            this.Rouge = a[0];
            this.Vert = a[1];
            this.Bleu = a[2];
        }

        public byte[] Pixel_en_bytes()
        {
            byte byte_1 = Rouge;
            byte byte_2 = Vert;
            byte byte_3 = Bleu;
            byte[] retour = new byte[3] { byte_1, byte_2, byte_3 };
            return retour;
        }

        public byte R
        {
            get { return Rouge; }
            set { Rouge = value; }
        }

        public byte G
        {
            get { return Vert; }
            set { Vert = value; }
        }

        public byte B
        {
            get { return Bleu; }
            set { Bleu = value; }
        }

        public string Affichage_byte()
        {
            byte[] a = new byte[3];
            a[0] = this.Rouge;
            a[1] = this.Vert;
            a[2] = this.Bleu;
            string str = a[0] + " " + a[1] + " " + a[2] + " ";

            return str;
        }
    }
}
