using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Engines;
using System.Globalization;
using System.Threading;

namespace Livrable2
{
    class Program
    {
       
        /*public static void Main(string[] args)
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

        }*/

        static string connectionString = "Server=localhost;Database=livinparis;User ID=root;Password=root;";

        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;


            while (true)
            {
                Console.WriteLine("===== Menu Principal =====");
                Console.WriteLine("1. Module Client");
                Console.WriteLine("2. Module Cuisinier ");
                Console.WriteLine("3. Module Commande ");
                Console.WriteLine("4. Module Statistique");
                Console.WriteLine("5. Module Autre");
                Console.WriteLine("6. Quitter");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();



                switch (choix)
                {
                    case "1":
                        ModuleClient();
                        break;
                    case "2":
                        ModuleCuisinier();
                        break;
                    case "3":
                        ModuleCommande();
                        Console.WriteLine("en cours");
                        break;
                    case "4":
                        ModuleStatistique();
                        break;
                    case "5":
                        ModuleAutre();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Choix invalide, veuillez réessayer.");
                        break;
                }
            }
        }


        #region MODULE CLIENT
        static void ModuleClient()
        {
            while (true)
            {
                Console.WriteLine("\n===== Module Client =====");
                Console.WriteLine("1. Ajouter un client");
                Console.WriteLine("2. Supprimer un client");
                Console.WriteLine("3. Modifier un client");
                Console.WriteLine("4. Afficher les clients");
                Console.WriteLine("5. Retour au menu principal");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                if (choix == "1")
                {
                    AjouterClient();
                }
                else if (choix == "2")
                {
                    SupprimerClient();
                }
                else if (choix == "3")
                {
                    ModifierClient();
                }
                else if (choix == "4")
                {
                    AfficherClients();
                }
                else if (choix == "5")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Choix invalide, veuillez réessayer.");
                }
            }
        }

        static string GenererIDUniqueUtilisateur()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                int numero = 1;
                while (true)
                {
                    string id = "U" + numero.ToString("D3");
                    string query = "SELECT COUNT(*) FROM Utilisateur WHERE idUtilisateur = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            return id;
                        }
                    }
                    numero++;
                }
            }
        }

        static void AjouterClient()
        {
            Console.WriteLine("\n=== AJOUT D'UN NOUVEAU CLIENT ===");
            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();
            Console.Write("Téléphone : ");
            string telephone = Console.ReadLine();
            Console.Write("Adresse Mail : ");
            string email = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string password = Console.ReadLine();
            Console.Write("Type de client (Particulier, Entreprise, Autre) : ");
            string typeClient = Console.ReadLine();

          
            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);


            string idUtilisateur = GenererIDUniqueUtilisateur();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteUtilisateur = "INSERT INTO Utilisateur (idUtilisateur, adresseMail, nom, prenom, adresse, telephone, password, StationLaPlusProche) VALUES (@id, @mail, @nom, @prenom, @adresse, @telephone, @password, @idstationPP)";
                using (MySqlCommand cmd = new MySqlCommand(requeteUtilisateur, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idUtilisateur);
                    cmd.Parameters.AddWithValue("@mail", email);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@prenom", prenom);
                    cmd.Parameters.AddWithValue("@adresse", adresse);
                    cmd.Parameters.AddWithValue("@telephone", telephone);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@idstationPP", idstationPP);  
                    cmd.ExecuteNonQuery();
                }

                string requeteClient = "INSERT INTO Client (idUtilisateur, typeClient) VALUES (@id, @type)";
                using (MySqlCommand cmd = new MySqlCommand(requeteClient, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idUtilisateur);
                    cmd.Parameters.AddWithValue("@type", typeClient);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Client ajouté avec succès.");
        }

        static void SupprimerClient()
        {

            Console.WriteLine("\n=== SUPPRESSION D'UN CLIENT ===");

            Console.Write("ID du client à supprimer : ");
            string idClient = Console.ReadLine();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string[] listRequete = {
                "DELETE FROM Evaluation WHERE idEvaluateur = @id" ,
                "DELETE FROM Commande WHERE idUtilisateur = @id",
                "DELETE FROM AdresseLivraison WHERE idUtilisateur = @id",
                "DELETE FROM Client WHERE idUtilisateur = @id",
                "DELETE FROM Utilisateur WHERE idUtilisateur = @id"
            };
                foreach (string requete in listRequete)
                {
                    using (MySqlCommand cmd = new MySqlCommand(requete, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idClient);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine("Client supprimé avec succès.");
        }

        static void ModifierClient()
        {

            Console.WriteLine("\n=== MODIFICATION D'UN CLIENT ===");

            Console.Write("ID du client à modifier: ");
            string id = Console.ReadLine();
            Console.Write("Nouveau nom: ");
            string nom = Console.ReadLine();
            Console.Write("Nouveau prénom: ");
            string prenom = Console.ReadLine();
            Console.Write("Nouvelle adresse: ");
            string adresse = Console.ReadLine();
            Console.Write("Nouveau téléphone: ");
            string telephone = Console.ReadLine();
            Console.Write("Nouveau type de client: ");
            string typeClient = Console.ReadLine();

            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Nouvelle station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string requeteUti = "UPDATE Utilisateur SET nom=@nom, prenom=@prenom, adresse=@adresse, telephone=@telephone, StationLaPlusProche=@idstationPP WHERE idUtilisateur=@id;";
                using (MySqlCommand cmd = new MySqlCommand(requeteUti, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@prenom", prenom);
                    cmd.Parameters.AddWithValue("@adresse", adresse);
                    cmd.Parameters.AddWithValue("@telephone", telephone);
                    cmd.Parameters.AddWithValue("@idstationPP", idstationPP);
                    cmd.ExecuteNonQuery();
                }

                string requeteclient = "UPDATE Client SET typeClient=@type WHERE idUtilisateur=@id;";
                using (MySqlCommand cmd = new MySqlCommand(requeteclient, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@type", typeClient);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Client modifié avec succès !");
        }

        static void AfficherClients()
        {

            Console.WriteLine("\n=== AFFICHAGE D'UN CLIENT ===");

            Console.WriteLine("\nAfficher les clients par :");
            Console.WriteLine("1. Nom (A-Z)");
            Console.WriteLine("2. Rue");
            Console.WriteLine("3. Montant des achats cumulés");
            Console.Write("Choix : ");
            string choix = Console.ReadLine();

            string orderBy = "nom";
            if (choix == "2") orderBy = "adresse";
            else if (choix == "3") orderBy = "total_achats DESC";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string requeteAffichageC = "SELECT Client.idUtilisateur, nom, prenom, adresse, " +
                               "IFNULL(SUM(Commande.prix), 0) AS total_achats, COUNT(idCommande) AS nb_commandes " +
                               "FROM Utilisateur " +
                               "JOIN Client using(idUtilisateur) " +
                               "JOIN Commande using(idUtilisateur) " +
                               "GROUP BY idUtilisateur ORDER BY " + orderBy + ";";

                using (MySqlCommand cmd = new MySqlCommand(requeteAffichageC, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("ID: " + reader["idUtilisateur"] + ", Nom: " + reader["nom"] + " " + reader["prenom"] + ", Adresse: " + reader["adresse"] + ", Achats: " + reader["total_achats"] + "E, Commandes: " + reader["nb_commandes"]);
                    }
                    reader.Close();
                }
            }
        }
        #endregion

        #region MODULE CUISINIER
        static void ModuleCuisinier()
        {
            while (true)
            {
                Console.WriteLine("\n===== MODULE CUISINIER =====");
                Console.WriteLine("1. Ajouter un cuisinier");
                Console.WriteLine("2. Modifier un cuisinier");
                Console.WriteLine("3. Supprimer un cuisinier");
                Console.WriteLine("4. Afficher les clients servis");
                Console.WriteLine("5. Afficher les plats réalisés par fréquence");
                Console.WriteLine("6. Afficher le plat du jour");
                Console.WriteLine("7. Ajouter un plat");
                Console.WriteLine("8. Retour au menu principal");
                Console.Write("Choisissez une option: ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AjouterCuisinier();
                        break;
                    case "2":
                        ModifierCuisinier();
                        break;
                    case "3":
                        SupprimerCuisinier();
                        break;
                    case "4":
                        AfficherClientsServis();
                        break;
                    case "5":
                        AfficherPlatsParFrequence();
                        break;
                    case "6":
                        AfficherPlatDuJour();
                        break;
                    case "7":
                        AjouterPlat();
                        break;

                    case "8":
                        return;
                    default:
                        Console.WriteLine("Option invalide, veuillez réessayer.");
                        break;
                }
            }
        }

        static void AjouterCuisinier()
        {
            Console.WriteLine("\n=== AJOUT D'UN NOUVEAU CUISINIER ===");
            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            Console.Write("Prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Adresse : ");
            string adresse = Console.ReadLine();
            Console.Write("Téléphone : ");
            string telephone = Console.ReadLine();
            Console.Write("Adresse Mail : ");
            string email = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string password = Console.ReadLine();
            Console.Write("Spécialité culinaire : ");
            string specialite = Console.ReadLine();

            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);


            string idUtilisateur = GenererIDUniqueUtilisateur();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteUtilisateur = "INSERT INTO Utilisateur (idUtilisateur, adresseMail, nom, prenom, adresse, telephone, password, StationLaPlusProche) VALUES (@id, @mail, @nom, @prenom, @adresse, @telephone, @password, @idstationPP)";
                using (MySqlCommand cmd = new MySqlCommand(requeteUtilisateur, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idUtilisateur);
                    cmd.Parameters.AddWithValue("@mail", email);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@prenom", prenom);
                    cmd.Parameters.AddWithValue("@adresse", adresse);
                    cmd.Parameters.AddWithValue("@telephone", telephone);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@idstationPP", idstationPP);
                    cmd.ExecuteNonQuery();
                }

                string requeteClient = "INSERT INTO Cuisinier (idUtilisateur, specialite) VALUES (@id, @specialite)";
                using (MySqlCommand cmd = new MySqlCommand(requeteClient, connection))
                {
                    cmd.Parameters.AddWithValue("@id", idUtilisateur);
                    cmd.Parameters.AddWithValue("@specialite", specialite);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Cuisinier ajouté avec succès.");
        }

        static void ModifierCuisinier()
        {

            Console.WriteLine("\n=== MODIFICATION D'UN CUISINIER ===");

            Console.Write("ID du client à modifier: ");
            string id = Console.ReadLine();
            Console.Write("Nouveau nom: ");
            string nom = Console.ReadLine();
            Console.Write("Nouveau prénom: ");
            string prenom = Console.ReadLine();
            Console.Write("Nouvelle adresse: ");
            string adresse = Console.ReadLine();
            Console.Write("Nouveau téléphone: ");
            string telephone = Console.ReadLine();
            Console.Write("Nouvelle spécialité:  ");
            string specialite = Console.ReadLine();

            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string requeteUti = "UPDATE Utilisateur SET nom=@nom, prenom=@prenom, adresse=@adresse, telephone=@telephone, StationLaPlusProche=@idstationPP WHERE idUtilisateur=@id;";
                using (MySqlCommand cmd = new MySqlCommand(requeteUti, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nom", nom);
                    cmd.Parameters.AddWithValue("@prenom", prenom);
                    cmd.Parameters.AddWithValue("@adresse", adresse);
                    cmd.Parameters.AddWithValue("@telephone", telephone);
                    cmd.Parameters.AddWithValue("@idstationPP", idstationPP);
                    cmd.ExecuteNonQuery();
                }

                string requeteclient = "UPDATE Cuisinier SET specialite=@specialite WHERE idUtilisateur=@id;";
                using (MySqlCommand cmd = new MySqlCommand(requeteclient, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@type", specialite);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Cuisinier modifié avec succès !");


        }

        static void SupprimerCuisinier()
        {

            Console.WriteLine("\n=== SUPPRESSION D'UN CUISINIER ===");

            Console.Write("ID du cuisinier à supprimer : ");
            string idCuisinier = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string[] requetesDeleteCuisi = {
            "DELETE FROM Plat WHERE idUtilisateur = @id",
            "DELETE FROM Cuisinier WHERE idUtilisateur = @id",
            "DELETE FROM Utilisateur WHERE idUtilisateur = @id"
        };

                foreach (string requete in requetesDeleteCuisi)
                {
                    using (MySqlCommand cmd = new MySqlCommand(requete, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idCuisinier);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine("Cuisinier supprimé avec succès.");
        }

        static void AfficherClientsServis()
        {
            Console.Write("ID du cuisinier : ");
            string idCuisinier = Console.ReadLine();

            Console.Write("Afficher depuis l'inscription (Oui/Non) ? ");
            string choix = Console.ReadLine().ToUpper();

            string requeteAfficheClients;
            if (choix == "OUI")
            {
                requeteAfficheClients = "SELECT DISTINCT idUtilisateur, nom, prenom " +
                        "FROM Commande " +
                        "JOIN Client using(idUtilisateur) " +
                        "WHERE idUtilisateur = @id";

                using (MySqlCommand cmd = new MySqlCommand(connectionString))
                {
                    cmd.Parameters.AddWithValue("@id", idCuisinier);
                }
            }
            else if (choix == "NON")
            {
                Console.Write("Date de début (YYYY-MM-DD) : ");
                string dateDebut = Console.ReadLine();

                Console.Write("Date de fin (YYYY-MM-DD) : ");
                string dateFin = Console.ReadLine();

                requeteAfficheClients = "SELECT DISTINCT idUTilisateur, nom, prenom " +
                        "FROM Commande " +
                        "JOIN Client using(idUtilisateur) " +
                        "WHERE Cuisinier.idUtilisateur = @id " +
                        "AND dateCommande BETWEEN @dateDebut AND @dateFin";

                using (MySqlCommand cmd = new MySqlCommand(connectionString))
                {
                    cmd.Parameters.AddWithValue("@id", idCuisinier);

                    cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);

                }
            }
            else
            {
                Console.WriteLine("Veuillez choisir une réponse entre Oui et Non.");
            }


            if (choix == "OUI" || choix == "NON")
            {


                using (MySqlCommand cmd = new MySqlCommand(connectionString))
                {

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("Client : " + reader["nom"] + " " + reader["prenom"]);
                        }
                    }
                }
            }
        }

        static void AfficherPlatsParFrequence()
        {
            Console.Write("ID du cuisinier: ");
            string id = Console.ReadLine();

            string requeteFreqPlat = "SELECT Plat.nom, COUNT(*) as freq FROM Commande " +
                           "JOIN Plat using (idPlat) " +
                           "WHERE Cuisinier.idUtilisateur = @id " +
                           "GROUP BY Plat.nom ORDER BY freq DESC";

            using (MySqlCommand cmd = new MySqlCommand(connectionString))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Plat: " + reader["nom"] + " | Fréquence: " + reader["freq"]);
                    }
                }
            }
        }

        static void AfficherPlatDuJour()
        {
            string requetePlatJ = "SELECT nom FROM Plat WHERE platDuJour = 1";

            using (MySqlCommand cmd = new MySqlCommand(connectionString))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Plat du jour : " + reader["nom"]);
                    }
                    else
                    {
                        Console.WriteLine("Aucun plat du jour n'a été défini.");
                    }
                }
            }
        }






        #endregion

        #region MODULE COMMANDE

        public static void ModuleCommande()
        {


            while (true)
            {
                Console.WriteLine("\n==== Module Commande ===");
                Console.WriteLine("1. Passer une commande");
                Console.WriteLine("2. Afficher le prix d’une commande");
                Console.WriteLine("3. Déterminer le trajet optimal");
                Console.WriteLine("4. Modifier une commande");
                Console.WriteLine("5. Retourner au Menu principal");
                Console.Write("Choisissez une option : ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        PasserCommande();
                        break;
                    case "2":
                        AfficherPrixCommande();
                        break;
                    case "3":
                        DeterminerTrajetOptimal();
                        break;
                    case "4":
                        ModifierCommande();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Option invalide, veuillez réessayer.");
                        break;
                }
            }
        }


        static string GenererIDUniqueCommande()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                int numero = 1;
                while (true)
                {
                    string idC = "C" + numero.ToString("D3");
                    string requete = "SELECT COUNT(*) FROM Commande WHERE idCommande = @id";
                    using (MySqlCommand cmd = new MySqlCommand(requete, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", idC);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            return idC;
                        }
                    }
                    numero++;
                }
            }
        }

        public static void PasserCommande()
        {
            Console.Write("Êtes-vous client ? (Oui/Non) : ");
            string reponse = Console.ReadLine().ToUpper();

            if (reponse != "OUI")
            {
                Console.WriteLine("Vous devez être client pour passer une commande. Redirection vers la création d'un compte...");
                AjouterClient();
                return;
            }

            Console.Write("Entrez votre ID utilisateur : ");
            string idU = Console.ReadLine();

            Console.Write("Entrez l'ID du cuisinier auquel vous voulez commander : ");
            string idCuisinier = Console.ReadLine();

            int stationArrivee = -1; 
            int stationDepart = -1;  

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Station client
                string requeteStationArrive_client = "SELECT StationLaPlusProche FROM Utilisateur WHERE idUtilisateur = '" + idU + "'";
                using (MySqlCommand commande = new MySqlCommand(requeteStationArrive_client, connection))
                using (MySqlDataReader reader = commande.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stationArrivee = reader.GetInt32(0);
                    }
                    else
                    {
                        Console.WriteLine("Utilisateur introuvable. Veuillez réessayer.");
                        return;
                    }
                }

                // Station cuisinier
                string requeteStationDepart_cuisinier = "SELECT StationLaPlusProche FROM Utilisateur WHERE idUtilisateur = '" + idCuisinier + "'";
                using (MySqlCommand commande = new MySqlCommand(requeteStationDepart_cuisinier, connection))
                using (MySqlDataReader reader = commande.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stationDepart = reader.GetInt32(0);
                    }
                    else
                    {
                        Console.WriteLine("Cuisinier introuvable. Veuillez réessayer.");
                        return;
                    }
                }

                // Liste des plats proposés par ce cuisinier
                string requetePlats = "SELECT idPlat, nomPlat, prix FROM Plat WHERE idUtilisateur = '" + idCuisinier + "'";
                List<string> platsDisponibles = new List<string>();
                using (MySqlCommand cmd = new MySqlCommand(requetePlats, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Plats disponibles :");
                    while (reader.Read())
                    {
                        string idPlat = reader["idPlat"].ToString();
                        string nomPlat = reader["nomPlat"].ToString();
                        double prix = Convert.ToDouble(reader["prix"]);
                        platsDisponibles.Add(idPlat);
                        Console.WriteLine("- "+idPlat+ " : " +nomPlat+ " ("+prix+" euros)");
                    }
                }

                if (platsDisponibles.Count == 0)
                {
                    Console.WriteLine("Ce cuisinier n’a pas encore de plat.");
                    return;
                }

                Console.Write("Entrez l'ID du plat choisi : ");
                string idPlatChoisi = Console.ReadLine();
                if (!platsDisponibles.Contains(idPlatChoisi))
                {
                    Console.WriteLine("ID de plat invalide.");
                    return;
                }

                
                double prixCommande = 0;
                string requetePrix = "SELECT prix FROM Plat WHERE idPlat = '" + idPlatChoisi + "'";
                using (MySqlCommand cmd = new MySqlCommand(requetePrix, connection))
                {
                    prixCommande = Convert.ToDouble(cmd.ExecuteScalar());
                }

                
                string idCommande = GenererIDUniqueCommande();
                string statut = "En cours";

                string requeteCommande = @"INSERT INTO Commande (idCommande, dateCommande, prix, idStationDepart, idStationArrivee, statut, idUtilisateur, idCuisinier, idPlat) 
                                   VALUES (@idCommande, NOW(), @prix, @depart, @arrivee, @statut, @client, @cuisinier, @plat)";
                using (MySqlCommand cmd = new MySqlCommand(requeteCommande, connection))
                {
                    cmd.Parameters.AddWithValue("@idCommande", idCommande);
                    cmd.Parameters.AddWithValue("@prix", prixCommande);
                    cmd.Parameters.AddWithValue("@depart", stationDepart);
                    cmd.Parameters.AddWithValue("@arrivee", stationArrivee);
                    cmd.Parameters.AddWithValue("@statut", statut);
                    cmd.Parameters.AddWithValue("@client", idU);
                    cmd.Parameters.AddWithValue("@cuisinier", idCuisinier);
                    cmd.Parameters.AddWithValue("@plat", idPlatChoisi);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Commande enregistrée avec succès !");
            }
        }



        public static void AfficherPrixCommande()
        {
            Console.Write("Entrez l'id de la commande : ");
            string idCommande = Console.ReadLine();

            string requetePrix = "SELECT prix FROM Commande WHERE idCommande = '" + idCommande + "'";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand commande = new MySqlCommand(requetePrix, connection))
                using (MySqlDataReader reader = commande.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        double prix = reader.GetDouble(0);
                        Console.WriteLine("Le prix de la commande " + idCommande + " est de " + prix + "E.");
                    }
                    else
                    {
                        Console.WriteLine("Commande introuvable.");
                    }
                }
            }
        }


        public static void DeterminerTrajetOptimal()
        {
            Console.Write("Entrez le numéro de commande : ");
            string idCommande =Console.ReadLine();

            int stationDepart = -1;
            int stationArrivee = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                string requeteRecupStations = "SELECT idStationDepart, idStationArrivee FROM Commande WHERE idCommande = '" + idCommande+"'";
               

                using (MySqlCommand commande = new MySqlCommand(requeteRecupStations, connection))
                using (MySqlDataReader reader = commande.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stationDepart = reader.GetInt32(0);
                        stationArrivee = reader.GetInt32(1);
                    }
                    else
                    {
                        Console.WriteLine("Commande introuvable.");
                        return;
                    }
                }


                if (stationDepart == -1 || stationArrivee == -1)
                {
                    Console.WriteLine("Stations invalides pour cette commande.");
                    return;
                }
                string fichier_connexion = "Connexions.txt";
                string fichier_StationMetro = "StationsMetro.txt";
                Graphe<int> g2 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

                Console.WriteLine("\nChoisissez l'algorithme :");
                Console.WriteLine("1. Dijkstra");
                Console.WriteLine("2. Bellman-Ford");
                Console.WriteLine("3. Floyd-Warshall");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                List<int> chemin = new List<int>();

                switch (choix)
                {
                    case "1":
                        chemin = g2.Dijkstra(stationDepart, stationArrivee);
                        Console.WriteLine("Algorithme utilisé : Dijkstra");
                        //Afficher le chemin avec carte
                        break;

                    case "2":
                        chemin = g2.BellmanFord(stationDepart, stationArrivee);
                        Console.WriteLine("Algorithme utilisé : Bellman-Ford");
                        //Afficher le chemin avec carte
                        break;

                    case "3":
                        Console.WriteLine("En cours d'implémentation....");
                        break;

                    default:
                        Console.WriteLine("Choix invalide.");
                        return;
                }

                Console.WriteLine("Le trajet optimal pour la commande " + idCommande + " est :");
                Console.Write("Chemin : ");
                Console.WriteLine(string.Join(" -> ", chemin));
                Console.WriteLine("Temps estimé : " + g2.CalculerTempsChemin(chemin) + " minutes.");
            }
        }

        public   static void ModifierCommande()
        {
            Console.Write("Entrez l'ID de la commande à modifier : ");
            string idCommande = Console.ReadLine();

            Console.WriteLine("\nQue voulez-vous modifier ?");
            Console.WriteLine("1. Statut");
            Console.WriteLine("2. Cuisinier associé");
            Console.WriteLine("3. Station d’arrivée");
            Console.WriteLine("4. Prix");
            Console.WriteLine("5. Plat");
            Console.Write("Votre choix : ");
            string choix = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();

                switch (choix)
                {
                    case "1":
                        Console.Write("Nouveau statut (ex: En cours, Validée, Livrée) : ");
                        string statut = Console.ReadLine();
                        cmd.CommandText = "UPDATE Commande SET statut = @val WHERE idCommande = @id";
                        cmd.Parameters.AddWithValue("@val", statut);
                        break;

                    case "2":
                        Console.Write("Nouvel ID du cuisinier : ");
                        string idCuisinier = Console.ReadLine();
                        cmd.CommandText = "UPDATE Commande SET idCuisinier = @val WHERE idCommande = @id";
                        cmd.Parameters.AddWithValue("@val", idCuisinier);
                        break;

                    case "3":
                        Console.Write("Nouvel ID de station d’arrivée : ");
                        int idStation = int.Parse(Console.ReadLine());
                        cmd.CommandText = "UPDATE Commande SET idStationArrivee = @val WHERE idCommande = @id";
                        cmd.Parameters.AddWithValue("@val", idStation);
                        break;

                    case "4":
                        Console.Write("Nouveau prix : ");
                        double prix = double.Parse(Console.ReadLine());
                        cmd.CommandText = "UPDATE Commande SET prix = @val WHERE idCommande = @id";
                        cmd.Parameters.AddWithValue("@val", prix);
                        break;
                    case "5":
                        Console.Write("Nouvel ID du plat : ");
                        string idPlat = Console.ReadLine();
                        string requeteVerifPlat = "SELECT COUNT(*) FROM Plat WHERE idPlat = @idPlat";
                        using (MySqlCommand verifCmd = new MySqlCommand(requeteVerifPlat, connection))
                        {
                            verifCmd.Parameters.AddWithValue("@idPlat", idPlat);
                            int count = Convert.ToInt32(verifCmd.ExecuteScalar());
                            if (count == 0)
                            {
                                Console.WriteLine("Plat introuvable.");
                                return;
                            }
                        }

                        cmd.CommandText = "UPDATE Commande SET idPlat = @val WHERE idCommande = @id";
                        cmd.Parameters.AddWithValue("@val", idPlat);
                        break;


                    default:
                        Console.WriteLine("Choix invalide.");
                        return;
                }

                cmd.Parameters.AddWithValue("@id", idCommande);
                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                    Console.WriteLine("Commande modifiée avec succès.");
                else
                    Console.WriteLine("Erreur : commande non trouvée ou non modifiée.");
            }
        }

        static string GenererIDUniquePlat()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                int numero = 1;
                while (true)
                {
                    string id = "P" + numero.ToString("D3");
                    string query = "SELECT COUNT(*) FROM Plat WHERE idPlat = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            return id;
                        }
                    }
                    numero++;
                }
            }
        }


        static void AjouterPlat()
        {
            Console.Write("ID du cuisinier : ");
            string idCuisinier = Console.ReadLine();
            Console.Write("Nom du plat : ");
            string nom = Console.ReadLine();
            Console.Write("Type de plat (Entrée / Plat principal / Dessert) : ");
            string type = Console.ReadLine();
            Console.Write("Nombre de personnes : ");
            int nbPers = int.Parse(Console.ReadLine());
            Console.Write("Prix par personne : ");
            double prix = double.Parse(Console.ReadLine());
            Console.Write("Nationalité : ");
            string nat = Console.ReadLine();
            Console.Write("Régime alimentaire (ex: Végétarien, Halal...) : ");
            string regime = Console.ReadLine();
            Console.Write("Date de fabrication (YYYY-MM-DD) : ");
            DateTime dateFab = DateTime.Parse(Console.ReadLine());
            Console.Write("Date de péremption (YYYY-MM-DD) : ");
            DateTime datePer = DateTime.Parse(Console.ReadLine());
            Console.Write("ID de recette (ou une valeur fictive ex: R001 si non utilisé) : ");
            string idRecette = Console.ReadLine();
            Console.Write("Le plat est-il le plat du jour ? (Oui/Non) : ");
            bool platDuJour = Console.ReadLine().Trim().ToUpper() == "OUI";

            string idPlat = GenererIDUniquePlat();


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string requete = @"INSERT INTO Plat (idPlat, idUtilisateur, nomPlat, typePlat, nbPersonne, dateFabrication, datePeremption, prix, nationalite, regAlimentaire, idRecette, platDuJour) 
                           VALUES (@idPlat, @idUtilisateur, @nomPlat, @typePlat, @nbPers, @dateFab, @datePer, @prix, @nat, @regime, @idRecette, @platDuJour)";
                using (MySqlCommand cmd = new MySqlCommand(requete, conn))
                {
                    cmd.Parameters.AddWithValue("@idPlat", idPlat);
                    cmd.Parameters.AddWithValue("@idUtilisateur", idCuisinier);
                    cmd.Parameters.AddWithValue("@nomPlat", nom);
                    cmd.Parameters.AddWithValue("@typePlat", type);
                    cmd.Parameters.AddWithValue("@nbPers", nbPers);
                    cmd.Parameters.AddWithValue("@dateFab", dateFab);
                    cmd.Parameters.AddWithValue("@datePer", datePer);
                    cmd.Parameters.AddWithValue("@prix", prix);
                    cmd.Parameters.AddWithValue("@nat", nat);
                    cmd.Parameters.AddWithValue("@regime", regime);
                    cmd.Parameters.AddWithValue("@idRecette", idRecette);
                    cmd.Parameters.AddWithValue("@platDuJour", platDuJour);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Plat ajouté avec succès !");
        }





        #endregion

        #region MODULE STATISTIQUE 

        static void ModuleStatistique()
        {
            while (true)
            {
                Console.WriteLine("\n===== Module Statistiques =====");
                Console.WriteLine("1. Afficher le nombre de livraisons par cuisinier");
                Console.WriteLine("2. Afficher les commandes selon une période");
                Console.WriteLine("3. Afficher la moyenne des prix des commandes");
                Console.WriteLine("4. Afficher la moyenne des comptes clients");
                Console.WriteLine("5. Afficher les commandes par nationalité et période");
                Console.WriteLine("6. Retour au menu principal");
                Console.Write("Choix : ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherNombreLivraisonsParCuisinier();
                        break;
                    case "2":
                        AfficherCommandesParPeriode();
                        break;
                    case "3":
                        AfficherMoyennePrixCommandes();
                        break;
                    case "4":
                        AfficherMoyenneComptesClients();
                        break;
                    case "5":
                        AfficherCommandesParNationaliteEtPeriode();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Choix invalide, veuillez réessayer.");
                        break;
                }
            }
        }

        static void AfficherNombreLivraisonsParCuisinier()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteNbLivraison = "SELECT Commande.idUtilisateur, nom, prenom, COUNT(idCommande) AS nombre_livraisons FROM Cuisinier c" +
                    " LEFT JOIN Commande using(idUtilisateur) JOIN Utilisateur using(idUtilisateur)" +
                    "GROUP BY c.idUtilisateur, nom, prenom;";

                using (MySqlCommand cmd = new MySqlCommand(requeteNbLivraison, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n===== Nombre de livraisons par cuisinier =====");
                        while (reader.Read())
                        {
                            Console.WriteLine("Cuisinier: " + reader["nom"] + " " + reader["prenom"] + ", Livraisons: " + reader["nombre_livraisons"]);
                        }
                    }
                }
            }
        }

        static void AfficherCommandesParPeriode()
        {
            Console.Write("Date de début (YYYY-MM-DD) : ");
            string dateDebut = Console.ReadLine();
            Console.Write("Date de fin (YYYY-MM-DD) : ");
            string dateFin = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteAfficheCommandeP = "SELECT idCommande, idUtilisateur, dateCommande FROM Commande " +
                               "WHERE dateCommande BETWEEN @dateDebut AND @dateFin";

                using (MySqlCommand cmd = new MySqlCommand(requeteAfficheCommandeP, connection))
                {
                    cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n===== Commandes entre " + dateDebut + " et " + dateFin + " =====");
                        while (reader.Read())
                        {
                            Console.WriteLine("Commande: " + reader["idCommande"] + ", Client: " + reader["idUtilisateur"] + ", Date: " + reader["dateCommande"]);
                        }
                    }
                }
            }
        }

        static void AfficherMoyennePrixCommandes()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteMoyennePC = "SELECT IFNULL(AVG(prix), 0) AS moyennePrix FROM Commande"; // Remplace NULL par 0

                using (MySqlCommand cmd = new MySqlCommand(requeteMoyennePC, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Console.WriteLine("Moyenne des prix des commandes : " + reader["moyennePrix"] + "E");
                }
            }
        }

        static void AfficherMoyenneComptesClients()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteMoyenneCC = "SELECT IFNULL(AVG(soldeCompte), 0) AS moyenneComptes FROM Client"; // idem ça remplace NULL par 0

                using (MySqlCommand cmd = new MySqlCommand(requeteMoyenneCC, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Console.WriteLine("Moyenne des soldes des comptes clients : " + reader["moyenneComptes"] + "E");
                }
            }
        }

        static void AfficherCommandesParNationaliteEtPeriode()
        {
            Console.Write("Nationalité des plats (ex: Français, Italien) : ");
            string nationalite = Console.ReadLine();
            Console.Write("Date de début (YYYY-MM-DD) : ");
            string dateDebut = Console.ReadLine();
            Console.Write("Date de fin (YYYY-MM-DD) : ");
            string dateFin = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string requeteAffichCommande_NatPerio = "SELECT idCommande, Commande.idUtilisateur, dateCommande, nomPlat, nationalite " +
                               "FROM Commande " +
                               "JOIN Plat using(idPlat) " +
                               "WHERE nationalite = @nationalite " +
                               "AND dateCommande BETWEEN @dateDebut AND @dateFin";

                using (MySqlCommand cmd = new MySqlCommand(requeteAffichCommande_NatPerio, connection))
                {
                    cmd.Parameters.AddWithValue("@nationalite", nationalite);
                    cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                    cmd.Parameters.AddWithValue("@dateFin", dateFin);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n===== Commandes de plats " + nationalite + " entre " + dateDebut + " et " + dateFin + " =====");
                        while (reader.Read())
                        {
                            Console.WriteLine("Commande: " + reader["idCommande"] + ", Client: " + reader["idClient"] + ", Date: " + reader["dateCommande"] + ", Plat: " + reader["nomPlat"]);
                        }
                    }
                }
            }
        }



        #endregion


        #region MODULE AUTRE

        public static void ModuleAutre()
        {
            Console.WriteLine("\n=== MODULE AUTRE ===");
            Console.WriteLine("1. Afficher les stations");
            Console.WriteLine("2. Afficher la dernière commande passée");
            Console.WriteLine("3. Afficher le prix moyen des commandes");
            Console.WriteLine("4. Afficher les clients ayant commandé plus de X fois");
            Console.WriteLine("5. Afficher le nombre total de plats différents commandés");

            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherStations();
                    break;
                case "2":
                    AfficherDerniereCommande();
                    break;
                case "3":
                    AfficherPrixMoyenCommandes();
                    break;
                case "4":
                    AfficherClientsCommandesXfois();
                    break;
                case "5":
                    AfficherNombrePlatsDifferentsCommandes();
                    break;
                default:
                    Console.WriteLine("Option invalide.");
                    break;
            }
        }

        public static void AfficherStations()
        {
            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> g1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            for (int i = 0; i < g1.Graph.Length; i++)
            {
                Console.Write("Id :" + g1.Graph[i].Station.Id + " Nom :" + g1.Graph[i].Station.Nom + " Lignes : ");
                for (int j = 0; j < g1.Graph[i].Station.Lignes.Length; j++)
                {
                    Console.Write(g1.Graph[i].Station.Lignes[j] + ",");
                }
                Console.WriteLine();
            }
        }

        public static void AfficherDerniereCommande()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string requeteAfficherDerniereCommande = "SELECT idCommande, idUtilisateur, idPlat, dateCommande, prix FROM Commande ORDER BY dateCommande DESC LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(requeteAfficherDerniereCommande, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("Dernière commande :");
                    Console.WriteLine("ID : " + reader["idCommande"]);
                    Console.WriteLine("Client : " + reader["idUtilisateur"]);
                    Console.WriteLine("Plat : " + reader["idPlat"]);
                    Console.WriteLine("Date : " + reader["dateCommande"]);
                    Console.WriteLine("Prix : " + reader["prix"]);
                }
                else
                {
                    Console.WriteLine("Aucune commande trouvée.");
                }

                reader.Close();
            }
        }

        public static void AfficherPrixMoyenCommandes()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string requetePrixMoyen = "SELECT AVG(prix) FROM Commande";
                MySqlCommand cmd = new MySqlCommand(requetePrixMoyen, connection);
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    double prixMoyen = Convert.ToDouble(result);
                    Console.WriteLine("Prix moyen des commandes : " + prixMoyen.ToString("0.00") + " E");
                }
                else
                {
                    Console.WriteLine("Aucune commande enregistrée.");
                }
            }
        }
         
        public static void AfficherClientsCommandesXfois()
        {
            Console.Write("Entrez le nombre minimum de commandes à afficher (X) : ");
            int x = int.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string requeteAfficheXClient = "SELECT idUtilisateur, COUNT(*) as nbCommandes FROM Commande GROUP BY idUtilisateur HAVING nbCommandes > @x";
                MySqlCommand cmd = new MySqlCommand(requeteAfficheXClient, connection);
                cmd.Parameters.AddWithValue("@x", x);
                MySqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("Clients ayant commandé plus de " + x + " fois :");
                while (reader.Read())
                {
                    Console.WriteLine("Client ID : " + reader["idUtilisateur"] + " - Commandes : " + reader["nbCommandes"]);
                }

                reader.Close();
            }
        }

        public static void AfficherNombrePlatsDifferentsCommandes()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string requeteAffichDiffPlat = "SELECT COUNT(DISTINCT idPlat) FROM Commande";
                MySqlCommand cmd = new MySqlCommand(requeteAffichDiffPlat, connection);
                object result = cmd.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    int nbPlats = Convert.ToInt32(result);
                    Console.WriteLine("Nombre total de plats différents commandés : " + nbPlats);
                }
                else
                {
                    Console.WriteLine("Aucune commande trouvée.");
                }
            }
        }




        #endregion




    }
}

