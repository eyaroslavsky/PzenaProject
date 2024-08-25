using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PzenaProject.DataModels;

namespace PzenaProject
{
    public class DatabaseHelper
    {
        protected string ConnectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
        protected string DatabaseName = "Pzena";

        public DatabaseHelper() 
        {
            string sql = @"IF EXISTS (SELECT name FROM sys.databases WHERE name = @DatabaseName) SELECT 1 ELSE SELECT 0";
            var parameters = new DynamicParameters();
            parameters.Add("@DatabaseName", DatabaseName);
            int result = ExecuteSQLReturnInt(sql, parameters);

            if (result == 0)
            {
                sql = $"CREATE DATABASE {DatabaseName};";
                ExecuteSQL(sql);                
            }

            ConnectionString = $"Server=localhost;Database={DatabaseName};Trusted_Connection=True;";
        }

        public void ExecuteSQL(string sql, DynamicParameters parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection to SQL Server opened successfully.");

                    connection.Query(sql, parameters, commandTimeout: 300);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Server connection failed: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection to SQL Server closed.");
                }
            }
        }

        public int ExecuteSQLReturnInt(string sql, DynamicParameters parameters = null)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection to SQL Server opened successfully.");

                    result = connection.Query<int>(sql, parameters, commandTimeout: 300).FirstOrDefault();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Server connection failed: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection to SQL Server closed.");
                }
            }

            return result;
        }

        public List<T> ExecuteSQLReturnList<T>(string sql, DynamicParameters parameters = null)
        {
            List<T> results = new List<T>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection to SQL Server opened successfully.");

                    results = connection.Query<T>(sql, parameters, commandTimeout: 300).ToList();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Server connection failed: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection to SQL Server closed.");
                }
            }

            return results; ;
        }

        public Dictionary<(string, string), long> RetrieveFamaDictionary()
        {
            string famaSQL = "SELECT * FROM dbo.Fama";
            List<FamaDataModel> famaList = ExecuteSQLReturnList<FamaDataModel>(famaSQL);
            Dictionary<(string, string), long> dict = famaList.ToDictionary(k => (k.FamaIndustry, k.FamaSector), v => v.FamaId);
            return dict;
        }

        public Dictionary<int, long> RetrieveSicDictionary()
        {
            string sicSQL = "SELECT * FROM dbo.Sic";
            List<SicDataModel> sicList = ExecuteSQLReturnList<SicDataModel>(sicSQL);
            Dictionary<int, long> dict = sicList.ToDictionary(k => k.SicCode, v => v.SicId);
            return dict;
        }

        public Dictionary<(string, string), long> RetrieveSectorIndustryDictionary()
        {
            string sectorIndustrySQL = "SELECT * FROM dbo.SectorIndustry";
            List<SectorIndustryDataModel> famaList = ExecuteSQLReturnList<SectorIndustryDataModel>(sectorIndustrySQL);
            Dictionary<(string, string), long> dict = famaList.ToDictionary(k => (k.Industry, k.Sector), v => v.SectorIndustryId);
            return dict;

        }

        public Dictionary<(string, string), long> RetrieveIssuerDictionary()
        {
            string issuerSQL = "SELECT * FROM dbo.Issuer";
            List<IssuerDataModel> sicList = ExecuteSQLReturnList<IssuerDataModel>(issuerSQL);
            Dictionary<(string, string), long> dict = sicList.ToDictionary(k => (k.Name, k.Category), v => v.IssuerId);
            return dict;
        }

        public void InitializeSQLTables()
        {
            List<string> tableSQLStrings = RetrieveTableSQLStrings();
            foreach (string tableSQL in tableSQLStrings)
            {
                ExecuteSQL(tableSQL);
            }
        }

        public void InitializeStoredProcedures()
        {
            List<string> spStrings = RetrieveStoredProcedureStrings();
            foreach (string storedProc in spStrings)
            {
                ExecuteSQL(storedProc);
            }
        }

        private List<string> RetrieveStoredProcedureStrings()
        {
            string averagePrice52Day = @"
                CREATE PROCEDURE Calculate52DayMovingAverage
                    @Ticker VARCHAR(255)
                AS
                BEGIN
                    -- Calculate the 52-day moving average for the specified ticker
                    SELECT 
                        @Ticker AS Ticker,
                        AVG(p.CloseAdj) AS MovingAverage,
                        MIN(p.Date) AS MinDate,
                        MAX(p.Date) AS MaxDate
                    FROM 
                        dbo.Prices p
                    WHERE 
                        p.Ticker = @Ticker
                        AND p.Date >= DATEADD(DAY, -52, (SELECT MAX(p2.Date) FROM dbo.Prices p2 WHERE p2.Ticker = @Ticker)) -- Filter for the last available 52 days
                    GROUP BY 
                        p.Ticker
                END
            ";

            string highPrice52Week = @"
                CREATE PROCEDURE Calculate52WeekHighPrice
                    @Ticker VARCHAR(255)
                AS
                BEGIN
                    -- Calculate the 52-week high price for the specified ticker
                    SELECT 
                        @Ticker AS Ticker,
                        MAX(p.High) AS HighPrice,
                        MIN(p.Date) AS MinDate,
                        MAX(p.Date) AS MaxDate
                    FROM 
                        dbo.Prices p
                    WHERE 
                        p.Ticker = @Ticker
                        AND p.Date >= DATEADD(WEEK, -52, (SELECT MAX(p2.Date) FROM dbo.Prices p2 WHERE p2.Ticker = @Ticker)) -- Filter for the last available 52 weeks
                    GROUP BY 
                        p.Ticker;
                END
            ";

            string lowPrice52Week = @"
                CREATE PROCEDURE Calculate52WeekLowPrice
                    @Ticker VARCHAR(255)
                AS
                BEGIN
                    -- Calculate the 52-week low price for the specified ticker
                    SELECT 
                        @Ticker AS Ticker,
                        MIN(p.Low) AS LowPrice,
                        MIN(p.Date) AS MinDate,
                        MAX(p.Date) AS MaxDate
                    FROM 
                        dbo.Prices p
                    WHERE 
                        p.Ticker = @Ticker
                        AND p.Date >= DATEADD(WEEK, -52, (SELECT MAX(p2.Date) FROM dbo.Prices p2 WHERE p2.Ticker = @Ticker)) -- Filter for the last available 52 weeks
                    GROUP BY 
                        p.Ticker;
                END
            ";

            return new List<string> { averagePrice52Day, highPrice52Week, lowPrice52Week };
        }

        private List<string> RetrieveTableSQLStrings()
        {
            string securityTable = @"
                IF OBJECT_ID('dbo.Security', 'U') IS NULL
                BEGIN
                    CREATE TABLE Security (
                        Ticker VARCHAR(255),
                        Permaticker VARCHAR(255),
                        TableName VARCHAR(255),
                        Exchange VARCHAR(255),
                        IsDelisted VARCHAR(255),
                        SicId INT,
                        FamaId INT,
                        SectorIndustryId INT,
                        IssuerId INT,
                        LastUpdated DATE,
                        FirstAdded DATE,
                        FirstPriceDate DATE,
                        LastPriceDate DATE,
                        FirstQuarter DATE,
                        LastQuarter DATE,
                        SecFilings VARCHAR(255),
                        PRIMARY KEY (Ticker, Permaticker, TableName),
                        FOREIGN KEY (SicId) REFERENCES Sic(SicId),
                        FOREIGN KEY (FamaId) REFERENCES Fama(FamaId),
                        FOREIGN KEY (SectorIndustryId) REFERENCES SectorIndustry(SectorIndustryId),
                        FOREIGN KEY (IssuerId) REFERENCES Issuer(IssuerId)
                    )

                    CREATE INDEX IDX_Security_Ticker ON Security(Ticker)
                    CREATE INDEX IDX_Security_SicId ON Security(SicId)
                    CREATE INDEX IDX_Security_FamaId ON Security(FamaId)
                    CREATE INDEX IDX_Security_SectorIndustryId ON Security(SectorIndustryId)
                    CREATE INDEX IDX_Security_IssuerId ON Security(IssuerId)
                END
            ";

            string cusipTable = @"
                IF OBJECT_ID('dbo.Cusips', 'U') IS NULL
                BEGIN
                    CREATE TABLE Cusips (
                        CusipId INT IDENTITY(1,1) PRIMARY KEY,
                        Ticker VARCHAR(255),
                        Cusip VARCHAR(255)
                    )

                    CREATE INDEX IDX_Cusips_Ticker ON Cusips(Ticker);
                END
            ";

            string relatedTickersTable = @"
                IF OBJECT_ID('dbo.RelatedTickers', 'U') IS NULL
                BEGIN
                    CREATE TABLE RelatedTickers (
                        RelatedTickersId INT IDENTITY(1,1) PRIMARY KEY,
                        Ticker VARCHAR(255),
                        RelatedTicker VARCHAR(255)
                    )

                    CREATE INDEX IDX_RelatedTickers_Ticker ON RelatedTickers(Ticker);
                END
            ";

            string issuerTable = @"
                IF OBJECT_ID('dbo.Issuer', 'U') IS NULL
                BEGIN
                    CREATE TABLE Issuer (
                        IssuerId INT IDENTITY(1,1) PRIMARY KEY,
                        Name VARCHAR(255),
                        Category VARCHAR(255),
                        ScaleMarketCap VARCHAR(255),
                        ScaleRevenue VARCHAR(255),
                        Currency VARCHAR(10),
                        Location VARCHAR(255),
                        CompanySite VARCHAR(255),
                        LastUpdated DATE
                    )
                END
            ";

            string sicTable = @"
                IF OBJECT_ID('dbo.Sic', 'U') IS NULL
                BEGIN
                    CREATE TABLE Sic (
                        SicId INT IDENTITY(1,1) PRIMARY KEY,
                        SicCode INT,
                        SicSector VARCHAR(255),
                        SicIndustry VARCHAR(255),
                        LastUpdated DATE
                    )
                END
            ";

            string famaTable = @"
                IF OBJECT_ID('dbo.Fama', 'U') IS NULL
                BEGIN
                    CREATE TABLE Fama (
                        FamaId INT IDENTITY(1,1) PRIMARY KEY,
                        FamaSector VARCHAR(255),
                        FamaIndustry VARCHAR(255),
                        LastUpdated DATE
                    )
                END
            ";

            string sectorIndustryTable = @"
                IF OBJECT_ID('dbo.SectorIndustry', 'U') IS NULL
                BEGIN
                    CREATE TABLE SectorIndustry (
                        SectorIndustryId INT IDENTITY(1,1) PRIMARY KEY,
                        Sector VARCHAR(255),
                        Industry VARCHAR(255),
                        LastUpdated DATE
                    )
                END
            ";

            string pricesTable = @"
                IF OBJECT_ID('dbo.Prices', 'U') IS NULL
                BEGIN
                    CREATE TABLE Prices (
                        Ticker VARCHAR(255),
                        Date DATE,
                        OpenValue DECIMAL(18, 2),
                        High DECIMAL(18, 2),
                        Low DECIMAL(18, 2),
                        CloseValue DECIMAL(18, 2),
                        Volume DECIMAL(18, 2),
                        CloseAdj DECIMAL(18, 2),
                        CloseUnadj DECIMAL(18, 2),
                        LastUpdated DATE,
                        PRIMARY KEY (Ticker, Date)
                    )

                    CREATE INDEX IDX_Prices_Ticker ON Prices(Ticker);
                    CREATE INDEX IDX_Prices_Date ON Prices(Date);
                    CREATE INDEX IDX_Prices_LastUpdated ON Prices(LastUpdated);
                END
            ";

            return new List<string> { cusipTable, relatedTickersTable, issuerTable, sicTable, famaTable, sectorIndustryTable, securityTable, pricesTable };
        }

        public void BulkInsert<T>(List<T> records, string tableName, bool ignoreIDColumns = true)
        {
            // Create a DataTable to hold the data
            DataTable table = CreateDataTable(records, ignoreIDColumns);

            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection to SQL Server opened successfully.");

                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 300;

                        // Write data to the database
                        bulkCopy.WriteToServer(table);
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Server connection failed: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection to SQL Server closed.");
                }
            }
        }

        public void BulkInsertInBatchesParallel<T>(List<T> records, string tableName, int batchSize, int maxDegreeOfParallelism, bool ignoreIDColumns = true)
        {
            var batches = SplitList(records, batchSize);
            int counter = 1;
            object lockObj = new object();

            Parallel.ForEach(batches, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, batch =>
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 600; // Set timeout to 10 minutes (600 seconds)

                        DataTable table = CreateDataTable(batch, ignoreIDColumns);

                        try
                        {
                            bulkCopy.WriteToServer(table);
                            Console.WriteLine($"Finished Batch {counter}");
                            lock (lockObj) 
                            {
                                counter++;
                            }
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine($"SQL Error: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            });
        }

        private static List<List<T>> SplitList<T>(List<T> list, int batchSize)
        {
            List<List<T>> results = new List<List<T>>();
            for (int i = 0; i < list.Count; i += batchSize)
            {
                results.Add(list.Skip(i).Take(batchSize).ToList());
            }

            return results;
        }

        private DataTable CreateDataTable<T>(List<T> records, bool ignoreIDColumns = true)
        {
            var table = new DataTable();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Populate the DataTable
            foreach (var record in records)
            {
                var row = table.NewRow();
                foreach (var prop in properties)
                {
                    if (!prop.Name.EndsWith("Id"))
                        row[prop.Name] = prop.GetValue(record) ?? DBNull.Value;
                    else if (prop.Name.EndsWith("Id") && !ignoreIDColumns)
                        row[prop.Name] = prop.GetValue(record) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
