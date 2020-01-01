using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger
{
    public class DBModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private bool _dbLoaded;
        public bool DBLoaded
        {
            get => _dbLoaded;
            set
            {
                if (_dbLoaded != value)
                {
                    _dbLoaded = value;
                    NotifyPropertyChanged(nameof(DBLoaded));
                }
            }
        }

    }
}
