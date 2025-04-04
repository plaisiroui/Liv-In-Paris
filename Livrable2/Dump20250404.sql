CREATE DATABASE  IF NOT EXISTS `livinparis` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `livinparis`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: livinparis
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `client`
--

DROP TABLE IF EXISTS `client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `client` (
  `idUtilisateur` varchar(30) NOT NULL,
  `typeClient` varchar(50) DEFAULT NULL,
  `soldeCompte` decimal(8,2) DEFAULT '0.00',
  PRIMARY KEY (`idUtilisateur`),
  CONSTRAINT `client_ibfk_1` FOREIGN KEY (`idUtilisateur`) REFERENCES `utilisateur` (`idUtilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client`
--

LOCK TABLES `client` WRITE;
/*!40000 ALTER TABLE `client` DISABLE KEYS */;
INSERT INTO `client` VALUES ('U001','Particulier',0.00);
/*!40000 ALTER TABLE `client` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commande` (
  `idCommande` varchar(15) NOT NULL,
  `dateCommande` date DEFAULT NULL,
  `prix` decimal(10,2) DEFAULT NULL,
  `statut` varchar(10) DEFAULT NULL,
  `idUtilisateur` varchar(50) NOT NULL,
  `idPlat` varchar(15) DEFAULT NULL,
  `idStationDepart` int NOT NULL,
  `idStationArrivee` int NOT NULL,
  PRIMARY KEY (`idCommande`),
  KEY `Commande_ibfk_1` (`idUtilisateur`),
  KEY `fk_commande_plat` (`idPlat`),
  CONSTRAINT `Commande_ibfk_1` FOREIGN KEY (`idUtilisateur`) REFERENCES `utilisateur` (`idUtilisateur`),
  CONSTRAINT `fk_commande_plat` FOREIGN KEY (`idPlat`) REFERENCES `plat` (`idPlat`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES ('C001','2024-03-01',25.50,'En cours','U001',NULL,0,0),('C002','2025-04-04',13.00,'En cours','U001',NULL,66,1),('C003','2025-04-04',19.00,'En cours','U001',NULL,66,1),('C004','2025-04-04',13.00,'En cours','U001',NULL,66,1);
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `composer`
--

DROP TABLE IF EXISTS `composer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `composer` (
  `idIngredient` varchar(50) NOT NULL,
  `idRecette` varchar(20) NOT NULL,
  PRIMARY KEY (`idIngredient`,`idRecette`),
  KEY `idRecette` (`idRecette`),
  CONSTRAINT `composer_ibfk_1` FOREIGN KEY (`idIngredient`) REFERENCES `ingredient` (`idIngredient`),
  CONSTRAINT `composer_ibfk_2` FOREIGN KEY (`idRecette`) REFERENCES `recette` (`idRecette`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `composer`
--

LOCK TABLES `composer` WRITE;
/*!40000 ALTER TABLE `composer` DISABLE KEYS */;
INSERT INTO `composer` VALUES ('I001','R001'),('I002','R001'),('I003','R001'),('I004','R001');
/*!40000 ALTER TABLE `composer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contenir`
--

DROP TABLE IF EXISTS `contenir`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contenir` (
  `idCommande` varchar(15) NOT NULL,
  `idPlat` varchar(15) NOT NULL,
  PRIMARY KEY (`idCommande`,`idPlat`),
  KEY `idPlat` (`idPlat`),
  CONSTRAINT `contenir_ibfk_1` FOREIGN KEY (`idCommande`) REFERENCES `commande` (`idCommande`),
  CONSTRAINT `contenir_ibfk_2` FOREIGN KEY (`idPlat`) REFERENCES `plat` (`idPlat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contenir`
--

LOCK TABLES `contenir` WRITE;
/*!40000 ALTER TABLE `contenir` DISABLE KEYS */;
INSERT INTO `contenir` VALUES ('C001','P001');
/*!40000 ALTER TABLE `contenir` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisinier`
--

DROP TABLE IF EXISTS `cuisinier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuisinier` (
  `idUtilisateur` varchar(30) NOT NULL,
  `specialite` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`idUtilisateur`),
  CONSTRAINT `cuisinier_ibfk_1` FOREIGN KEY (`idUtilisateur`) REFERENCES `utilisateur` (`idUtilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier`
--

LOCK TABLES `cuisinier` WRITE;
/*!40000 ALTER TABLE `cuisinier` DISABLE KEYS */;
INSERT INTO `cuisinier` VALUES ('U002',NULL),('U004','pate'),('U005','Pizza');
/*!40000 ALTER TABLE `cuisinier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `evaluation`
--

DROP TABLE IF EXISTS `evaluation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `evaluation` (
  `idEvaluation` varchar(15) NOT NULL,
  `idEvaluateur` varchar(50) NOT NULL,
  `idEvalue` varchar(50) NOT NULL,
  `typeEvaluation` enum('Client','Cuisinier','Plat') DEFAULT NULL,
  `note` int DEFAULT NULL,
  `commentaire` varchar(200) DEFAULT NULL,
  `dateEvaluation` date DEFAULT NULL,
  PRIMARY KEY (`idEvaluation`),
  KEY `idEvaluateur` (`idEvaluateur`),
  KEY `idEvalue` (`idEvalue`),
  CONSTRAINT `evaluation_ibfk_1` FOREIGN KEY (`idEvaluateur`) REFERENCES `utilisateur` (`idUtilisateur`),
  CONSTRAINT `evaluation_ibfk_2` FOREIGN KEY (`idEvalue`) REFERENCES `utilisateur` (`idUtilisateur`),
  CONSTRAINT `evaluation_chk_1` CHECK ((`note` between 1 and 5))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `evaluation`
--

LOCK TABLES `evaluation` WRITE;
/*!40000 ALTER TABLE `evaluation` DISABLE KEYS */;
INSERT INTO `evaluation` VALUES ('E001','U001','U002','Cuisinier',5,'C\"etait vraiment excellent','2024-03-02');
/*!40000 ALTER TABLE `evaluation` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `idIngredient` varchar(50) NOT NULL,
  `nomIngredient` varchar(50) NOT NULL,
  `typeIngredient` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`idIngredient`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES ('I001','Oignons','Légume'),('I002','Poulet','Viande'),('I003','Poivrons','Légume'),('I004','Huile','Matière grasse');
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `livraison`
--

DROP TABLE IF EXISTS `livraison`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `livraison` (
  `idLivraison` varchar(50) NOT NULL,
  `idCommande` varchar(15) NOT NULL,
  `dateLivraison` date DEFAULT NULL,
  PRIMARY KEY (`idLivraison`),
  UNIQUE KEY `idCommande` (`idCommande`),
  CONSTRAINT `livraison_ibfk_1` FOREIGN KEY (`idCommande`) REFERENCES `commande` (`idCommande`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `livraison`
--

LOCK TABLES `livraison` WRITE;
/*!40000 ALTER TABLE `livraison` DISABLE KEYS */;
INSERT INTO `livraison` VALUES ('L001','C001','2024-03-01');
/*!40000 ALTER TABLE `livraison` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plat` (
  `idPlat` varchar(15) NOT NULL,
  `nomPlat` varchar(50) DEFAULT NULL,
  `nbPersonne` int DEFAULT NULL,
  `typePlat` varchar(50) DEFAULT NULL,
  `nationalite` varchar(50) DEFAULT NULL,
  `regAlimentaire` varchar(50) DEFAULT NULL,
  `prix` decimal(10,2) DEFAULT NULL,
  `photo` varchar(256) DEFAULT NULL,
  `dateFabrication` date DEFAULT NULL,
  `datePeremption` date DEFAULT NULL,
  `idRecette` varchar(20) NOT NULL,
  `idUtilisateur` varchar(50) NOT NULL,
  `platDuJour` tinyint DEFAULT '0',
  PRIMARY KEY (`idPlat`),
  KEY `idRecette` (`idRecette`),
  KEY `Plat_ibfk_2` (`idUtilisateur`),
  CONSTRAINT `plat_ibfk_1` FOREIGN KEY (`idRecette`) REFERENCES `recette` (`idRecette`),
  CONSTRAINT `Plat_ibfk_2` FOREIGN KEY (`idUtilisateur`) REFERENCES `utilisateur` (`idUtilisateur`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES ('P001','Fajitas',4,'Plat principal','Mexicain','Non végétarien',12.00,NULL,'2024-03-01','2024-03-05','R001','U002',0);
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recette`
--

DROP TABLE IF EXISTS `recette`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recette` (
  `idRecette` varchar(20) NOT NULL,
  `nomRecette` varchar(50) NOT NULL,
  `tempsPreparation` int DEFAULT NULL,
  `descriptionRecette` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`idRecette`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recette`
--

LOCK TABLES `recette` WRITE;
/*!40000 ALTER TABLE `recette` DISABLE KEYS */;
INSERT INTO `recette` VALUES ('R001','Fajitas maison au poulet',30,'Si vous avez une poêle à fajitas en fonte, la placer dans le four et préchauffer le four à 200 °C (400 °F). Sinon, prévoir une assiette de service pour la préparation à fajitas chaude.\n\nDans une grande poêle à feu moyen, dorer les poivrons dans 15 ml (1 c. à soupe) de l’huile. Saler et poivrer. Réserver sur une assiette.\n\nDans la même poêle, caraméliser les oignons dans 15 ml (1 c. à soupe) d’huile et le beurre. Saler et poivrer. Réserver avec les poivrons.\n\nDans la même poêle, dorer le poulet dans le reste de l’huile avec les épices. Saler et poivrer. Remettre les légumes réservés dans la poêle. Poursuivre la cuisson 1 minute en remuant. Rectifier l’assaisonnement.\n\nRetirer la poêle du four. Y verser la préparation à fajitas. Servir immédiatement au centre de la table avec les garnitures placées dans des bols séparés. Laisser chacun garnir ses tortillas.');
/*!40000 ALTER TABLE `recette` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `utilisateur` (
  `idUtilisateur` varchar(30) NOT NULL,
  `adresseMail` varchar(50) NOT NULL,
  `nom` varchar(50) DEFAULT NULL,
  `prenom` varchar(50) DEFAULT NULL,
  `adresse` varchar(150) DEFAULT NULL,
  `telephone` varchar(12) DEFAULT NULL,
  `password` varchar(50) DEFAULT NULL,
  `StationLaPlusProche` int DEFAULT NULL,
  PRIMARY KEY (`idUtilisateur`),
  UNIQUE KEY `adresseMail` (`adresseMail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO `utilisateur` VALUES ('U001','mateo.dubreuil@edu.devinci.fr','Mateo','Dubreuil','13 avenue Marceau','0666954935','mateo!',1),('U002','tom.gaffet@edu.devinci.fr','Tom','Gaffet','34 avenue Leonard de Vinci','0698765432','TomTom',20),('U003','rtyui@gmail.com','Max','ugo','ugo avenue charles','0555555552','baguette',40),('U004','azergh','Tom','Alam','45 azerg','azertyhj','baguette',66),('U005','ghjklm@gmail.com','Romain','Hugo','78 rue de clamart','0569587423','123456hug',109);
/*!40000 ALTER TABLE `utilisateur` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-04 13:02:40
