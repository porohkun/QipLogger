using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IridiumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MyContext();

            var model = new Model()
            {
                Name = "mdl1"
            };
            model.Save(m => m.Objects);
            //db.Save(model, m => m.Objects);
            db.Save(new ChildModel() { Name = "cm1", Parent = model });
            db.Save(new ChildModel() { Name = "cm2", Parent = model });
            db.Save(new ChildModel() { Name = "cm3", Parent = model });
            db.Save(new ChildModel() { Name = "cm4", Parent = model });


            //db.Save(order, o => o.Customer);
        }
    }
}
