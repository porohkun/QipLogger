using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger.Core.Data
{
    public class Chat : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }

        public string Name { get; set; }


        [Relation.OneToMany(ForeignKey = nameof(Message.ChatID))]
        public IDataSet<Message> Messages { get; set; }
    }
}
