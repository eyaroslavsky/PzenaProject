# PzenaProject

This project reads two CSV files (TICKERS.csv and PRICES.csv), normalizes and inserts the data into SQL tables, and creates stored procedures to better analyze the data.

This project starts from the Program.cs file, where no Main() method is needed due to this being made after .NET 6.

This project takes in one argument that represents the root folder location of the CSV files (Example argument would look like: "D:\Temp\RootProjectFolder").

This project uses the localhost server for the SQL Server database instance and Windows Authentication, creating a new "Pzena" database as needed.    
