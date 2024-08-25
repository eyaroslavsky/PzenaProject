using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject.DataModels
{
    public class PriceDataModel
    {         
        public string Ticker { get; set; }           
        public DateTime? Date { get; set; }
        public decimal? OpenValue { get; set; }
        public decimal? High { get; set; }       
        public decimal? Low { get; set; }         
        public decimal? CloseValue { get; set; }      
        public decimal? Volume { get; set; }            
        public decimal? CloseAdj { get; set; }      
        public decimal? CloseUnadj { get; set; }    
        public DateTime? LastUpdated { get; set; }
    }

    public class PriceMap : ClassMap<PriceDataModel>
    {
        public PriceMap()
        {           
            Map(m => m.Ticker).Name("ticker");
            Map(m => m.Date).Name("date");
            Map(m => m.OpenValue).Name("open").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.High).Name("high").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.Low).Name("low").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.CloseValue).Name("close").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.Volume).Name("volume").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.CloseAdj).Name("closeadj").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.CloseUnadj).Name("closeunadj").TypeConverter<ScientificNotationDecimalConverter>();
            Map(m => m.LastUpdated).Name("lastupdated");
        }
    }

    public class ScientificNotationDecimalConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            throw new InvalidCastException($"Unable to convert '{text}' to a decimal.");
        }
    }
}
