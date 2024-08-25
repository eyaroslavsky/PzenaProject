using PzenaProject;
using PzenaProject.DataModels;

if (args.Length == 0)
{
    throw new ArgumentException("No arguments provided. Please provide the root path of the CSV files");
}
string basePath = args[0];
string tickerFilePath = Path.Combine(basePath, "TICKERS.csv");
string priceFilePath = Path.Combine(basePath, "PRICES.csv");

Console.WriteLine("Creating Tables and Stored Procedures...");
DatabaseHelper dbHelper = new DatabaseHelper();
dbHelper.InitializeSQLTables();
dbHelper.InitializeStoredProcedures();
Console.WriteLine("Finished Creating Tables and Stored Procedures");

Console.WriteLine("Reading Ticker CSV File...");
List<FullTickersDataModel> tickersFileData = CSVHelper.ReadTickersCsvFile(tickerFilePath);
Console.WriteLine("Finished Reading Ticker CSV File");

Console.WriteLine("Normalizing and Inserting data from Ticker File...");
List<IssuerDataModel> issuerData = TickerNormalization.NormalizeIntoIssuerData(tickersFileData);
List<CusipsDataModel> cusipData = TickerNormalization.NormalizeIntoCusipData(tickersFileData);
List<FamaDataModel> famaData = TickerNormalization.NormalizeIntoFamaData(tickersFileData);
List<RelatedTickersDataModel> relatedTickerData = TickerNormalization.NormalizeIntoRelatedTickerData(tickersFileData);
List<SectorIndustryDataModel> sectorIndustryData = TickerNormalization.NormalizeIntoSectorIndustryData(tickersFileData);
List<SicDataModel> sicData = TickerNormalization.NormalizeIntoSicData(tickersFileData);
dbHelper.BulkInsert(issuerData, "dbo.Issuer");
dbHelper.BulkInsert(cusipData, "dbo.Cusips");
dbHelper.BulkInsert(famaData, "dbo.Fama");
dbHelper.BulkInsert(relatedTickerData, "dbo.RelatedTickers");
dbHelper.BulkInsert(sectorIndustryData, "dbo.SectorIndustry");
dbHelper.BulkInsert(sicData, "dbo.Sic");

Dictionary<(string, string), long> issuerDict = dbHelper.RetrieveIssuerDictionary();
Dictionary<int, long> sicDict = dbHelper.RetrieveSicDictionary();
Dictionary<(string, string), long> famaDict = dbHelper.RetrieveFamaDictionary();
Dictionary<(string, string), long> sectorIndustryDict =  dbHelper.RetrieveSectorIndustryDictionary();
List<SecurityDataModel> securityData = TickerNormalization.NormalizeIntoSecurityData(tickersFileData, sicDict, famaDict, sectorIndustryDict, issuerDict);
dbHelper.BulkInsert(securityData, "dbo.Security", false);
Console.WriteLine("Finished Normalizing and Inserting data from Ticker File");

Console.WriteLine("Reading Prices CSV File...");
List<PriceDataModel> pricesFileData = CSVHelper.ReadPriceCsvFile(priceFilePath);
Console.WriteLine("Finished Reading Prices CSV File");

Console.WriteLine("Inserting Prices data in batches...");
int maxDegreesOfParallelism = 20;
int batchSize = 1000000;
dbHelper.BulkInsertInBatchesParallel(pricesFileData, "dbo.Prices", batchSize, maxDegreesOfParallelism, false);
Console.WriteLine("Finished Inserting Prices data in batches");




