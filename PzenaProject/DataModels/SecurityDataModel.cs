using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class SecurityDataModel
    {
        public string Ticker { get; set; }
        public string Permaticker { get; set; }
        public string TableName { get; set; }
        public string Exchange { get; set; }
        public string IsDelisted { get; set; }
        public long? SicId { get; set; }
        public long? FamaId { get; set; }
        public long? SecotrIndustryId { get; set; }
        public long? IssuerId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? FirstAdded { get; set; }
        public DateTime? FirstPriceDate { get; set; }
        public DateTime? LastPriceDate { get; set; }
        public DateTime? FirstQuarter { get; set; }
        public DateTime? LastQuarter { get; set; }
        public string SecFilings { get; set; }
    }
}
