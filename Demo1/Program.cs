using DemoBatch.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
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
                    case "Upload-Person": if (UploadPerson()) MaConsole.Afficher("Ok"); else MaConsole.Afficher("Erreur"); break;
                }
            }
        }
        private static List<Personne> LectureExcel()
        {
            string connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={MaConsole.File};Extended Properties=""Excel 8.0;HDR=YES""";
            var connection = new OleDbConnection(connectionString);
            string cmdText = "SELECT * FROM [Feuil1$]";
            OleDbCommand command = new OleDbCommand(cmdText, connection);

            command.Connection.Open();
            OleDbDataReader reader = command.ExecuteReader();
            List<Personne> ps = new List<Personne>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ps.Add(new Personne { Nom = reader["Nom"].ToString(), Ville = reader["Ville"].ToString() });
                }
            }
            reader.Close();
            return ps;
        }
        private static List<Personne> LectureCsv()
        {
            List<Personne> ps = new List<Personne>();

            // Lecture du fichier csv -> ps
            foreach (var ligne in File.ReadAllLines(MaConsole.File))
            {
                var tab = ligne.Split(';');
                ps.Add(new Personne { Nom = tab[0], Ville = tab[1] });
            }
            return ps;
        }
        private static bool UploadPerson()
        {
            // Upload-Person -Mode "Excel" -File "Personnel.xslx"
            if (string.IsNullOrEmpty(MaConsole.File)) return false;
            if (!File.Exists(MaConsole.File)) return false;

            List<Personne> ps = null;
            if (string.IsNullOrEmpty(MaConsole.Mode)) MaConsole.Mode = MaConsole.DefaultMode;

            if (MaConsole.Mode.ToUpper() == "EXCEL")
                ps = LectureExcel();
            else 
                ps = LectureCsv();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51092");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new ObjectContent(typeof(List<Personne>), ps, new JsonMediaTypeFormatter());
                var t = client.PostAsync("/api/personne", content).Result;
            }
            return true;

            //// Todo : convert liste de personnes en byte[]
            //byte[] psByte = ObjectToByteArray(ps);


            //string url = $"http://localhost:51092/api/personne";
            //WebClient1.Headers.Add("Content-Type", "application/json");
            //var octets = WebClient1.UploadData(url, "POST", psByte);
            //var s = Encoding.Default.GetString(octets);
            //return s == @"""Ok""";
        }
        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
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
        public string Mode = null;
        public string File = null;
        public string DefaultMode = null;

        public ConsoleColor Couleur1 = ConsoleColor.Gray, Couleur2 = ConsoleColor.Yellow;
        //private Regex Reg = new Regex(@"(?'command'^[A-Z][a-z,0-9]*-[A-Z][a-z,0-9]*)( (-Id) (?'id'\d+))?( (-Nom) ""?(?'nom'[\w,\s]+)""?)?( (-Ville) ""?(?'ville'[\w,\s]+)""?)?");
        private Regex Reg = new Regex(
                   @"(?'command'^[A-Z][a-z,0-9]*-[A-Z][a-z,0-9]*)" +
                   @"( (-Id) (?'id'\d+))?" +
                   @"( (-Nom) ""(?'nom'[\w,\s]+)"")?" +
                   @"( (-Ville) ""(?'ville'[\w,\s]+)"")?" +
                   @"( (-Mode) ""(?'mode'[\w,\s,\., :,\\]+)"")?" +
                   @"( (-File) ""(?'file'[\w,\s,\., :,\\]+)"")?");

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

            for (int i = 1; i < res.Groups.Count; i++)
            {
                var group = res.Groups[i];
                if (!string.IsNullOrEmpty(group.Value))
                {
                    switch (group.Name)
                    {
                        case "id": Id = int.Parse(group.Value); break;
                        case "nom": Nom = group.Value; break;
                        case "ville": Ville = group.Value; break;
                        case "command": Commande = group.Value; break;
                        case "mode": Mode = group.Value; break;
                        case "file": File = group.Value; break;
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
