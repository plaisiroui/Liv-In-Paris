using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable2
{
    public class Noeud<T>
    {
        ///Pour chaque noeud, on lui associe sa station ainsi que ses lien avec les autres stations et le poids des liens
        Station station;
        int[] lien;
        double[] poids;

        public Noeud(int id, double[,] matrice, string stations)
        {
            this.station = new Station(id, stations);   ///On affecte la station à noeud grâce au document listant toutes les stations
            int k = 0;
            for (int i = 0; i< matrice.GetLength(1); i++)   ///On répertorie tout les identifiant de stations qui ont des liens avec la station
            {
                if (matrice[id - 1, i] != 0) { k++; }
            }
            int[] list = new int[k];
            double[] poids = new double[k];
            k = 0;
            for (int i = 0; i < matrice.GetLength(1); i++)
            {
                if (matrice[id-1,i] != 0)
                {
                    list[k] = i+1;
                    poids[k] = matrice[id - 1, i];
                    k++;
                } 
            }
            this.lien = list;
            this.poids = poids;
        }
        public Noeud(Station station, int[] lien)
        {
            this.station = station;
            this.lien = lien;
        }
        public Station Station
        {
            get { return station; }
        }
        public int[] Lien
        {
            get { return lien; }
        }
        public double[] Poids
        {
            get { return poids; }
        }
    }
}
