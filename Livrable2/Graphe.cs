using MySqlX.XDevAPI.Common;
using SkiaSharp;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Livrable2
{
    public class Graphe<T>
    {
        Noeud<T>[] graph;

        public Noeud<T>[] Graph
        {
            get { return graph; }
        }

        /// <summary>
        /// Permet de construire un graph avec les fichiers connexions et stations
        /// </summary>
        /// <param name="connexions">Fichier contenant les connexions entre stations</param>
        /// <param name="stations">Fichier contenant toutes les stations</param>
        public Graphe(string connexions, string stations)
        {
            double[,] matrice = MatriceIncidence(connexions);

            Noeud<T>[] graph = new Noeud<T>[matrice.GetLength(0)];
            for (int p = 1; p <= matrice.GetLength(0); p++)
            {
                graph[p - 1] = new Noeud<T>(p, matrice, stations);
            }
            this.graph = graph;
        }

        /// <summary>
        /// Convertis le nom d'une station en son identifiant 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns>L'identifiant de nom</returns>
        public int NomAIdentifiant(string nom)
        {
            int result = -1;
            for (int i = 0; i < graph.Length; i++)
            {
                if (nom == graph[i].Station.Nom)
                {
                    result = graph[i].Station.Id;
                }
            }
            return result;
        }

        /// <summary>
        /// Cette méthode permet de générer la matrice d'incidence des stations de métro qui est essentiel pour la suite du projet
        /// </summary>
        /// <param name="connexion">Fichier contenant les connexions entre stations</param>
        /// <returns>La matrice qui associe pour chaque station de métro ces liens avec les autres</returns>
        static double[,] MatriceIncidence(string connexion)
        {
            double[] m = Array.ConvertAll(File.ReadAllText(connexion)
                .Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries), double.Parse);
            double[,] matrice = new double[Convert.ToInt32(m[0]), Convert.ToInt32(m[1])];
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    matrice[i, j] = 0;
                }
            }
            for (int i = 2; i < m.Length; i += 4)
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

        /// <summary>
        /// Vérifie si le graphe contient un cycle
        /// On utilise un parcourt en profondeur
        /// </summary>
        /// <param name="graphe">Graphe sous forme de tableau de noeuds</param>
        /// <returns>True si un cycle est détecté</returns>
        static bool ContientCycle(Noeud<T>[] graphe)
        {
            List<int> visite = new List<int>();
            List<int> parent = new List<int>(new int[graphe.Length]);

            for (int i = 0; i < graphe.Length; i++)
            {
                parent[i] = -1;
            }

            List<int> aVisiter = new List<int>();

            foreach (Noeud<T> noeud in graphe)
            {
                if (!visite.Contains(noeud.Station.Id))
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
                            if (!visite.Contains(voisin))
                            {
                                aVisiter.Add(voisin);
                                parent[voisin] = actuel;
                            }
                            else if (parent[actuel] != voisin)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Vérifie si le graphe est bien connexe
        /// </summary>
        /// <param name="graphe">Graphe sous forme de tableau de noeuds</param>
        /// <returns>True si le graphe est connexe</returns>
        static bool EstConnexe(Noeud<T>[] graphe)
        {
            bool result = false;
            if (graphe.Length == ParcoursLargeur(graphe, graphe[0].Station.Id).Length)
            {
                result = true;
            }
            return result;
        }


        #region PARCOURS DE GRAPHE

        /// <summary>
        /// Effectue un parcours en profondeur depuis un identifiant de station.
        /// </summary>
        /// <param name="graph">Graphe à parcourir</param>
        /// <param name="id">Identifiant de la station de départ</param>
        /// <returns>Liste des stations visitées</returns>
        static int[] ParcoursProfondeur(Noeud<T>[] graph, int id)
        {
            List<int> resultat = new List<int>();
            List<int> visite = new List<int>();
            List<int> pile = new List<int>();

            pile.Add(id);
            while (pile.Count > 0)
            {
                int noeudActuel = pile[pile.Count - 1];
                pile.RemoveAt(pile.Count - 1);

                if (!visite.Contains(noeudActuel))
                {
                    visite.Add(noeudActuel);
                    resultat.Add(noeudActuel);


                    for (int i = graph[noeudActuel].Lien.Length - 1; i >= 0; i--)
                    {
                        int voisin = graph[noeudActuel].Lien[i];
                        if (!visite.Contains(voisin))
                        {
                            pile.Add(voisin);
                        }
                    }
                }
            }
            return resultat.ToArray();
        }


        /// <summary>
        /// Effectue un parcours en largeur depuis un identifiant de station
        /// </summary>
        /// <param name="graph">Graphe à parcourir</param>
        /// <param name="id">Identifiant de la station de départ</param>
        /// <returns>Liste des stations visitées</returns>
        static int[] ParcoursLargeur(Noeud<T>[] graph, int id)
        {

            List<int> parcours = new List<int>();
            parcours.Add(id);
            for (int i = 0; i < parcours.Count; i++)
            {
                for (int j = 0; j < graph[parcours[i] - 1].Lien.Length; j++)
                {
                    if (!parcours.Contains(graph[parcours[i] - 1].Lien[j]))
                    {
                        parcours.Add(graph[parcours[i] - 1].Lien[j]);
                    }
                }
            }
            return parcours.ToArray();
        }

        #endregion


        #region PLUS COURT CHEMIN

        /// <summary>
        /// Détermine le plus court chemin entre deux stations en comparant Dijkstra, Bellman-Ford et Floyd-Warshall
        /// </summary>
        /// <param name="departId">Identifiant de la station de départ</param>
        /// <param name="arriveeId">Identifiant de la station d’arrivée</param>
        /// <returns>Liste des stations représentant le plus court chemin</returns>
        public List<int> PlusCourtChemin(int departId, int arriveeId)
        {
            List<int> result = null;
            List<int> c1 = Dijkstra(departId, arriveeId);
            List<int> c2 = BellmanFord(departId, arriveeId);
            (double[,] mat, int[,] next) = FloydWarshall();
            List<int> c3 = ReconstituerChemin(departId, arriveeId, next);

            double t1 = CalculerTempsChemin(c1);
            double t2 = CalculerTempsChemin(c2);
            double t3 = CalculerTempsChemin(c3);

            if (t1 <= t2 && t1 <= t3)
            {
                result = c1;
            }
            else if (t2 <= t1 && t2 <= t3)
            {
                result = c2;
            }
            else if (t3 <= t2 && t3 <= t1)
            {
                result = c3;
            }
            return result;
        }

        /// <summary>
        /// Calcule le temps estimé pour un chemin donné.
        /// </summary>
        /// <param name="chemin">Liste des identifiants de stations du chemin.</param>
        /// <returns>Temps total en minutes.</returns>
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

                ///Trouver le temps entre les deux stations
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

                ///Trouver la ligne utilisée
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

        /// <summary>
        /// Applique l'algorithme de Dijkstra pour trouver le chemin le plus court.
        /// </summary>
        /// <param name="departId">Identifiant de départ.</param>
        /// <param name="arriveeId">Identifiant d’arrivée.</param>
        /// <returns>Liste du chemin le plus court.</returns>
        public List<int> Dijkstra(int departId, int arriveeId)
        {
            int n = graph.Length;

            double[] distances = new double[n];

            int[] precedents = new int[n];

            bool[] visite = new bool[n];

            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;
                precedents[i] = -1;
                visite[i] = false;
            }

            distances[departId - 1] = 0;

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

                if (u == -1)
                    break;

                visite[u] = true;

                Noeud<T> noeudActuel = graph[u];
                for (int k = 0; k < noeudActuel.Lien.Length; k++)
                {
                    int v = noeudActuel.Lien[k] - 1;
                    double poids = noeudActuel.Poids[k];

                    if (!visite[v] && distances[u] + poids < distances[v])
                    {
                        distances[v] = distances[u] + poids;
                        precedents[v] = u;
                    }
                }
            }

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

        /// <summary>
        /// Applique l'algorithme de Bellman-Ford pour trouver le chemin le plus court.
        /// </summary>
        /// <param name="departId">Identifiant de départ.</param>
        /// <param name="arriveeId">Identifiant d’arrivée.</param>
        /// <returns>Chemin le plus court.</returns>
        public List<int> BellmanFord(int departId, int arriveeId)
        {
            int n = graph.Length;

            double[] distances = new double[n];
            int[] precedents = new int[n];
            double[] distancesAvant = new double[n];

            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;
                distancesAvant[i] = double.MaxValue;
                precedents[i] = -1;
            }

            distances[departId - 1] = 0;

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

                
                bool identique = true;
                for (int j = 0; j < n; j++)
                {
                    if (distances[j] != distancesAvant[j])
                    {
                        identique = false;
                        break;
                    }
                }

                if (identique)
                {
                    break;
                }

                for (int j = 0; j < n; j++)
                {
                    distancesAvant[j] = distances[j];
                }
            }

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

        /// <summary>
        /// Applique l'algorithme de Floyd-Warshall pour pré-calculer les distances minimales entre toutes les paires de sommets.
        /// </summary>
        /// <returns>Tuple contenant les matrices des distances et des successeurs.</returns>
        public (double[,], int[,]) FloydWarshall()
        {
            int n = graph.Length;
            double[,] dist = new double[n, n];
            int[,] next = new int[n, n];
            int[,] lignes = new int[n, n];

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
                            lignes[i, j] = ligneIK;
                        }
                    }
                }
            }

            return (dist, next);
        }

        /// <summary>
        /// Reconstitue un chemin à partir de la matrice de successeurs de Floyd-Warshall.
        /// </summary>
        /// <param name="departId">Identifiant de départ.</param>
        /// <param name="arriveeId">Identifiant d’arrivée.</param>
        /// <param name="next">Matrice des successeurs.</param>
        /// <returns>Chemin reconstitué.</returns>
        public List<int> ReconstituerChemin(int departId, int arriveeId, int[,] next)
        {
            int u = departId - 1;
            int v = arriveeId - 1;

            if (next[u, v] == -1)
                return null;

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


        #region VISUALISATION

        /// <summary>
        /// Génère et sauvegarde une image du graphe avec les stations et leurs connexions
        /// </summary>
        public void VisualiserGraphe()
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
                    string lignePrincipale = n.Station.Lignes[0].ToString();
                    if (ligneCouleurs.TryGetValue(lignePrincipale, out var couleurLigne))
                        couleur = couleurLigne;
                }

                var nodePaint = new SKPaint { Color = couleur, IsAntialias = true };
                canvas.DrawCircle(pos, 6, nodePaint);


                canvas.DrawText(n.Station.Nom, pos.X, pos.Y - 10, textPaint);
            }


            string filePath = "graphe_paris_colore.png";
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);

            Console.WriteLine($"Graphe sauvegardé dans {filePath}");
            Process.Start("explorer.exe", filePath);
        }

        /// <summary>
        /// Génère une image du graphe avec un chemin spécifique dessiné en rouge
        /// </summary>
        /// <param name="chemin">Liste des stations à relier dans le graphe</param>
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

            ///Lignes du chemin en rouge
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


        #region COLORATION

        /// <summary>
        /// Trouve la coloration du graphe avec la méthode de Welsh Powell
        /// </summary>
        /// <returns>Liste avec les couleurs pour chaque stations</returns>
        public int[] WelshPowellColoration()
        {
            int n = graph.Length;
            int[] couleurs = new int[n];


            int[] ids = new int[n];
            int[] degres = new int[n];

            for (int i = 0; i < n; i++)
            {
                ids[i] = i;
                degres[i] = graph[i].Lien.Length;
            }


            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (degres[ids[j]] > degres[ids[i]])
                    {
                        int temp = ids[i];
                        ids[i] = ids[j];
                        ids[j] = temp;
                    }
                }
            }

            int couleurActuelle = 1;

            for (int i = 0; i < n; i++)
            {
                int idCourant = ids[i];

                if (couleurs[idCourant] == 0)
                {
                    couleurs[idCourant] = couleurActuelle;


                    for (int j = 0; j < n; j++)
                    {
                        int autreId = ids[j];

                        if (couleurs[autreId] == 0)
                        {
                            bool voisinADejaCetteCouleur = false;

                            for (int k = 0; k < graph[autreId].Lien.Length; k++)
                            {
                                int voisinId = graph[autreId].Lien[k] - 1;
                                if (couleurs[voisinId] == couleurActuelle)
                                {
                                    voisinADejaCetteCouleur = true;
                                    break;
                                }
                            }

                            if (!voisinADejaCetteCouleur)
                            {
                                couleurs[autreId] = couleurActuelle;
                            }
                        }
                    }

                    couleurActuelle++;
                }
            }
            return couleurs;
        }

        /// <summary>
        /// Retourne le nombre chromatique du graphe (nombre de couleurs utilisées)
        /// </summary>
        /// <returns>Nombre chromatique</returns>
        public int NombreChromatique()
        {
            int[] couleurs = WelshPowellColoration();
            int nbCouleurs = couleurs.Max();

            return nbCouleurs;

        }

        /// <summary>
        /// Indique si le graphe est biparti (2 couleurs suffisent)
        /// </summary>
        /// <returns>True si biparti</returns>
        public bool EstBiparti()
        {
            bool result = false;
            if (NombreChromatique() == 2)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Vérifie si le graphe est planaire selon le théorème des 4 couleurs
        /// </summary>
        /// <returns>True si planaire</returns>
        public bool EstPotentiellementPlanaire()
        {
            bool result = false;
            if (NombreChromatique() <= 4)
            {
                result = true;
            }
            return result;
        }

        #endregion
    }

}
