using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class SicDataModel
    {
        public long SicId { get; set; }
        public int SicCode { get; set; }
        public string SicIndustry { get; set; }
        public string SicSector { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
