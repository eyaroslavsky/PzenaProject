using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class FamaDataModel
    {
        public long FamaId { get; set; }
        public string FamaSector { get; set; }
        public string FamaIndustry { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
