
INSERT INTO Utilisateur VALUES ('U001', 'mateo.dubreuil@edu.devinci.fr', 'Mateo', 'Dubreuil', '13 avenue Marceau', '0768554456', 'mateo!');
INSERT INTO Utilisateur VALUES ('U002', 'tom.gaffet@edu.devinci.fr', 'Tom', 'Gaffet', '34 avenue Leonard de Vinci', '0698765432', 'TomTom');

INSERT INTO Client VALUES ('U001', 'Particulier');

INSERT INTO Cuisinier VALUES ('U002');

INSERT INTO AdresseLivraison VALUES ('A001', 'U001', '12 avenue Leonard De Vinci', 'Courbevoie', '92400');

INSERT INTO Commande VALUES ('C001', '2024-03-01', 25.50, 'En cours', 'U001', 'A001');

INSERT INTO Livraison VALUES ('L001', 'C001', '2024-03-01');

INSERT INTO Recette VALUES ('R001', 'Fajitas maison au poulet', 30, 'Si vous avez une poêle à fajitas en fonte, la placer dans le four et préchauffer le four à 200 °C (400 °F). Sinon, prévoir une assiette de service pour la préparation à fajitas chaude.

Dans une grande poêle à feu moyen, dorer les poivrons dans 15 ml (1 c. à soupe) de l’huile. Saler et poivrer. Réserver sur une assiette.

Dans la même poêle, caraméliser les oignons dans 15 ml (1 c. à soupe) d’huile et le beurre. Saler et poivrer. Réserver avec les poivrons.

Dans la même poêle, dorer le poulet dans le reste de l’huile avec les épices. Saler et poivrer. Remettre les légumes réservés dans la poêle. Poursuivre la cuisson 1 minute en remuant. Rectifier l’assaisonnement.

Retirer la poêle du four. Y verser la préparation à fajitas. Servir immédiatement au centre de la table avec les garnitures placées dans des bols séparés. Laisser chacun garnir ses tortillas.');

INSERT INTO Ingredient VALUES ('I001', 'Oignons', 'Légume');
INSERT INTO Ingredient VALUES ('I002', 'Poulet', 'Viande');
INSERT INTO Ingredient VALUES ('I003', 'Poivrons', 'Légume');
INSERT INTO Ingredient VALUES ('I004', 'Huile', 'Matière grasse');

INSERT INTO Composer VALUES ('I001', 'R001');
INSERT INTO Composer VALUES ('I002', 'R001');
INSERT INTO Composer VALUES ('I003', 'R001');
INSERT INTO Composer VALUES ('I004', 'R001');

INSERT INTO Plat VALUES ('P001', 'Fajitas', 4, 'Plat principal', 'Mexicain', 'Non végétarien', 10, null, '2024-03-01', '2024-03-05', 'R001', 'U002');


INSERT INTO Contenir VALUES ('C001', 'P001');

INSERT INTO Evaluation VALUES ('E001', 'U001', 'U002', 'Cuisinier', 5, 'Excellent!!! A refaire assurément!!', '2024-03-02');
