using Karaté;
using System;
using System.Net.Http.Headers;

namespace karate
{
    class Program
    {

        public static void Main(string[] args)
        {
            string fichier_matrice = "soc-karate.mtx";
            Graphe g1 = new Graphe(fichier_matrice);
        }

    }
}