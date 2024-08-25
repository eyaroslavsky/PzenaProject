using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class RelatedTickersDataModel
    {
        public long RelatedTickersId { get; set; }
        public string Ticker { get; set; }
        public string RelatedTicker { get; set; }
    }
}
