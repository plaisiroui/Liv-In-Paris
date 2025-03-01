SELECT * FROM Client;
SELECT * FROM Cuisinier;
SELECT * FROM Commande;
SELECT * FROM Plat;
SELECT * FROM Evaluation;

update Utilisateur set telephone = '0666954935' WHERE idUtilisateur = 'U001';
update AdresseLivraison set adresse = '13 Avenue Leonard de Vinci' WHERE idAdresse = 'A001';
Update plat set prix =12 where idPlat='P001';
update evaluation set commentaire='C"etait vraiment excellent' where idEvaluation='E001';

INSERT INTO AdresseLivraison VALUES ('A002', 'U001', '12 avenue des champs Elysee', 'Paris', '75000');
delete from AdresseLivraison where idAdresse='A002';


