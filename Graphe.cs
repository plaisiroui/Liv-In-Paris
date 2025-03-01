using SkiaSharp;
using System;
using System.Diagnostics;


namespace Karaté
{
    internal class Graphe
    {
        Noeud[] noeuds;
        public Graphe(string fichier)
        {
            bool[,] matrice = MatriceAdjacence(fichier);
            int[][] liste = ListeAdjacence(fichier);

            Noeud[] noeuds = new Noeud[matrice.GetLength(0)];
            for (int p = 0; p < matrice.GetLength(0); p++)
            {
                noeuds[p] = new Noeud(p, liste);
            }
            /*Noeud[] test = new Noeud[10];
            int[] l0 = { 1, 2, 3 };
            int[] l1 = { 0, 5, 7 };
            int[] l2 = { 0, 8 };
            int[] l3 = { 0, 4, 8 };
            int[] l4 = { 3, 5 };
            int[] l5 = { 1, 4, 6 };
            int[] l6 = { 5, 7 };
            int[] l7 = { 1, 6, 9 };
            int[] l8 = { 2, 3 };
            int[] l9 = { 7 };
            Noeud a = new Noeud(0, l0);
            test[0] = a;
            Noeud b = new Noeud(1, l1);
            test[1] = b;
            Noeud c = new Noeud(2, l2);
            test[2] = c;
            Noeud d = new Noeud(3, l3);
            test[3] = d;
            Noeud e = new Noeud(4, l4);
            test[4] = e;
            Noeud f = new Noeud(5, l5);
            test[5] = f;
            Noeud g = new Noeud(6, l6);
            test[6] = g;
            Noeud h = new Noeud(7, l7);
            test[7] = h;
            Noeud i = new Noeud(8, l8);
            test[8] = i;
            Noeud j = new Noeud(9, l9);
            test[9] = j;
            int[] l = ParcoursLargeur(test, 0);
            int[] m = ParcoursProfondeur(test, 0);
            for (int w = 0; w < l.Length; w++)
            {
                Console.Write(l[w] + " ");
            }
            Console.WriteLine();
            for (int w = 0; w < m.Length; w++)
            {
                Console.Write(m[w] + " ");
            }
            Console.WriteLine(ContientCycle(test));
            Console.WriteLine(EstConnexe(test));*/

            VisualiserGraphe(noeuds);
        }
        static bool ContientCycle(Noeud[] graph)
        {
            List<int> visite = new List<int>(); // Liste des nœuds déjà visités
            List<int> parent = new List<int>(new int[graph.Length]); // Liste des parents pour suivre la relation parent-enfant

            for (int i = 0; i < graph.Length; i++)
            {
                parent[i] = -1;
            }

            List<int> aVisiter = new List<int>(); // Liste utilisée comme pile pour le parcours en profondeur

            foreach (Noeud noeud in graph)
            {
                if (!visite.Contains(noeud.Numero)) // Vérifie si le nœud n'a pas été visité
                {
                    aVisiter.Add(noeud.Numero);

                    while (aVisiter.Count > 0)
                    {
                        int actuel = aVisiter[aVisiter.Count - 1];
                        aVisiter.RemoveAt(aVisiter.Count - 1);

                        if (!visite.Contains(actuel))
                        {
                            visite.Add(actuel);
                        }

                        foreach (int voisin in graph[actuel].Lien)
                        {
                            if (!visite.Contains(voisin)) //si le voisin n'est pas visité, l'ajouter à visiter
                            {
                                aVisiter.Add(voisin);
                                parent[voisin] = actuel;
                            }
                            else if (parent[actuel] != voisin) //si le voisin est déjà visité et n'est pas le parent, cycle détecté
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        static bool EstConnexe(Noeud[] graph)
        {
            bool result = false;    //Si le graphe est connexe, alors le degré d'un graphe doit être égal au nombre de sommets visité par le parcours en largeur
            if (graph.Length == ParcoursLargeur(graph, graph[0].Numero).Length)
            {
                result = true;
            }
            return result;
        }
        static int[] ParcoursProfondeur(Noeud[] graph, int start)
        {
            List<int> resultat = new List<int>(); // Liste pour stocker le parcours des nœuds visités
            List<int> visite = new List<int>(); // Liste pour suivre les nœuds déjà visités
            List<int> pile = new List<int>(); // Liste utilisée comme une pile pour le parcours en profondeur

            pile.Add(start);
            while (pile.Count > 0)
            {
                int noeudActuel = pile[pile.Count - 1];
                pile.RemoveAt(pile.Count - 1);

                if (!visite.Contains(noeudActuel)) // Vérifie si le nœud n'a pas encore été visité
                {
                    visite.Add(noeudActuel); // Marque le nœud comme visité
                    resultat.Add(noeudActuel);

                    // Ajoute les voisins du nœud actuel à la pile pour exploration
                    for (int i = graph[noeudActuel].Lien.Length - 1; i >= 0; i--)
                    {
                        int voisin = graph[noeudActuel].Lien[i];
                        if (!visite.Contains(voisin)) // Ajoute seulement les nœuds non visités
                        {
                            pile.Add(voisin);
                        }
                    }
                }
            }
            return resultat.ToArray();
        }
        static int[] ParcoursLargeur(Noeud[] noeud, int num)
        {
            //On construit la list du parcour en largeur du graphe
            List<int> parcours = new List<int>();
            parcours.Add(num);                              //On ajoute le sommet de départ, 
            for (int i = 0; i < parcours.Count; i++)          //puis on ajoute les sommet adjacents au premier sommet
            {
                for (int j = 0; j < noeud[parcours[i]].Lien.Length; j++)
                {   //Si le sommet est deja présent dans la liste, alors on ne l'ajoute pas dans la liste
                    if (!parcours.Contains(noeud[parcours[i]].Lien[j]))
                    {
                        parcours.Add(noeud[parcours[i]].Lien[j]);
                    }
                }
            }
            return parcours.ToArray();
        }
        static bool[,] MatriceAdjacence(string fichier)
        {
            int[] m = Array.ConvertAll(File.ReadAllText(fichier) // On sépare les nombres
                .Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            bool[,] matrice = new bool[m[0], m[1]];
            for (int i = 0; i < matrice.GetLength(0); i++) // On initialise la matrice comme étant false
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    matrice[i, j] = false;
                }
            }
            for (int i = 3; i < m.Length; i += 2) // On modifie la matrice aux endroits ou il y a une liaison
            {
                matrice[m[i] - 1, m[i + 1] - 1] = true;
                matrice[m[i + 1] - 1, m[i] - 1] = true;
            }
            return matrice;
        }
        static int[][] ListeAdjacence(string fichier)
        {
            string[] lignes = File.ReadAllLines(fichier);
            string[] premiereligne = lignes[0].Split(' ');

            int nombreSommets = int.Parse(premiereligne[0]);
            List<int>[] liste_adjacence = new List<int>[nombreSommets];

            // On initialise
            for (int i = 0; i < nombreSommets; i++)
            {
                liste_adjacence[i] = new List<int>();
            }

            for (int i = 1; i < lignes.Length; i++)
            {
                string[] edge = lignes[i].Split(' ');
                int u = int.Parse(edge[0]) - 1;
                int v = int.Parse(edge[1]) - 1;

                liste_adjacence[u].Add(v + 1); // ici, le graphe est connexe donc on fait la liste des deux cotés
                liste_adjacence[v].Add(u + 1);
            }

            // On convertis en tableau de tableau 
            int[][] listeAdjacence = new int[nombreSommets][];
            for (int i = 0; i < nombreSommets; i++)
            {
                listeAdjacence[i] = liste_adjacence[i].ToArray();
            }

            return listeAdjacence;
        }

        public static void VisualiserGraphe(Noeud[] noeuds)
        {
            // Crée une image SkiaSharp
            int width = 600;
            int height = 600;
            var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            // Efface l'arrière-plan de l'image
            canvas.Clear(SKColors.White);

            // Calculer la taille dynamique des cercles en fonction du nombre de nœuds
            float radius = Math.Max(10, 600 / noeuds.Length); // Rayon des cercles, réduit en fonction du nombre de nœuds
            if (radius > 30) radius = 30; // Limiter la taille maximale des cercles (pour éviter que les cercles ne soient trop grands)

            // Définir la peinture pour les nœuds (cercles bleus)
            SKPaint nodePaint = new SKPaint
            {
                Color = SKColors.Blue,
                IsAntialias = true
            };

            // Définir la peinture pour le texte (numéro des nœuds)
            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.White, // Couleur du texte (blanc)
                TextSize = 20,          // Taille du texte
                IsAntialias = true,     // Anti-aliasing pour des bords lisses
                TextAlign = SKTextAlign.Center, // Centrage du texte
                FakeBoldText = true     // Mettre le texte en gras pour plus de lisibilité
            };

            // Calculer les positions des nœuds sur un cercle autour du centre de l'image
            SKPoint[] positions = new SKPoint[noeuds.Length];
            float angleStep = (float)(2 * Math.PI / noeuds.Length); // Étape de calcul pour espacer les nœuds sur le cercle
            for (int i = 0; i < noeuds.Length; i++)
            {
                // Calculer les positions sur un cercle autour du centre de l'image
                float x = width / 2 + (float)Math.Cos(i * angleStep) * 200;
                float y = height / 2 + (float)Math.Sin(i * angleStep) * 200;
                positions[i] = new SKPoint(x, y);

                // Dessiner les cercles représentant les nœuds
                canvas.DrawCircle(x, y, radius, nodePaint);
            }

            // Dessiner les arêtes entre les nœuds
            SKPaint edgePaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                StrokeWidth = 1 // Rendre les arêtes plus fines
            };

            for (int i = 0; i < noeuds.Length; i++)
            {
                // Parcourir les voisins de chaque nœud pour dessiner les arêtes
                foreach (int voisin in noeuds[i].Lien)
                {
                    // Dessiner une ligne entre le nœud actuel et ses voisins
                    if (i < voisin - 1)
                    {
                        canvas.DrawLine(positions[i].X, positions[i].Y, positions[voisin - 1].X, positions[voisin - 1].Y, edgePaint);
                    }
                }
            }

            // Dessiner les noms des nœuds (numéro des nœuds) au-dessus des arêtes
            for (int i = 0; i < noeuds.Length; i++)
            {
                // Dessiner le numéro du nœud au centre du cercle, après les arêtes
                canvas.DrawText(noeuds[i].Numero.ToString(), positions[i].X, positions[i].Y + 7, textPaint);
            }

            // Sauvegarder l'image générée dans un fichier
            string filePath = "graphe_skia.png";
            using (var image = surface.Snapshot())
            using (var data = image.Encode())
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }

            Console.WriteLine($"Le graphe a été visualisé et sauvegardé sous : {filePath}");
            Process.Start("explorer.exe", filePath);
        }
    }

}

