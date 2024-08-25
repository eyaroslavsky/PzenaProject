using PzenaProject.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaProject
{
    public class TickerNormalization
    {
        public static List<IssuerDataModel> NormalizeIntoIssuerData(List<FullTickersDataModel> fullData)
        {
            DateTime lastUpdated = DateTime.Now;

            List<IssuerDataModel> issuerData =
                (fullData.GroupBy(x => new { x.Name, x.Category })
                .Select(grp => new IssuerDataModel
                {
                    Name = grp.Key.Name,
                    Category = grp.Key.Category,
                    ScaleMarketCap = grp.FirstOrDefault().ScaleMarketCap,
                    ScaleRevenue = grp.FirstOrDefault().ScaleRevenue,
                    Currency = grp.FirstOrDefault().Currency,
                    Location = grp.FirstOrDefault().Location,
                    CompanySite = grp.FirstOrDefault().CompanySite,
                    LastUpdated = lastUpdated
                })).ToList();

            return issuerData;
        }

        public static List<SicDataModel> NormalizeIntoSicData(List<FullTickersDataModel> fullData)
        {
            DateTime lastUpdated = DateTime.Now;

            List<SicDataModel> sicData =
                (fullData.GroupBy(x => x.SicCode)
                .Where(x => x.Key != null)
                .Select(grp => new SicDataModel
                {
                    SicCode = grp.Key.Value,
                    SicSector = grp.FirstOrDefault().SicSector,
                    SicIndustry = grp.FirstOrDefault().SicIndustry,
                    LastUpdated = lastUpdated
                })).ToList();

            return sicData;
        }

        public static List<FamaDataModel> NormalizeIntoFamaData(List<FullTickersDataModel> fullData)
        {
            DateTime lastUpdated = DateTime.Now;

            List<FamaDataModel> famaData =
                (fullData.GroupBy(x => new { x.FamaIndustry, x.FamaSector })
                .Where(x => !string.IsNullOrEmpty(x.Key.FamaIndustry))
                .Select(grp => new FamaDataModel
                {
                    FamaIndustry = grp.Key.FamaIndustry,
                    FamaSector = grp.Key.FamaSector,
                    LastUpdated = lastUpdated
                })).ToList();

            return famaData;
        }

        public static List<SectorIndustryDataModel> NormalizeIntoSectorIndustryData(List<FullTickersDataModel> fullData)
        {
            DateTime lastUpdated = DateTime.Now;

            List<SectorIndustryDataModel> sectorIndustryData =
                (fullData.GroupBy(x => new { x.Industry, x.Sector })
                .Where(x => !string.IsNullOrEmpty(x.Key.Industry))
                .Select(grp => new SectorIndustryDataModel
                {
                    Industry = grp.Key.Industry,
                    Sector = grp.Key.Sector,
                    LastUpdated = lastUpdated
                })).ToList();

            return sectorIndustryData;
        }

        public static List<CusipsDataModel> NormalizeIntoCusipData(List<FullTickersDataModel> fullData)
        {
            List<CusipsDataModel> cusipList = new List<CusipsDataModel>();

            foreach (FullTickersDataModel fullDataItem in fullData)
            {
                string ticker = fullDataItem.Ticker;
                List<string> cusips = fullDataItem.Cusips.Split(" ").ToList();
                foreach (string cusip in cusips)
                {
                    cusipList.Add(new CusipsDataModel
                    {
                        Ticker = ticker,
                        Cusip = cusip                        
                    });
                }
            }

            return cusipList;
        }

        public static List<RelatedTickersDataModel> NormalizeIntoRelatedTickerData(List<FullTickersDataModel> fullData)
        {
            List<RelatedTickersDataModel> relatedTickerList = new List<RelatedTickersDataModel>();

            foreach (FullTickersDataModel fullDataItem in fullData)
            {
                string ticker = fullDataItem.Ticker;
                List<string> relatedTickers = fullDataItem.RelatedTickers.Split(" ").ToList();
                foreach (string relatedTicker in relatedTickers)
                {
                    if (!string.IsNullOrEmpty(relatedTicker.Trim()))
                    {
                        relatedTickerList.Add(new RelatedTickersDataModel
                        {
                            Ticker = ticker,
                            RelatedTicker = relatedTicker
                        });
                    }
                }
            }

            return relatedTickerList;
        }

        public static List<SecurityDataModel> NormalizeIntoSecurityData(List<FullTickersDataModel> fullData, 
            Dictionary<int, long> sicCodeToID, 
            Dictionary<(string, string), long> famaToId, 
            Dictionary<(string, string), long> sectorIndustryToId,
            Dictionary<(string, string), long> issuerToId)
        {
            List<SecurityDataModel> securityList = new List<SecurityDataModel>();
            DateTime lastUpdated = DateTime.Now;

            foreach (FullTickersDataModel fullTickersDataModel in fullData)
            {
                securityList.Add(new SecurityDataModel
                {
                    Ticker = fullTickersDataModel.Ticker,
                    Permaticker = fullTickersDataModel.Permaticker,
                    TableName = fullTickersDataModel.TableName,
                    Exchange = fullTickersDataModel.Exchange,
                    IsDelisted = fullTickersDataModel.IsDelisted,
                    SicId = fullTickersDataModel.SicCode.HasValue && sicCodeToID.ContainsKey(fullTickersDataModel.SicCode.Value) ? sicCodeToID[fullTickersDataModel.SicCode.Value] : null,
                    FamaId = famaToId.ContainsKey((fullTickersDataModel.FamaIndustry, fullTickersDataModel.FamaSector)) ? famaToId[(fullTickersDataModel.FamaIndustry, fullTickersDataModel.FamaSector)] : null,
                    SecotrIndustryId = sectorIndustryToId.ContainsKey((fullTickersDataModel.Industry, fullTickersDataModel.Sector)) ? sectorIndustryToId[(fullTickersDataModel.Industry, fullTickersDataModel.Sector)] : null,
                    IssuerId = issuerToId.ContainsKey((fullTickersDataModel.Name, fullTickersDataModel.Category)) ? issuerToId[(fullTickersDataModel.Name, fullTickersDataModel.Category)] : null,
                    LastUpdated = lastUpdated,
                    FirstAdded = fullTickersDataModel.FirstAdded,
                    FirstPriceDate = fullTickersDataModel.FirstPriceDate,
                    LastPriceDate = fullTickersDataModel.LastPriceDate,
                    FirstQuarter = fullTickersDataModel.FirstQuarter,
                    LastQuarter = fullTickersDataModel.LastQuarter,
                    SecFilings = fullTickersDataModel.SecFilings
                });
            }

            return securityList;
        }
    }
}
