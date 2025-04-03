using SkiaSharp;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Livrable2
{
    internal class Graphe<T>
    {
        Noeud<T>[] graph;
        public Graphe(string connexions, string stations)
        {
            double[,] matrice = MatriceIncidence(connexions); //On génère la martrice d'incidence qui est essentielle pour la suite 

            Noeud<T>[] graph = new Noeud<T>[matrice.GetLength(0)];  //On génère une liste de noeud qui va pour chaque station, répertorier ses liens avec les autres
            for (int p = 1; p <= matrice.GetLength(0); p++)
            {
                graph[p-1] = new Noeud<T>(p, matrice, stations);
            }
            this.graph = graph;


        }


        //Vérifie si le graphe contient un cycle
        static bool ContientCycle(Noeud<T>[] graphe)
        {
            List<int> visite = new List<int>(); // Liste des nœuds déjà visités
            List<int> parent = new List<int>(new int[graphe.Length]); // Liste des parents pour suivre la relation parent-enfant

            for (int i = 0; i < graphe.Length; i++)
            {
                parent[i] = -1;
            }

            List<int> aVisiter = new List<int>(); // Liste utilisée comme pile pour le parcours en profondeur

            foreach (Noeud<T> noeud in graphe)
            {
                if (!visite.Contains(noeud.Station.Id)) // Vérifie si le nœud n'a pas été visité
                {
                    aVisiter.Add(noeud.Station.Id);

                    while (aVisiter.Count > 0)
                    {
                        int actuel = aVisiter[aVisiter.Count - 1];
                        aVisiter.RemoveAt(aVisiter.Count - 1);

                        if (!visite.Contains(actuel))
                        {
                            visite.Add(actuel);
                        }

                        foreach (int voisin in graphe[actuel].Lien)
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


        //Vérifie si le graphe est bien connexe
        static bool EstConnexe(Noeud<T>[] graphe)
        {
            bool result = false;    //Si le graphe est connexe, alors le degré d'un graphe doit être égal au nombre de sommets visité par le parcours en largeur
            if (graphe.Length == ParcoursLargeur(graphe, graphe[0].Station.Id).Length)
            {
                result = true;
            }
            return result;
        }

        #region PARCOURS DE GRAPHE
        //Parcours en largeur sur le graphe à partir d'un identifiant de station
        static int[] ParcoursProfondeur(Noeud<T>[] graph, int id)
        {
            List<int> resultat = new List<int>(); // Liste pour stocker le parcours des nœuds visités
            List<int> visite = new List<int>(); // Liste pour suivre les nœuds déjà visités
            List<int> pile = new List<int>(); // Liste utilisée comme une pile pour le parcours en profondeur

            pile.Add(id);
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


        //Parcours en largeur sur le graphe à partir d'un identifiant de station
        static int[] ParcoursLargeur(Noeud<T>[] graph, int id)
        {
            //On construit la list du parcour en largeur du graphe
            List<int> parcours = new List<int>();
            parcours.Add(id);                              //On ajoute le sommet de départ, 
            for (int i = 0; i < parcours.Count; i++)          //puis on ajoute les sommet adjacents au premier sommet
            {
                for (int j = 0; j < graph[parcours[i]-1].Lien.Length; j++)
                {   //Si le sommet est deja présent dans la liste, alors on ne l'ajoute pas dans la liste
                    if (!parcours.Contains(graph[parcours[i]-1].Lien[j]))
                    {
                        parcours.Add(graph[parcours[i]-1].Lien[j]);
                    }
                }
            }
            return parcours.ToArray();
        }

        #endregion

        //Cette méthode permet de générer la matrice d'incidence des stations de métro qui est essentiel pour la suite du projet
        static double[,] MatriceIncidence(string connexion) 
        {
            double[] m = Array.ConvertAll(File.ReadAllText(connexion) // On sépare les nombres
                .Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries), double.Parse);
            double[,] matrice = new double[Convert.ToInt32(m[0]), Convert.ToInt32(m[1])];
            for (int i = 0; i < matrice.GetLength(0); i++) // On initialise la matrice comme étant nulle
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    matrice[i, j] = 0;
                }
            }
            for (int i = 2; i < m.Length; i += 4) // On modifie la matrice aux endroits ou il y a une liaison
            {
                if (matrice[Convert.ToInt32(m[i] - 1), Convert.ToInt32(m[i + 1] - 1)] == 0)
                {
                    matrice[Convert.ToInt32(m[i] - 1), Convert.ToInt32(m[i + 1] - 1)] = m[i + 3];
                    if (m[i + 2] == 0)
                    {
                        matrice[Convert.ToInt32(m[i + 1] - 1), Convert.ToInt32(m[i] - 1)] = m[i + 3];
                    }
                }
            }
            return matrice;
        }


        //Methode permetant de visualiser le graphe
        public void VisualiserGrapheParCoordonnees()
        {
            int width = 2400;
            int height = 2000;
            var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // Min/max pour normalisation
            double minLon = graph.Min(n => n.Station.Longitude);
            double maxLon = graph.Max(n => n.Station.Longitude);
            double minLat = graph.Min(n => n.Station.Latitude);
            double maxLat = graph.Max(n => n.Station.Latitude);
            int margin = 50;

            // Coordonnées normalisées
            Dictionary<int, SKPoint> positions = new();
            foreach (var n in graph)
            {
                float x = (float)((n.Station.Longitude - minLon) / (maxLon - minLon) * (width - 2 * margin) + margin);
                float y = (float)((1 - (n.Station.Latitude - minLat) / (maxLat - minLat)) * (height - 2 * margin) + margin);
                positions[n.Station.Id] = new SKPoint(x, y);
            }

            // Arêtes (liens)
            var edgePaint = new SKPaint { Color = SKColors.Gray, StrokeWidth = 1, IsAntialias = true };
            foreach (var n in graph)
            {
                var from = positions[n.Station.Id];
                foreach (int voisin in n.Lien)
                {
                    if (positions.ContainsKey(voisin))
                    {
                        var to = positions[voisin];
                        canvas.DrawLine(from, to, edgePaint);
                    }
                }
            }

            // ➕ Définir une palette de couleurs de lignes de métro
            Dictionary<string, SKColor> ligneCouleurs = new()
            {
                ["1"] = SKColors.Gold,
                ["2"] = SKColors.DeepSkyBlue,
                ["3"] = SKColors.OliveDrab,
                ["3bis"] = SKColors.MediumSeaGreen,
                ["4"] = SKColors.Purple,
                ["5"] = SKColors.OrangeRed,
                ["6"] = SKColors.SeaGreen,
                ["7"] = SKColors.HotPink,
                ["7bis"] = SKColors.LightBlue,
                ["8"] = SKColors.MediumSlateBlue,
                ["9"] = SKColors.Olive,
                ["10"] = SKColors.DarkGoldenrod,
                ["11"] = SKColors.DarkOrange,
                ["12"] = SKColors.ForestGreen,
                ["13"] = SKColors.DarkTurquoise,
                ["14"] = SKColors.DarkViolet
            };

            // Affichage des sommets (stations)
            var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 10,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            foreach (var n in graph)
            {
                var pos = positions[n.Station.Id];

                // 🖌️ Couleur selon la première ligne de la station
                SKColor couleur = SKColors.DarkGray;
                if (n.Station.Lignes.Length > 0)
                {
                    string lignePrincipale = n.Station.Lignes[0].ToString();
                    if (ligneCouleurs.TryGetValue(lignePrincipale, out var couleurLigne))
                        couleur = couleurLigne;
                }

                var nodePaint = new SKPaint { Color = couleur, IsAntialias = true };
                canvas.DrawCircle(pos, 6, nodePaint);

                // 🏷️ Nom de la station en noir
                canvas.DrawText(n.Station.Nom, pos.X, pos.Y - 10, textPaint);
            }

            // 📤 Sauvegarde de l'image
            string filePath = "graphe_paris_colore.png";
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);

            Console.WriteLine($"Graphe sauvegardé dans {filePath}");
            Process.Start("explorer.exe", filePath);
        }

    }

}

