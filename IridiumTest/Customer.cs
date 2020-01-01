using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IridiumTest
{
    public interface IModel
    {
        string Name { get; }
        IEnumerable<IChildModel> Objects { get; }
    }

    public class Model : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }

        [Column.Null]
        public string Name { get; set; }

        [Relation.OneToMany(ForeignKey = "ParentID")]
        public IDataSet<ChildModel> Objects { get; set; }
    }
}
