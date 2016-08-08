using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeGoBot.Core.Data.Poco
{
    public class PokestopPoco
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Id { get; set; }

        public double Distance { get; set; }
    }
}
