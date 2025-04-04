using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace Livrable2
{
    class Program
    {

        public static void Main(string[] args)
        {
            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> g1 = new Graphe<int>(fichier_connexion,fichier_StationMetro);
            List<int> chemin = g1.Dijkstra(50, 233);
            List<int> chemin2 = g1.BellmanFord(50, 233);
            (double[,] distance, int[,] next) = g1.FloydWarshall();
            List<int> chemin3 = g1.ReconstituerChemin(50, 233,next);

            for (int i = 0; i < chemin.Count; i++)
            {
                Console.Write(chemin[i]+" ");
            }
            Console.WriteLine(g1.CalculerTempsChemin(chemin));
            for (int i = 0; i < chemin3.Count; i++)
            {
                Console.Write(chemin2[i] + " ");
            }
            Console.WriteLine(g1.CalculerTempsChemin(chemin2));
            for (int i = 0; i < chemin3.Count; i++)
            {
                Console.Write(chemin3[i] + " ");
            }
            Console.WriteLine(g1.CalculerTempsChemin(chemin3));

            g1.VisualiserGrapheAvecChemin(chemin3);

        }



    }
}
