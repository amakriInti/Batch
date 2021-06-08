using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoBatch.Dto
{
    [Serializable]
    public class Personne
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Ville { get; set; }
        public override string ToString()
        {
            return $"{Id}: {Nom} - {Ville}";
        }
    }
}
