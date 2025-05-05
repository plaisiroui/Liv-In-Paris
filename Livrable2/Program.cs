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
        /// <summary>
        /// sous menu pour gérer les opérations liées aux client (ajout, suppression etc...)
        /// </summary>
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
        /// <summary>
        /// Génère un id unique pour un nouvel utilisateur en s'assurant que celui ci n'existe pas déjà dans la BDD
        /// </summary>
        /// <returns>retourne un string qui represente un id unique de la forme U+ 3 chiffres (ex: "U001")</returns>
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
        /// <summary>
        /// Demande les informations d'un nouveau client à l'utilisateur, génère un idUnique
        /// puis insère ces données dans les tables Utilisateur et Client de la BDD
        /// </summary>
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
            while (telephone.Length != 10)
            {
                Console.WriteLine("Erreur, réessayer :");
                telephone = Console.ReadLine();
            }
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
            while (idstationPP == -1)
            {
                Console.WriteLine("Erreur, station mal écrite, réessayer :");
                stationPP = Console.ReadLine();
                idstationPP = grph1.NomAIdentifiant(stationPP);
            }


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

        /// <summary>
        /// Supprime un client et toutes ses données associées des autres tables.
        /// </summary>
        static void SupprimerClient()
        {

            Console.WriteLine("\n=== SUPPRESSION D'UN CLIENT ===");

            Console.Write("ID du client à supprimer (Taper A pour afficher les cliens d'abord) : ");
            string idClient = Console.ReadLine();
            if (idClient == "A")
            {
                AfficherClients();
                Console.Write("ID du client à supprimer : ");
                idClient = Console.ReadLine();
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string[] listRequete = {
                     "DELETE FROM Contenir WHERE idCommande IN (SELECT idCommande FROM Commande WHERE idUtilisateur = @id)",
                     "DELETE FROM Livraison WHERE idCommande IN (SELECT idCommande FROM Commande WHERE idUtilisateur = @id)",
                     "DELETE FROM Evaluation WHERE idEvaluateur = @id",
                     "DELETE FROM Evaluation WHERE idEvalue = @id",
                     "DELETE FROM Commande WHERE idUtilisateur = @id",
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

        /// <summary>
        /// Permet de modifier les infos perso d'un client déjà existant 
        /// -> met à jour les tables Utilisateurs et Client
        /// </summary>
        static void ModifierClient()
        {

            Console.WriteLine("\n=== MODIFICATION D'UN CLIENT ===");

            Console.Write("ID du client à modifier(Taper A pour afficher les cliens d'abord) : ");
            string id = Console.ReadLine();
            if (id == "A")
            {
                AfficherClients();
                Console.Write("ID du client à supprimer : ");
                id = Console.ReadLine();
            }
            Console.Write("Nouveau nom: ");
            string nom = Console.ReadLine();
            Console.Write("Nouveau prénom: ");
            string prenom = Console.ReadLine();
            Console.Write("Nouvelle adresse: ");
            string adresse = Console.ReadLine();
            Console.Write("Nouveau téléphone: ");
            string telephone = Console.ReadLine();
            while (telephone.Length != 10)
            {
                Console.WriteLine("Erreur, réessayer :");
                telephone = Console.ReadLine();
            }
            Console.Write("Nouveau type de client: ");
            string typeClient = Console.ReadLine();

            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Nouvelle station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);
            while (idstationPP == -1)
            {
                Console.WriteLine("Erreur, station mal écrite, réessayer :");
                stationPP = Console.ReadLine();
                idstationPP = grph1.NomAIdentifiant(stationPP);
            }

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
        /// <summary>
        /// Affiche liste des clients triée comme c'est demandé sru le cahier des charges
        /// </summary>
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
        /// <summary>
        /// Sous menu pour gérer les opérations liées aux cuisiniers
        /// </summary>
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
        /// <summary>
        /// Ajoute un nouveau cuisinier en demande les infos a l'utilisateur
        /// puis insère les données dans les tables Utilisateur et Cuisinier
        /// </summary>
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
            while (telephone.Length != 10)
            {
                Console.WriteLine("Erreur, réessayer :");
                telephone = Console.ReadLine();
            }
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
            while (idstationPP == -1)
            {
                Console.WriteLine("Erreur, station mal écrite, réessayer :");
                stationPP = Console.ReadLine();
                idstationPP = grph1.NomAIdentifiant(stationPP);
            }


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
        /// <summary>
        /// Permet de modifier les infos d'un cuisinier dèjà existant
        /// puis met a jour les tables utilisateur et Cuisinier
        /// </summary>
        static void ModifierCuisinier()
        {

            Console.WriteLine("\n=== MODIFICATION D'UN CUISINIER ===");

            Console.Write("ID du cuisinier à modifier: (Taper A d'abord pour afficher les cuisiniers ");
            string id = Console.ReadLine();
            if (id == "A")
            {
                AfficherCuisiniers();
                Console.Write("ID du cuisinier à modifier : ");
                id = Console.ReadLine();
            }
            Console.Write("Nouveau nom: ");
            string nom = Console.ReadLine();
            Console.Write("Nouveau prénom: ");
            string prenom = Console.ReadLine();
            Console.Write("Nouvelle adresse: ");
            string adresse = Console.ReadLine();
            Console.Write("Nouveau téléphone: ");
            string telephone = Console.ReadLine();
            while (telephone.Length != 10)
            {
                Console.WriteLine("Erreur, réessayer :");
                telephone = Console.ReadLine();
            }
            Console.Write("Nouvelle spécialité:  ");
            string specialite = Console.ReadLine();

            string fichier_connexion = "Connexions.txt";
            string fichier_StationMetro = "StationsMetro.txt";
            Graphe<int> grph1 = new Graphe<int>(fichier_connexion, fichier_StationMetro);

            Console.Write("Station la plus proche (Bien écrire, exemple: Balard) ");
            string stationPP = Console.ReadLine();
            int idstationPP = grph1.NomAIdentifiant(stationPP);
            while (idstationPP == -1)
            {
                Console.WriteLine("Erreur, station mal écrite, réessayer :");
                stationPP = Console.ReadLine();
                idstationPP = grph1.NomAIdentifiant(stationPP);
            }

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
        /// <summary>
        /// Supprimee un cuisinier et toutes ses données associées
        /// </summary>
        static void SupprimerCuisinier()
        {

            Console.WriteLine("\n=== SUPPRESSION D'UN CUISINIER ===");

            Console.Write("ID du cuisinier à supprimer (Tapez A d'abord pour afficher les cuisiniers ");
            string idCuisinier = Console.ReadLine();
            if (idCuisinier == "A")
            {
                AfficherCuisiniers();
                Console.Write("ID du cuisinier à supprimer : ");
                idCuisinier = Console.ReadLine();
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string[] requetesDeleteCuisi = {
    "DELETE FROM Contenir WHERE idCommande IN (SELECT idCommande FROM Commande WHERE idUtilisateur = @id)",
    "DELETE FROM Contenir WHERE idPlat IN (SELECT idPlat FROM Plat WHERE idUtilisateur = @id)",
    "DELETE FROM Livraison WHERE idCommande IN (SELECT idCommande FROM Commande WHERE idUtilisateur = @id)",
    "DELETE FROM Evaluation WHERE idEvaluateur = @id",
    "DELETE FROM Evaluation WHERE idEvalue = @id",
    "DELETE FROM Commande WHERE idUtilisateur = @id",
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
        /// <summary>
        /// Affiche la liste des clients ayant été servis par un cuisinier donné
        /// possibilité de filtrer par période
        /// </summary>
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
        /// <summary>
        /// Affiche les plats réalisés par un cuisinier, trés par fréquence de commande
        /// </summary>
        static void AfficherPlatsParFrequence()
        {
            Console.Write("ID du cuisinier (Tapez A d'abord pour afficher les cuisiniers ");
            string id = Console.ReadLine();
            if (id == "A")
            {
                AfficherCuisiniers();
                Console.Write("ID du cuisinier: ");
                id = Console.ReadLine();
            }

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
        /// <summary>
        /// Affiche le nom plat du jour s'il existe
        /// </summary>
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
/// <summary>
/// Affiche la liste des cuisiniers
/// </summary>
        static void AfficherCuisiniers()
        {
            Console.WriteLine("\n=== LISTE DES CUISINIERS ===");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string requeteAffichageCuisinier =
                    "SELECT Cuisinier.idUtilisateur, nom, prenom, adresse, telephone, adresseMail, specialite " +
                    "FROM Utilisateur JOIN Cuisinier USING(idUtilisateur);";

                using (MySqlCommand cmd = new MySqlCommand(requeteAffichageCuisinier, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            "ID: " + reader["idUtilisateur"] + ", Nom: " + reader["nom"] + " " + reader["prenom"] +
                            ", Adresse: " + reader["adresse"] + ", Téléphone: " + reader["telephone"] +
                            ", Email: " + reader["adresseMail"] + ", Spécialité: " + reader["specialite"]);
                    }
                }
            }
        }







        #endregion

        #region MODULE COMMANDE
        /// <summary>
        /// Menu des opérations sur les commandes
        /// </summary>
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

        /// <summary>
        /// Génère un id unique pour une nouvelle commande
        /// </summary>
        /// <returns>un id unique sous la forme "C" + 3 chiffres (par exemple "C001")</returns>
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
        /// <summary>
        /// Permert à un client de passer une commande en sélectionnant un cuisinier et un plat
        /// Enregistre la commande dans las BDD
        /// </summary>
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
                        Console.WriteLine("- " + idPlat + " : " + nomPlat + " (" + prix + " euros)");
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


        /// <summary>
        /// Affiche prux d'une commande
        /// </summary>
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

        /// <summary>
        /// Détermine et affiche le plus court chemin entre la station du cuisinier et celle du client
        /// pour une commande
        /// En utilisant un algorithme que l'on peu choisir (Dijkstra, Bellman-Ford ou Floyd-Warshall).
        /// </summary>
        public static void DeterminerTrajetOptimal()
        {
            Console.Write("Entrez le numéro de commande : ");
            string idCommande = Console.ReadLine();

            int stationDepart = -1;
            int stationArrivee = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string requeteRecupStations = "SELECT idStationDepart, idStationArrivee FROM Commande WHERE idCommande = '" + idCommande + "'";


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
                        g2.VisualiserGrapheAvecChemin(chemin);
                        break;

                    case "2":
                        chemin = g2.BellmanFord(stationDepart, stationArrivee);
                        Console.WriteLine("Algorithme utilisé : Bellman-Ford");
                        g2.VisualiserGrapheAvecChemin(chemin);
                        break;

                    case "3":
                        Console.WriteLine("Algorithme utilisé : Floyd-Warshall");
                        (double[,] mat, int[,] next) = g2.FloydWarshall();
                        chemin = g2.ReconstituerChemin(stationDepart, stationArrivee, next);
                        g2.VisualiserGrapheAvecChemin(chemin);
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
        /// <summary>
        /// Modifie les infos d'une commande
        /// </summary>
        public static void ModifierCommande()
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
        /// <summary>
        /// Génère un id unique pour un plat
        /// </summary>
        /// <returns> un id unique sous la forme "P" suivi de 3 chiffre (ex:"P001")</returns>
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

        /// <summary>
        ///¨Permet à un cuisinier d'ajouter un nouveau plat avec toutes les infos de celui ci
        /// </summary>
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
        /// <summary>
        /// Affiche le nombre total de livraisons effectuées par chaque cuisinier
        /// </summary>
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
        /// <summary>
        /// Affiche les commandes passées entre deux dates que l'utilisateur saisies
        /// </summary>
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
        /// <summary>
        /// affiche la moyenne des pric de toutes les commandes de la BDD
        /// </summary>
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
        /// <summary>
        /// Affiche la moyenne des soldes des comptes clients 
        /// </summary>
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
        /// <summary>
        /// Affiche les commandes de plats correspondant à une nationalité donnée
        /// et^passées dans une période définie
        /// </summary>
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
        /// <summary>
        /// Menu avec des reuêtes diverses
        /// +les requetes avec Having , group by etc....
        /// </summary>
        public static void ModuleAutre()
        {
            Console.WriteLine("\n=== MODULE AUTRE ===");
            Console.WriteLine("1. Afficher les stations");
            Console.WriteLine("2. Afficher la dernière commande passée");
            Console.WriteLine("3. Afficher le prix moyen des commandes");
            Console.WriteLine("4. Afficher les clients ayant commandé plus de X fois");
            Console.WriteLine("5. Afficher le nombre total de plats différents commandés");
            Console.WriteLine("6. Clients avec plus de 2 commandes (GROUP BY + HAVING)");
            Console.WriteLine("7. Clients sans commande (LEFT JOIN)");
            Console.WriteLine("8. Plats plus chers qu’au moins un plat du cuisinier choisi(ANY)");
            Console.WriteLine("9. Plats plus chers que ceux du cuisinier choisi(ALL)");
            Console.WriteLine("10. Cuisiniers avec un plat du jour (EXISTS)");


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
                case "6":
                    RequeteGroupByHaving();
                    break;
                case "7":
                    RequeteLeftJoin();
                    break;
                case "8":
                    RequeteAny();
                    break;
                case "9":
                    RequeteAll();
                    break;
                case "10":
                    RequeteExists();
                    break;
                default:
                    Console.WriteLine("Option invalide.");
                    break;
            }
        }
        /// <summary>
        /// Affiche la liste des stations de métro contenues dans le graphe
        /// </summary>
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
        /// <summary>
        /// Affiche les détails de la derniere commande passée, triée par date decroissante
        /// </summary>
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
        /// <summary>
        /// Calcule et affiche le prix moyen de toutes les commandes
        /// </summary>
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

        /// <summary>
        /// Affiche les clients ayant passé + de X commandes (X tapé par l'utilisateur au 
        /// </summary>
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
        /// <summary>
        /// Affiche le nbr total de plats différents ayant été commandés
        /// </summary>
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
        /// <summary>
        /// Affiche les clients ayant passé plus de deux commandes
        /// </summary>
        static void RequeteGroupByHaving()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string requeteGBH = "SELECT idUtilisateur, COUNT(*) AS nbCommandes from Commande  GROUP BY idUtilisateur HAVING COUNT(*) > 2;";

            using var cmd = new MySqlCommand(requeteGBH, conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Clients ayant passé plus de 2 commandes :");
            while (reader.Read())
            {
                Console.WriteLine("Client " + reader["idUtilisateur"] + " => " + reader["nbCommandes"] + " commandes");
            }
        }
        /// <summary>
        /// Affiche les utilisateurs qui n'ont jamais passé de commande
        /// </summary>
        static void RequeteLeftJoin()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string requeteLJ = "SELECT idUtilisateur, nom, prenom  FROM Utilisateur " +
                "  LEFT JOIN Commande using(idUtilisateur)" +
                " WHERE idCommande IS NULL;";

            using var cmd = new MySqlCommand(requeteLJ, conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Clients n’ayant jamais passé de commande :");
            while (reader.Read())
            {
                Console.WriteLine(reader["idUtilisateur"] + " – " + reader["nom"] + " " + reader["prenom"]);
            }
        }
        /// <summary>
        /// Affiche les plats dont le prix est supérieur à au moins un plat du cuisinier sélectionné
        /// </summary>
        static void RequeteAny()
        {
            Console.Write("Entrez l’ID du cuisinier à comparer (Tapez A pour voir les cuisiniers) : ");
            string idCuisinier = Console.ReadLine();
            if (idCuisinier == "A")
            {
                AfficherCuisiniers();
                Console.Write("ID du cuisinier à comparer: ");
                idCuisinier = Console.ReadLine();
            }

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string requeteAny = "SELECT idPlat, nomPlat, prix FROM Plat " +
                "WHERE prix > ANY ( SELECT prix FROM Plat WHERE idUtilisateur = @idCuisinier);";

            using var cmd = new MySqlCommand(requeteAny, conn);
            cmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Plats plus chers qu’au moins un plat du cuisinier " + idCuisinier + " :");
            while (reader.Read())
            {
                Console.WriteLine(reader["idPlat"] + " – " + reader["nomPlat"] + " (" + reader["prix"] + " eruos)");
            }
        }
        /// <summary>
        /// Affiche les plats dont le prix est supérieur à tous les plats d'un cuisinier
        /// </summary>
        static void RequeteAll()
        {
            Console.Write("Entrez l’ID du cuisinier à comparer (Tapez A pour voir les cuisiniers) : ");
            string idCuisinier = Console.ReadLine();
            if (idCuisinier == "A")
            {
                AfficherCuisiniers();
                Console.Write("ID du cuisinier à comparer: ");
                idCuisinier = Console.ReadLine();
            }
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string requeteALL = "SELECT idPlat, nomPlat, prix FROM Plat " +
                "WHERE prix > ALL ( SELECT prix FROM Plat WHERE idUtilisateur = @idCuisinier );";

            using var cmd = new MySqlCommand(requeteALL, conn);
            cmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Plats plus chers que TOUS ceux du cuisinier " + idCuisinier + " :");
            while (reader.Read())
            {
                Console.WriteLine(reader["idPlat"] + " – " + reader["nomPlat"] + " (" + reader["prix"] + " eruos)");
            }
        }
        /// <summary>
        /// Affiche les cuisiniers ayant au moins un plat 
        /// </summary>
        static void RequeteExists()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string requeteExists = @"SELECT u.idUtilisateur, nom, prenom  FROM Utilisateur u "+
                "WHERE EXISTS (SELECT * FROM Plat p WHERE p.idUtilisateur = u.idUtilisateur AND platDuJour = 1  );";

            using var cmd = new MySqlCommand(requeteExists, conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Cuisiniers avec au moins un plat du jour :");
            while (reader.Read())
            {
                Console.WriteLine(reader["idUtilisateur"] + " – " + reader["nom"] + " " + reader["prenom"]);
            }
        }









        #endregion




    }
}
