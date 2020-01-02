using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger.Core.Data
{
    public class User : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }

        [Relation.OneToMany(ForeignKey = nameof(UserInfo.UserID))]
        public IDataSet<UserInfo> UserInfos { get; set; }


        [Relation.OneToOne]
        public UserInfo MainUserInfo { get; set; }


        [Column.NotNull]
        public string UIN { get; set; }
    }
}
