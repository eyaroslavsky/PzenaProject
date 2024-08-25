using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class SectorIndustryDataModel
    {
        public long SectorIndustryId { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
