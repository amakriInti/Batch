using DemoBatch.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DemoBatch.Webapi.Controllers
{
    public class PersonneController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        public IEnumerable<Personne> GetPersonne(string ville)
        {
            var liste = (List<Personne>)HttpContext.Current.Application["Personne"];
            if (ville.ToUpper() == "TOUT") return liste;
            return liste.Where(p => ville == null || p.Ville == ville);
        }
        public string Post(int id, string nom, string ville)
        {
            var liste = (List<Personne>)System.Web.HttpContext.Current.Application["Personne"];
            var p = liste.FirstOrDefault(x => x.Id == id);
            if (p == null) return null;
            p.Nom = nom;
            p.Ville = ville;
            return "Ok";
        }
        public string Post(string nom, string ville)
        {
            var liste = (List<Personne>)System.Web.HttpContext.Current.Application["Personne"];

            var id = liste.Select(p => p.Id).Max() + 1;
            liste.Add(new Personne { Id = id, Nom = nom, Ville = ville });
            return "Ok";
        }
        // Upload-Person -File "Administrateurs.csv"
        public string Post(List<Personne> ps)
        {
            if (ps == null) return "Erreur";
            var liste = (List<Personne>)System.Web.HttpContext.Current.Application["Personne"];
            int id = liste.Select(p => p.Id).Max() + 1;
            foreach (var p in ps)
            {
                liste.Add(new Personne { Id = id, Nom = p.Nom, Ville = p.Ville });
                id++;
            }
            return "Ok";
        }
        public string PostAutre(int id)
        {
            var liste = (List<Personne>)System.Web.HttpContext.Current.Application["Personne"];
            var p = liste.FirstOrDefault(x => x.Id == id);
            if (p == null) return null;
            liste.Remove(p);
            return "Ok";
        }

    }
}
