using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger
{
    public class DBModelDesign
    {
        public bool DBLoaded => false;
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<User> SelectedUsers { get; set; }

        public DBModelDesign()
        {
            Users = new ObservableCollection<User>()
            {
                new User("User A"),
                new User("User B"),
                new User("User C"),
                new User("User D")
            };
            SelectedUsers = new ObservableCollection<User>()
            {
                Users[0],
                Users[2]
            };
        }
    }
}
