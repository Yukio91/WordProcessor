using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Dapper;
using WordProcessor.Utils.Logger;

namespace WordProcessor.Db
{
    internal class DbManager : DbManagerBase
    {
        private const string DbName = "DbWords.sdf";

        private readonly string _connectionString;

        public DbManager(ILogger logger = null) : base(logger)
        {
            _connectionString = $"DataSource=\"{DbName}\";Password=\"password\";";
        }

        public bool CreateDatabase()
        {
            try
            {
                if (File.Exists(DbName))
                    File.Delete(DbName);

                SqlCeEngine engine = new SqlCeEngine(_connectionString);
                engine.CreateDatabase();

                CreateWordsTable();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("CreateDatabase", ex);

                return false;
            }
        }

        public void Insert(Dictionary<string, int> wordsDict)
        {
            Clear();

            Update(wordsDict);
        }

        public bool Update(Dictionary<string, int> wordsDict)
        {
            try
            {
                OpenConnection();

                foreach (var kvp in wordsDict)
                {
                    InsertOrUpdateWord(kvp.Key, kvp.Value);
                }
            }
            finally
            {
                DbConnection?.Close();
            }

            return true;
        }

        public void Clear()
        {
            ExecuteSql("DELETE FROM Words");
        }

        private void CreateWordsTable()
        {
            var sql = "Create Table Words (Id int NOT NULL IDENTITY(1,1), " +
                      "text nchar(30), frequency int)";

            ExecuteSql(sql);
        }

        public IEnumerable<string> FindWordPrefix(string prefix, int top)
        {
            try
            {
                OpenConnection();

                return DbConnection.Query<string>($@"Select Top({top}) text " +
                                                  "From Words " +
                                                  $@"Where text like '{prefix}%' " +
                                                  "Order By frequency desc, text");

            }
            finally
            {
                DbConnection?.Close();
            }
        }
        private void InsertOrUpdateWord(string word, int frequency)
        {
            int value = (int?)new SqlCeCommand($"Select frequency From Words Where text='{word}'",
                            (SqlCeConnection)DbConnection).ExecuteScalar() ?? 0;
            if (value > 0)
            {
                frequency += value;
                new SqlCeCommand($"Update Words Set frequency={frequency} Where text='{word}'",
                    (SqlCeConnection)DbConnection).ExecuteNonQuery();
            }
            else
                new SqlCeCommand($"Insert Into Words (text, frequency) values ('{word}', {frequency})",
                    (SqlCeConnection)DbConnection).ExecuteNonQuery();
        }

        private void ExecuteSql(string sql)
        {
            try
            {
                OpenConnection();

                var command = new SqlCeCommand(sql, (SqlCeConnection) DbConnection);
                command.ExecuteNonQuery();

            }
            finally
            {
                DbConnection?.Close();
            }
        }


        private bool OpenConnection()
        {
            if (!File.Exists(DbName))
                throw new FileNotFoundException("Базы данных не существует!", DbName);

            if (DbConnection == null)
                DbConnection = new SqlCeConnection(_connectionString);

            if (DbConnection.State != ConnectionState.Open)
                DbConnection.Open();

            return true;
        }
    }
}