using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class IssuerDataModel
    {
        public long IssuerId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ScaleMarketCap { get; set; }
        public string ScaleRevenue { get; set; }
        public string Currency { get; set; }
        public string Location { get; set; }
        public string CompanySite { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
