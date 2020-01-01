//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Runtime.Remoting.Channels;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Windows;

//namespace QipLogger
//{
//    public class SQLiteHelper : IDisposable
//    {
//        public string ConnectionString { get; }

//        public SQLiteHelper(string connectionString)
//        {
//            ConnectionString = connectionString;
//        }

//        public static SQLiteHelper FromFile(string path)
//        {
//            if (!File.Exists(path))
//            {
//                SQLiteConnection.CreateFile(path);
//                using (var con = FromFile(path))
//                {
//                    con.InitialDB();
//                    return con;
//                }
//            }
//            return new SQLiteHelper($"Data Source={path};");
//        }

//        public void FreeFile()
//        {
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//        }

//        #region universal

//        private SQLiteTransaction BeginTransaction()
//        {
//            var connection = new SQLiteConnection(ConnectionString);
//            connection.Open();
//            return connection.BeginTransaction();
//        }

//        private void EndTransaction(SQLiteTransaction transaction)
//        {
//            transaction.Commit();
//        }

//        private void CancelTransaction(SQLiteTransaction transaction)
//        {
//            transaction.Rollback();
//            transaction.Connection.Close();
//        }

//        private int ExecuteInt(string command)
//        {
//            var result = 0;
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(command, connection)).ExecuteReader())
//                {
//                    if (!reader.Read()) result = 0;
//                    result = (int)(long)reader[0];
//                }
//            }
//            return result;
//        }

//        private long ExecuteLong(string command)
//        {
//            long result = 0;
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(command, connection)).ExecuteReader())
//                {
//                    if (!reader.Read()) result = 0;
//                    result = (long)reader[0];
//                }
//            }
//            return result;
//        }

//        private string ExecuteString(string command)
//        {
//            string result;
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(command, connection)).ExecuteReader())
//                {
//                    if (!reader.Read()) result = "";
//                    result = (string)reader[0];
//                }
//            }
//            return result;
//        }

//        private bool ExecuteBool(string command)
//        {
//            bool result;
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(command, connection)).ExecuteReader())
//                {
//                    if (!reader.Read()) result = false;
//                    result = (bool)reader[0];
//                }
//            }
//            return result;
//        }

//        private IEnumerable<string> ExecuteList(string command)
//        {
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(command, connection)).ExecuteReader())
//                {
//                    while (reader.Read())
//                        yield return (string)reader[0];
//                }
//            }
//        }

//        private void Execute(string command, SQLiteConnection connection = null)
//        {
//            var conn = connection ?? new SQLiteConnection(ConnectionString).OpenAndReturn();
//            (new SQLiteCommand(command, conn)).ExecuteNonQuery();
//            if (connection == null)
//            {
//                conn.Close();
//                conn.Dispose();
//            }
//        }

//        private void Execute(params string[] transactionCommands)
//        {
//            using (var transaction = BeginTransaction())
//            {
//                foreach (var command in transactionCommands)
//                    (new SQLiteCommand(command, transaction.Connection)).ExecuteNonQuery();
//                EndTransaction(transaction);
//            }
//        }

//        private void Execute(string command, byte[] bytes, SQLiteConnection connection = null)
//        {
//            var conn = connection ?? new SQLiteConnection(ConnectionString).OpenAndReturn();
//            var com = new SQLiteCommand(command, conn);
//            com.Parameters.Add("@bytes", System.Data.DbType.Binary, bytes.Length).Value = bytes;
//            com.ExecuteNonQuery();
//            if (connection == null)
//                conn.Close();
//        }

//        #endregion

//        public void InitialDB()
//        {
//            Execute("CREATE TABLE 'messages' ( 'id' INTEGER NOT NULL UNIQUE, 'chat_id' INTEGER, 'author_id' INTEGER, 'date' INTEGER, 'message' TEXT )");
//            Execute("CREATE TABLE 'nicknames' ( 'id' INTEGER NOT NULL UNIQUE, 'uin' TEXT NOT NULL, 'nick' TEXT NOT NULL, 'is_main' INTEGER )");
//            Execute("CREATE TABLE 'chats' ( 'id' INTEGER NOT NULL UNIQUE )");
//            Execute("CREATE TABLE 'chat_members' ( 'chat_id' INTEGER, 'member_id' INTEGER )");
//        }

//        #region firmwares

//        public IEnumerable<Firmware> GetFirmwares(params string[] ids)
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("SELECT id, type, version, description, data FROM firmwares");
//            if (ids.Length > 0)
//            {
//                sb.Append(" WHERE id in (");
//                for (int i = 0; i < ids.Length; i++)
//                {
//                    sb.Append('\'');
//                    sb.Append(ids[i]);
//                    sb.Append('\'');
//                    if (i != ids.Length - 1) sb.Append(", ");
//                }
//                sb.Append(')');
//            }
//            var firms = new List<Firmware>();
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(sb.ToString(), connection)).ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        var id = (string)reader[0];
//                        var type = (FirmwareType)(int)(long)reader[1];
//                        var version = KsytalVersion.Parse((string)reader[2]);
//                        var description = SQLDecode((string)reader[3]);
//                        var data = (byte[])reader[4];
//                        firms.Add(new Firmware(id, type, version, description, data));
//                    }
//                }
//            }
//            return firms;
//        }

