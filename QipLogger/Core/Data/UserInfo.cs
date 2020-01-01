using Iridium.DB;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger.Core.Data
{
    public class UserInfo : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }


        public int UserID { get; set; }
        [Relation.ManyToOne(LocalKey = nameof(UserID), ForeignKey = nameof(Data.User.ID))]
        public User User
        {
            get => _user;
            set { UserID = value.ID; _user = value; }
        }
        private User _user;


        [Column.NotNull]
        public string Name { get; set; }
    }
}
