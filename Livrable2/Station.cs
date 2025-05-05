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

        public Station(int id, string stations) ///Ici, on va lire la ligne (qui correspond à l'identifiant de la station concernée) 
        {                                       ///pour extraire les informations de la station

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
        public Station(int id, string nom, double latititude, double longitude, int[] lignes)
        {
            this.id = id;
            this.nom = nom;
            this.latititude = latititude;
            this.longitude = longitude;
            this.lignes = lignes;
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
