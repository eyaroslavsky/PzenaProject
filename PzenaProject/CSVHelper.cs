using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PzenaProject.DataModels;

namespace PzenaProject
{
    public class CSVHelper
    {
        public static List<FullTickersDataModel> ReadTickersCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // Assumes the CSV file has a header row
                MissingFieldFound = null // Avoids exceptions for missing fields
            }))
            {
                csv.Context.RegisterClassMap<FullTickersMap>();
                var records = csv.GetRecords<FullTickersDataModel>();
                return new List<FullTickersDataModel>(records); 
            }
        }

        public static List<PriceDataModel> ReadPriceCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // Assumes the CSV file has a header row
                MissingFieldFound = null // Avoids exceptions for missing fields
            }))
            {
                csv.Context.RegisterClassMap<PriceMap>();
                var records = csv.GetRecords<PriceDataModel>();
                return new List<PriceDataModel>(records); 
            }
        }
    }
}
