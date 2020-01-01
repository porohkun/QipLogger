using System.ComponentModel;

namespace QipLogger
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }

        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }
    }
}