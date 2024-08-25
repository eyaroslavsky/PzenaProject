using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class FullTickersDataModel
    {
        public string TableName { get; set; }           
        public string Permaticker { get; set; }         
        public string Ticker { get; set; }              
        public string Name { get; set; }                
        public string Exchange { get; set; }            
        public string IsDelisted { get; set; }            
        public string Category { get; set; }            
        public string Cusips { get; set; }              
        public int? SicCode { get; set; }                
        public string SicSector { get; set; }           
        public string SicIndustry { get; set; }         
        public string FamaSector { get; set; }          
        public string FamaIndustry { get; set; }       
        public string Sector { get; set; }              
        public string Industry { get; set; }            
        public string ScaleMarketCap { get; set; }     
        public string ScaleRevenue { get; set; }       
        public string RelatedTickers { get; set; }      
        public string Currency { get; set; }            
        public string Location { get; set; }            
        public DateTime? LastUpdated { get; set; }       
        public DateTime? FirstAdded { get; set; }        
        public DateTime? FirstPriceDate { get; set; }    
        public DateTime? LastPriceDate { get; set; }    
        public DateTime? FirstQuarter { get; set; }      
        public DateTime? LastQuarter { get; set; }       
        public string SecFilings { get; set; }          
        public string CompanySite { get; set; }
    }

    public class FullTickersMap : ClassMap<FullTickersDataModel>
    {
        public FullTickersMap()
        {
            Map(m => m.TableName).Name("table");
            Map(m => m.Permaticker).Name("permaticker");
            Map(m => m.Ticker).Name("ticker");
            Map(m => m.Name).Name("name");
            Map(m => m.Exchange).Name("exchange");
            Map(m => m.IsDelisted).Name("isdelisted");
            Map(m => m.Category).Name("category");
            Map(m => m.Cusips).Name("cusips");
            Map(m => m.SicCode).Name("siccode");
            Map(m => m.SicSector).Name("sicsector");
            Map(m => m.SicIndustry).Name("sicindustry");
            Map(m => m.FamaSector).Name("famasector");
            Map(m => m.FamaIndustry).Name("famaindustry");
            Map(m => m.Sector).Name("sector");
            Map(m => m.Industry).Name("industry");
            Map(m => m.ScaleMarketCap).Name("scalemarketcap");
            Map(m => m.ScaleRevenue).Name("scalerevenue");
            Map(m => m.RelatedTickers).Name("relatedtickers");
            Map(m => m.Currency).Name("currency");
            Map(m => m.Location).Name("location");
            Map(m => m.LastUpdated).Name("lastupdated");
            Map(m => m.FirstAdded).Name("firstadded");
            Map(m => m.FirstPriceDate).Name("firstpricedate");
            Map(m => m.LastPriceDate).Name("lastpricedate");
            Map(m => m.FirstQuarter).Name("firstquarter");
            Map(m => m.LastQuarter).Name("lastquarter");
            Map(m => m.SecFilings).Name("secfilings");
            Map(m => m.CompanySite).Name("companysite");
        }
    }
}
