using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable2
{
    public class Noeud<T>
    {
        Station station;
        int[] lien;
        double[] poids;

        /// <summary>
        /// Initialise un nœud à partir d'une matrice d'incidence et d'un identifiant de station se trouvant 
        /// dans aussi dans le fichier de stations
        /// </summary>
        /// <param name="id">Identifiant de la station dans la matrice</param>
        /// <param name="matrice">Matrice d'incidence représentant les connexions</param>
        /// <param name="stations">Chemin vers le fichier contenant les stations</param>
        public Noeud(int id, double[,] matrice, string stations)
        {
            this.station = new Station(id, stations);   
            int k = 0;
            for (int i = 0; i< matrice.GetLength(1); i++)
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
