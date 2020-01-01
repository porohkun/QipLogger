using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IridiumTest
{
    public class MyContext : SqliteContext
    {
        public MyContext() : base("my.db")
        {
            CreateTable<Model>(true, true);
            CreateTable<ChildModel>(true, true);
        }

        public IDataSet<Model> Models; // will be assigned automatically
        public IDataSet<ChildModel> ChildModels;       // will be assigned automatically
        //IDataSet<Product> Products;   // will be assigned automatically
    }
}
