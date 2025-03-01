-- create database livinparis;
use livinparis;
CREATE TABLE Utilisateur (
   idUtilisateur VARCHAR(30),
   adresseMail VARCHAR(50) UNIQUE NOT NULL,
   nom VARCHAR(50),
   prenom VARCHAR(50),
   adresse VARCHAR(150),
   telephone VARCHAR(12),
   password VARCHAR(50),
   PRIMARY KEY(idUtilisateur)
);

CREATE TABLE Client (
   idUtilisateur VARCHAR(30) NOT NULL,
   typeClient VARCHAR(50),
   PRIMARY KEY(idUtilisateur),
   FOREIGN KEY(idUtilisateur) REFERENCES Utilisateur(idUtilisateur)
);

CREATE TABLE Cuisinier (
   idUtilisateur VARCHAR(30) NOT NULL,
   PRIMARY KEY(idUtilisateur),
   FOREIGN KEY(idUtilisateur) REFERENCES Utilisateur(idUtilisateur)
);

CREATE TABLE AdresseLivraison (
   idAdresse VARCHAR(50),
   idClient VARCHAR(50) NOT NULL,
   adresse VARCHAR(150),
   ville VARCHAR(50),
   codePostal VARCHAR(10),
   PRIMARY KEY(idAdresse),
   FOREIGN KEY(idClient) REFERENCES Client(idUtilisateur)
);

CREATE TABLE Commande (
   idCommande VARCHAR(15),
   dateCommande DATE,
   prix DECIMAL(10,2),
   statut VARCHAR(10),
   idClient VARCHAR(50) NOT NULL,
   idAdresseLivraison VARCHAR(50) NOT NULL,
   PRIMARY KEY(idCommande),
   FOREIGN KEY(idClient) REFERENCES Client(idUtilisateur),
   FOREIGN KEY(idAdresseLivraison) REFERENCES AdresseLivraison(idAdresse)
);

CREATE TABLE Livraison (
   idLivraison VARCHAR(50),
   idCommande VARCHAR(15) UNIQUE NOT NULL,
   dateLivraison DATE,
   PRIMARY KEY(idLivraison),
   FOREIGN KEY(idCommande) REFERENCES Commande(idCommande)
);

CREATE TABLE Recette (
   idRecette VARCHAR(20),
   nomRecette VARCHAR(50) NOT NULL,
   tempsPreparation INT,
   description VARCHAR(500),
   PRIMARY KEY(idRecette)
);

CREATE TABLE Plat (
   idPlat VARCHAR(15),
   nomPlat VARCHAR(50),
   nbPersonne INT,
   typePlat VARCHAR(50),
   nationalite VARCHAR(50),
   regAlimentaire VARCHAR(50),
   prix DECIMAL(10,2),
   photo VARCHAR(256),
   dateFabrication DATE,
   datePeremption DATE,
   idRecette VARCHAR(20) NOT NULL,
   idCuisinier VARCHAR(50) NOT NULL,
   PRIMARY KEY(idPlat),
   FOREIGN KEY(idRecette) REFERENCES Recette(idRecette),
   FOREIGN KEY(idCuisinier) REFERENCES Cuisinier(idUtilisateur)
);

CREATE TABLE Ingredient (
   idIngredient VARCHAR(50),
   nomIngredient VARCHAR(50) NOT NULL,
   type VARCHAR(50),
   PRIMARY KEY(idIngredient)
);

CREATE TABLE Composer (
   idIngredient VARCHAR(50),
   idRecette VARCHAR(20),
   PRIMARY KEY(idIngredient, idRecette),
   FOREIGN KEY(idIngredient) REFERENCES Ingredient(idIngredient),
   FOREIGN KEY(idRecette) REFERENCES Recette(idRecette)
);


CREATE TABLE Contenir (
   idCommande VARCHAR(15),
   idPlat VARCHAR(15),
   PRIMARY KEY(idCommande, idPlat),
   FOREIGN KEY(idCommande) REFERENCES Commande(idCommande),
   FOREIGN KEY(idPlat) REFERENCES Plat(idPlat)
);

CREATE TABLE Evaluation (
   idEvaluation VARCHAR(15),
   idEvaluateur VARCHAR(50) NOT NULL,
   idEvalue VARCHAR(50) NOT NULL,
   typeEvaluation ENUM('Client', 'Cuisinier', 'Plat'),
   note INT CHECK (note BETWEEN 1 AND 5),
   commentaire VARCHAR(200),
   dateEvaluation DATE,
   PRIMARY KEY(idEvaluation),
   FOREIGN KEY(idEvaluateur) REFERENCES Utilisateur(idUtilisateur),
   FOREIGN KEY(idEvalue) REFERENCES Utilisateur(idUtilisateur)
);
