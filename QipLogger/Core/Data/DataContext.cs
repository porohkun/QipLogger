using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger.Core.Data
{
    public class DataContext : SqliteContext
    {
        public DataContext() : base("my.db")
        {
            CreateTable<User>(true, true);
            CreateTable<UserInfo>(true, true);
            CreateTable<Message>(true, true);
            CreateTable<Chat>(true, true);
        }

        public IDataSet<Chat> Chats { get; set; }
        public IDataSet<User> Users { get; set; }
        public IDataSet<UserInfo> UserInfos { get; set; }
        public IDataSet<Message> Messages { get; set; }
    }
}
