using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Demo1
{
    class Program
    {
        private static ConsoleApp MaConsole = new ConsoleApp();
        private static WebClient WebClient1 = new WebClient { Encoding = Encoding.UTF8 };

        static void Main(string[] args)
        {
            MaConsole.Presentation();

            while (!MaConsole.Exit())
            {
                MaConsole.GetCommande();
                switch (MaConsole.Commande)
                {
                    case "Test-Connection": if (TestConnection()) MaConsole.Afficher("Ok"); else MaConsole.Afficher("Erreur"); break;
                    case "Get-Person": var ps = GetPerson(); if (ps.Count > 0) MaConsole.AfficherPersonne(ps); else MaConsole.Afficher("Aucune personne"); break;
                    case "Update-Person": if (UpdatePerson()) MaConsole.Afficher("Ok"); else MaConsole.Afficher("Erreur"); break;
                    case "Insert-Person": if (InsertPerson()) MaConsole.Afficher("Ok"); else MaConsole.Afficher("Erreur"); break;
                    case "Delete-Person": if (DeletePerson()) MaConsole.Afficher("Ok"); else MaConsole.Afficher("Erreur"); break;
                }
            }
        }

        private static bool UpdatePerson()
        {
            string url = $"http://localhost:51092/api/personne/?id={MaConsole.Id}&nom={MaConsole.Nom}&ville={MaConsole.Ville}";
            var octets = WebClient1.UploadData(url, "POST", new byte[] { });
            var s = Encoding.Default.GetString(octets);
            return s == @"""Ok""";
        }
        private static bool InsertPerson()
        {
            string url = $"http://localhost:51092/api/personne/?nom={MaConsole.Nom}&ville={MaConsole.Ville}";
            var octets = WebClient1.UploadData(url, "POST", new byte[] { });
            var s = Encoding.Default.GetString(octets);
            return s == @"""Ok""";
        }
        private static bool DeletePerson()
        {
            string url = $"http://localhost:51092/api/personne/{MaConsole.Id}";
            var octets = WebClient1.UploadData(url, "POST", new byte[] { });
            var s = Encoding.Default.GetString(octets);
            return s == @"""Ok""";
        }

        private static List<Personne> GetPerson()
        {
            string url; byte[] octets; string s;
            url = "http://localhost:51092/api/personne/?ville=tout";
            octets = WebClient1.DownloadData(url);
            s = Encoding.Default.GetString(octets);
            var liste = JsonConvert.DeserializeObject<List<Personne>>(s);
            return liste;
        }

        private static bool TestConnection()
        {
            WebClient WebClient1 = new WebClient { Encoding = Encoding.UTF8 };
            string url; byte[] octets; string s;
            url = "http://localhost:51092/api/personne";
            octets = WebClient1.DownloadData(url);
            s = Encoding.Default.GetString(octets);
            var liste = JsonConvert.DeserializeObject<List<string>>(s);

            return liste[0] == "value1" && liste[1] == "value2";
        }
    }
    class ConsoleApp
    {
        public string Prompt = "1> ";
        public string LigneCommande = "";

        // Regex
        public string Commande = null;
        public int Id;
        public string Nom = null;
        public string Ville = null;

        public ConsoleColor Couleur1 = ConsoleColor.Gray, Couleur2 = ConsoleColor.Yellow;
        private Regex Reg = new Regex(@"(?'command'^[A-Z][a-z,0-9]*-[A-Z][a-z,0-9]*)( (-Id) (?'id'\d+))?( (-Nom) ""?(?'nom'[\w,\s]+)""?)?( (-Ville) ""?(?'ville'[\w,\s]+)""?)?");

        public ConsoleApp()
        {
            Console.ForegroundColor = Couleur1;
        }
        internal void Afficher(string message)
        {
            Console.Write(Prompt);
            Console.ForegroundColor = Couleur2;
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ForegroundColor = Couleur1;
            Console.Write(Prompt);
        }

        internal void AfficherPersonne(List<Personne> ps)
        {
            Console.ForegroundColor = Couleur2;
            foreach (var p in ps) { Console.WriteLine(p); }
            Console.WriteLine();
            Console.ForegroundColor = Couleur1;
            Console.Write(Prompt);

        }

        internal bool Exit()
        {
            return LigneCommande == "Exit-Application";
        }

        internal void GetCommande()
        {
            LigneCommande = Console.ReadLine();
            var res = Reg.Match(LigneCommande);

            //if(res.Groups.Count == 1)
            //{
            //    var reg2 = new Regex("(?'Command'^[A-Z][a-z,0-9]*-[A-Z][a-z,0-9]*)");
            //    res = reg2.Match(LigneCommande);
            //}
            // Update-Person -Id 1 -Nom "Alex Le Grand" -Ville "Aix en Provence"
            for (int i = 1; i < res.Groups.Count; i++)
            {
                var group = res.Groups[i];
                if (! string.IsNullOrEmpty(group.Value))
                {
                    switch (group.Name)
                    {
                        case "id": Id = int.Parse(group.Value); break;
                        case "nom": Nom = group.Value; break;
                        case "ville": Ville = group.Value; break;
                        case "command": Commande = group.Value; break;
                    }
                }
            }
        }

        internal void Presentation()
        {
            Console.ForegroundColor = Couleur2;
            Console.WriteLine("Application Personne. Copyright Ali MAKRI");
            Console.WriteLine();
            Console.ForegroundColor = Couleur1;
            Console.Write(Prompt);
        }

    }
}
