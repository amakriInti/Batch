using DemoBatch.Webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DemoBatch.Webapi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var liste = new List<Personne> {
            new Personne{ Id=1, Nom="Alban", Ville="Agen"  },
            new Personne{ Id=2, Nom="Baltazar", Ville="Bern"  },
            new Personne{ Id=3, Nom="Carmen", Ville="Agen"  }
            };
            Application["Personne"] = liste;

        }
    }
}
