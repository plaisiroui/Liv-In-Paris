using System;
using System.Net.Http.Headers;

namespace Livrable2
{
    class Program
    {

        public static void Main(string[] args)
        {
            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> g1 = new Graphe<int>(fichier_connexion,fichier_StationMetro);
            g1.VisualiserGrapheParCoordonnees();
            (List<int> chemin,double temps) = g1.Dijkstra(1, 200);
        }

    }
}
