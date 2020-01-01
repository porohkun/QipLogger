using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IridiumTest
{
    public interface IChildModel
    {
        string Name { get; }
        IModel Parent { get; }
    }

    public class ChildModel : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }

        public int ParentID { get; set; }

        [Relation.ManyToOne(ForeignKey = "ID", LocalKey = "ParentID")]
        public Model Parent
        {
            get => _parent;
            set
            {
                if (value.ID != ParentID)
                    ParentID = value.ID;
                _parent = value;
            }
        }
        private Model _parent;

        [Column.Null]
        public string Name { get; set; }
    }
}
