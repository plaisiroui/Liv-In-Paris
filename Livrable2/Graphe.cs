using SkiaSharp;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Livrable2
{
    internal class Graphe<T>
    {
        Noeud<T>[] graph;

        public Noeud<T>[] Graph
        {
            get { return graph; }
        }
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

        //Permet de convertir le nom d'une station en son identifiant
        public int NomAIdentifiant(string nom)
        {
            int result = 0;
            for (int i = 0; i < graph.Length; i++)
            {
                if (nom == graph[i].Station.Nom)
                {
                    result = graph[i].Station.Id;
                }
            }
            return result;
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


        #region PLUS COURT CHEMIN

        public double CalculerTempsChemin(List<int> chemin)
        {
            double tempsTotal = 0;
            int lignePrecedente = -1;

            for (int i = 0; i < chemin.Count - 1; i++)
            {
                int stationActuelleId = chemin[i];
                int stationSuivanteId = chemin[i + 1];

                Noeud<T> stationActuelle = graph[stationActuelleId - 1];
                Noeud<T> stationSuivante = graph[stationSuivanteId - 1];

                // Trouver le temps entre les deux stations
                double temps = 0;
                bool lienTrouve = false;
                for (int k = 0; k < stationActuelle.Lien.Length; k++)
                {
                    if (stationActuelle.Lien[k] == stationSuivanteId)
                    {
                        temps = stationActuelle.Poids[k];
                        lienTrouve = true;
                        break;
                    }
                }

                if (!lienTrouve)
                {
                    Console.WriteLine("Erreur. Pas de liaison directe entre deux stations");
                    return -1;
                }

                // Trouver la ligne utilisée
                int ligneCommune = TrouverLigneCommune(stationActuelle.Station.Lignes, stationSuivante.Station.Lignes);

                if (lignePrecedente != -1 && ligneCommune != lignePrecedente)
                {
                    tempsTotal += 3;
                }

                tempsTotal += temps;
                lignePrecedente = ligneCommune;
            }

            return tempsTotal;
        }

        public List<int> Dijkstra(int departId, int arriveeId)
        {
            int n = graph.Length;

            //Tableau pour stocker les distances minimales depuis la station de départ
            double[] distances = new double[n];

            //Tableau pour stocker les prédécesseurs
            int[] precedents = new int[n];

            //tableau pour marquer les stations déjà visitées
            bool[] visite = new bool[n];

            //initialisation des tableaux
            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;  
                precedents[i] = -1;              
                visite[i] = false;               
            }

            distances[departId - 1] = 0;

            //programme principal
            for (int i = 0; i < n; i++)
            {
                int u = -1;
                double minDistance = double.MaxValue;

                for (int j = 0; j < n; j++)
                {
                    if (!visite[j] && distances[j] < minDistance)
                    {
                        minDistance = distances[j];
                        u = j;
                    }
                }

                //si aucune station accessible trouvée, on arrête
                if (u == -1)
                    break;

                visite[u] = true;

                //Pour chaque voisin de la station u
                Noeud<T> noeudActuel = graph[u];
                for (int k = 0; k < noeudActuel.Lien.Length; k++)
                {
                    int v = noeudActuel.Lien[k] - 1;      
                    double poids = noeudActuel.Poids[k];  

                    // Si une meilleure distance est trouvée vers v via u
                    if (!visite[v] && distances[u] + poids < distances[v])
                    {
                        distances[v] = distances[u] + poids;
                        precedents[v] = u;
                    }
                }
            }

            //reconstruction du chemin
            List<int> chemin = new List<int>();
            int actuel = arriveeId - 1;

            while (actuel != -1)
            {
                chemin.Insert(0, graph[actuel].Station.Id);
                actuel = precedents[actuel];
            }

            //on vérifie que il y a bien un chemin
            if (chemin.Count == 0 || chemin[0] != departId)
            {
                Console.WriteLine("Aucun chemin trouvé.");
                return null;
            }

            return chemin;
        }

        public List<int> BellmanFord(int departId, int arriveeId)
        {
            int n = graph.Length;

            double[] distances = new double[n];
            int[] precedents = new int[n];

            // Initialisation
            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;
                precedents[i] = -1;
            }

            distances[departId - 1] = 0;

            // Boucle de relaxation |n-1| fois
            for (int i = 0; i < n - 1; i++)
            {
                foreach (var noeud in graph)
                {
                    int u = noeud.Station.Id - 1;

                    for (int k = 0; k < noeud.Lien.Length; k++)
                    {
                        int v = noeud.Lien[k] - 1;
                        double poids = noeud.Poids[k];

                        if (distances[u] != double.MaxValue && distances[u] + poids < distances[v])
                        {
                            distances[v] = distances[u] + poids;
                            precedents[v] = u;
                        }
                    }
                }
            }

            // Reconstruction du chemin
            List<int> chemin = new List<int>();
            int actuel = arriveeId - 1;

            while (actuel != -1)
            {
                chemin.Insert(0, graph[actuel].Station.Id);
                actuel = precedents[actuel];
            }

            if (chemin.Count == 0 || chemin[0] != departId)
            {
                Console.WriteLine("Aucun chemin trouvé.");
                return null;
            }

            return chemin;
        }

        int TrouverLigneCommune(int[] lignes1, int[] lignes2)
        {
            HashSet<int> set = new HashSet<int>(lignes1);
            foreach (var l in lignes2)
            {
                if (set.Contains(l)) return l;
            }
            return -1;
        }

        public (double[,], int[,]) FloydWarshall()
        {
            int n = graph.Length;
            double[,] dist = new double[n, n];
            int[,] next = new int[n, n];
            int[,] lignes = new int[n, n]; // ligne utilisée pour aller de i à j

            //initialisation
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dist[i, j] = (i == j) ? 0 : double.MaxValue;
                    next[i, j] = -1;
                    lignes[i, j] = -1;
                }

                Noeud<T> noeudU = graph[i];
                for (int k = 0; k < noeudU.Lien.Length; k++)
                {
                    int j = noeudU.Lien[k] - 1;
                    double poids = noeudU.Poids[k];
                    int ligneCommune = TrouverLigneCommune(noeudU.Station.Lignes, graph[j].Station.Lignes);

                    dist[i, j] = poids;
                    next[i, j] = j;
                    lignes[i, j] = ligneCommune;
                }
            }

            // Triple boucle de Floyd-Warshall
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (dist[i, k] == double.MaxValue) continue;

                    for (int j = 0; j < n; j++)
                    {
                        if (dist[k, j] == double.MaxValue) continue;

                        int ligneIK = lignes[i, k];
                        int ligneKJ = lignes[k, j];

                        double changement = 0;
                        if (ligneIK != -1 && ligneKJ != -1 && ligneIK != ligneKJ)
                        {
                            changement = 0;
                        }

                        double nouvelleDistance = dist[i, k] + dist[k, j] + changement;

                        if (nouvelleDistance < dist[i, j])
                        {
                            dist[i, j] = nouvelleDistance;
                            next[i, j] = next[i, k];
                            lignes[i, j] = ligneIK; // on garde la ligne de départ du chemin
                        }
                    }
                }
            }

            return (dist, next);
        }

        public List<int> ReconstituerChemin(int departId, int arriveeId, int[,] next)
        {
            int u = departId - 1;
            int v = arriveeId - 1;

            if (next[u, v] == -1)
                return null; // aucun chemin possible

            List<int> chemin = new List<int> { departId };

            while (u != v)
            {
                u = next[u, v];
                if (u == -1)
                    return null;

                chemin.Add(u + 1);
            }

            return chemin;
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

        #region VISUALISATION

        //Methode permetant de visualiser le graphe
        public void VisualiserGraphe()
        {
            int width = 4800;
            int height = 4000;
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

            //Définir une palette de couleurs de lignes de métro
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

                //Couleur selon la première ligne de la station
                SKColor couleur = SKColors.DarkGray;
                if (n.Station.Lignes.Length > 0)
                {
                    string lignePrincipale = n.Station.Lignes[0].ToString();
                    if (ligneCouleurs.TryGetValue(lignePrincipale, out var couleurLigne))
                        couleur = couleurLigne;
                }

                var nodePaint = new SKPaint { Color = couleur, IsAntialias = true };
                canvas.DrawCircle(pos, 6, nodePaint);

                //Nom de la station en noir
                canvas.DrawText(n.Station.Nom, pos.X, pos.Y - 10, textPaint);
            }

            //Sauvegarde de l'image
            string filePath = "graphe_paris_colore.png";
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);

            Console.WriteLine($"Graphe sauvegardé dans {filePath}");
            Process.Start("explorer.exe", filePath);
        }

        public void VisualiserGrapheAvecChemin(List<int> chemin)
        {
            int width = 4800;
            int height = 4000;
            var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            double minLon = graph.Min(n => n.Station.Longitude);
            double maxLon = graph.Max(n => n.Station.Longitude);
            double minLat = graph.Min(n => n.Station.Latitude);
            double maxLat = graph.Max(n => n.Station.Latitude);
            int margin = 50;

            Dictionary<int, SKPoint> positions = new();
            foreach (var n in graph)
            {
                float x = (float)((n.Station.Longitude - minLon) / (maxLon - minLon) * (width - 2 * margin) + margin);
                float y = (float)((1 - (n.Station.Latitude - minLat) / (maxLat - minLat)) * (height - 2 * margin) + margin);
                positions[n.Station.Id] = new SKPoint(x, y);
            }

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

            //Lignes du chemin en rouge
            var cheminPaint = new SKPaint { Color = SKColors.Red, StrokeWidth = 4, IsAntialias = true };
            for (int i = 0; i < chemin.Count - 1; i++)
            {
                int fromId = chemin[i];
                int toId = chemin[i + 1];

                if (positions.ContainsKey(fromId) && positions.ContainsKey(toId))
                {
                    canvas.DrawLine(positions[fromId], positions[toId], cheminPaint);
                }
            }

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
                SKColor couleur = SKColors.DarkGray;
                if (n.Station.Lignes.Length > 0)
                {
                    string ligne = n.Station.Lignes[0].ToString();
                    if (ligneCouleurs.TryGetValue(ligne, out var c))
                        couleur = c;
                }

                var nodePaint = new SKPaint { Color = couleur, IsAntialias = true };
                canvas.DrawCircle(pos, 6, nodePaint);
                canvas.DrawText(n.Station.Nom, pos.X, pos.Y - 10, textPaint);
            }

            string filePath = "graphe_chemin_rouge.png";
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);

            Console.WriteLine($"Graphe avec chemin sauvegardé dans {filePath}");
            Process.Start("explorer.exe", filePath);
        }

        #endregion 
    }

}

