using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.Helpers
{
    public class JWT
    {
        public string key { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public double durationInHours { get; set; }
    }
}
