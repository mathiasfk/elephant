﻿using System;
using System.Data.SqlClient;

namespace Takenet.Elephant.Tests.Sql
{
    public class SqlFixture : IDisposable
    {
        public SqlFixture()
        {
            //var hostName = Environment.GetEnvironmentVariable("SQLSERVER_HOSTNAME");
            //var userName = Environment.GetEnvironmentVariable("SQLSERVER_USERNAME");
            //var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD");

            //Connection = new SqlConnection($"Server={hostName};Database=master;User ID={userName};Password={password}");

            // Note: You should create the Localdb instance if it doesn't exists
            // Go to the command prompt and run: sqllocaldb create "MSSQLLocalDB"
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            using (var useDatabaseCommand = Connection.CreateCommand())
            {
                useDatabaseCommand.CommandText = "USE master";
                useDatabaseCommand.ExecuteNonQuery();
            }

            using (var dropDatabaseCommand = Connection.CreateCommand())
            {
                dropDatabaseCommand.CommandText = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE Name = '{DatabaseName}') CREATE DATABASE {DatabaseName}";
                dropDatabaseCommand.ExecuteNonQuery();
            }

            using (var useDatabaseCommand = Connection.CreateCommand())
            {
                useDatabaseCommand.CommandText = $"USE {DatabaseName}";
                useDatabaseCommand.ExecuteNonQuery();
            }
        }

        public SqlConnection Connection { get; }

        public string DatabaseName { get; } = "Elephant";

        public string ConnectionString { get; } = @"Server=(localdb)\MSSQLLocalDB;Database=Elephant;Integrated Security=true";

        public void DropTable(string tableName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"IF EXISTS (SELECT * FROM sys.tables WHERE Name = '{tableName}') DROP TABLE {tableName}";
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}