//        public void AddFirmware(Firmware firm)
//        {
//            Execute("INSERT INTO firmwares (id, type, version, description, data) " +
//                $"VALUES ('{firm.Id}', {(int)firm.Type}, '{firm.Version.ToString()}', '{SQLEncode(firm.Description)}', @bytes)",
//                firm.Data);
//        }

//        #endregion

//        #region protocols

//        public IEnumerable<ProtModel> GetProtocolModels()
//        {
//            StringBuilder sb = new StringBuilder();
//            sb.Append("SELECT id, from_major, from_minor, to_major, to_minor FROM protocols ORDER BY from_major, from_minor;");
//            var prots = new List<ProtModel>();
//            using (var connection = new SQLiteConnection(ConnectionString).OpenAndReturn())
//            {
//                using (var reader = (new SQLiteCommand(sb.ToString(), connection)).ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        var id = (string)reader[0];
//                        var from = new KsytalVersion((int)(long)reader[1], (int)(long)reader[2]);
//                        var to = new KsytalVersion((int)(long)reader[3], (int)(long)reader[4]);
//                        prots.Add(new ProtModel(id, from, to));
//                    }
//                }
//            }
//            return prots;
//        }

//        public string GetProtocolText(string id)
//        {
//            return SQLDecode(ExecuteString($"SELECT json FROM protocols where id='{id}';"));
//        }

//        public string GetProtocolText(KsytalVersion version)
//        {
//            return SQLDecode(ExecuteString("SELECT json FROM protocols where " +
//                $"(from_major < {version.Major} OR (from_major = {version.Major} AND from_minor <= {version.Minor})) AND " +
//                $"(to_major > {version.Major} OR (to_major = {version.Major} AND to_minor >= {version.Minor}));"));
//        }

//        public string GetProtocolId(KsytalVersion version)
//        {
//            return SQLDecode(ExecuteString("SELECT id FROM protocols where " +
//                $"(from_major < {version.Major} OR (from_major = {version.Major} AND from_minor <= {version.Minor})) AND " +
//                $"(to_major > {version.Major} OR (to_major = {version.Major} AND to_minor >= {version.Minor}));"));
//        }

//        public string AddProtocol(ProtModel model, string text)
//        {
//            var newId = Guid.NewGuid().ToString();
//            Execute("INSERT INTO protocols (id, from_major, from_minor, to_major, to_minor, json) " +
//               $"Values ('{newId}', '{model.From.Major}', '{model.From.Minor}', '{model.To.Major}', '{model.To.Minor}', '{SQLEncode(text)}');");
//            return newId;
//        }

//        public string DuplicateProtocol(string id)
//        {
//            var newId = Guid.NewGuid().ToString();
//            Execute("INSERT INTO protocols (id, from_major, from_minor, to_major, to_minor, json) " +
//               $"SELECT '{newId}', from_major, from_minor, to_major, to_minor, json FROM protocols WHERE id='{id}';");
//            return newId;
//        }

//        public void DeleteProtocol(string id)
//        {
//            Execute($"DELETE FROM protocols WHERE id='{id}';");
//        }

//        public void UpdateProtocol(ProtModel model, string text)
//        {
//            Execute($"UPDATE protocols SET from_major = {model.From.Major}, from_minor = {model.From.Minor}, to_major = {model.To.Major}, to_minor = {model.To.Minor}, json = '{SQLEncode(text)}' WHERE id ='{model.Id}';");
//        }

//        #endregion

//        /// <summary>
//        /// делает безопасным
//        /// Если не работает - можно валить на Настю. :)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public static string SQLEncode(string input)//делает безопасным
//        {
//            input = input.Replace("\\", @"\\");
//            input = input.Replace("\a", @"\a");
//            input = input.Replace("\b", @"\b");
//            input = input.Replace("\f", @"\f");
//            input = input.Replace("\n", @"\n");
//            input = input.Replace("\r", @"\r");
//            input = input.Replace("\t", @"\t");
//            input = input.Replace("\v", @"\v");
//            input = input.Replace("\'", @"\'");
//            input = input.Replace("\"", @"\""");

//            return input;
//        }

//        /// <summary>
//        /// отменяет делание безопасным
//        /// Если не работает - можно валить на Настю. :)
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public static string SQLDecode(string input) //отменяет делание безопасным
//        {
//            input = input.Replace(@"\\", "\\");
//            input = input.Replace(@"\a", "\a");
//            input = input.Replace(@"\b", "\b");
//            input = input.Replace(@"\f", "\f");
//            input = input.Replace(@"\n", "\n");
//            input = input.Replace(@"\r", "\r");
//            input = input.Replace(@"\t", "\t");
//            input = input.Replace(@"\v", "\v");
//            input = input.Replace(@"\'", "\'");
//            input = input.Replace(@"\""", "\"");

//            return input;
//        }

//        public void Dispose()
//        {
//            FreeFile();
//        }
//    }
//}
