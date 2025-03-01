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
            /*bool[,] matrice_adjacence = MatriceAdjacence(fichier_matrice);
            int[][] liste_adjacence = ListeAdjacence(matrice_adjacence);
            for (int i = 0; i< matrice_adjacence.GetLength(0); i++)
            {
                for (int j = 0; j < matrice_adjacence.GetLength(1); j++)
                {
                    if (matrice_adjacence[i, j])
                    {
                        Console.Write("X ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    
                }
                Console.WriteLine();
            }
            for (int i = 0;i<liste_adjacence.Length; i++)
            {
                for (int j = 0; j<liste_adjacence[i].Length;j++)
                {
                    Console.Write(liste_adjacence[i][j] + " ");
                }
                Console.WriteLine();
            }*/
        }

    }
}