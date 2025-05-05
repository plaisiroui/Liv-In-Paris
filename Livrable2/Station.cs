using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable2
{
    public class Station
    {
        int id;
        string nom;
        double longitude;
        double latititude;       
        int[] lignes;

        /// <summary>
        /// Constructeur qui va donner pour chaque station, son identifiant, son nom, sa longitude, 
        /// sa latitude et les lignes de métro associées
        /// </summary>
        /// <param name="id">Identifiant de la station</param>
        /// <param name="stations">Chemin vers le fichier des stations</param>
        public Station(int id, string stations) 
        {                                
            string[] m = File.ReadAllText(stations).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] n = (m[id - 1]).Split(new[] { ';' });
            string[] o = (n[4]).Split(new[] { ',' });

            int[] p = new int[o.Length];
            for (int i = 0 ; i < o.Length; i++)
            {
                p[i] = int.Parse(o[i]);
            }
            this.id = id;
            this.nom = n[1];
            this.longitude = double.Parse(n[2]);
            this.latititude = double.Parse(n[3]);
            this.lignes = p;
        }
        public int Id
        {
            get { return id; }
        }
        public string Nom
        {
            get { return nom; }
        }
        public double Latitude
        {
            get { return latititude; }
        }
        public double Longitude
        {
            get { return longitude; }
        }
        public int[] Lignes
        {
            get { return lignes; }
        }
    }
}
