using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaté
{
    internal class Noeud
    {
        //Pour chaque noeud, on lui associe son numéro de sommet ainsi que ses lien avec les autres sommets
        int numero;
        int[] lien;

        public Noeud(int numero, int[][] liste) 
        {
            this.numero = numero+1; //Ici, on ajoute +1 juste pour la forme. Cela évitera d'avoir un sommet 0 mais un sommet 1
            int[] lien = new int[liste[numero].Length];
            for (int i = 0; i< liste[numero].Length; i++)
            {
                lien[i] = liste[numero][i];
            }
            this.lien = lien;
        }
        public Noeud(int numero, int[] lien)
        {
            this.numero = numero + 1;
            this.lien = lien;
        }
        public int Numero
        {
            get { return numero; }
        }
        public int[] Lien
        {
            get { return lien; }
        }
    }
}
