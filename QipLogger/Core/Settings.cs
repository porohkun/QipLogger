using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace QipLogger
{
    [Serializable]
    public class WindowStateSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string strPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }

        private double _width = 640;
        private double _height = 480;
        private WindowState _state;

        public event EventHandler WidthChanged;
        public event EventHandler HeightChanged;
        public event EventHandler StateChanged;

        [XmlAttribute]
        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    NotifyPropertyChanged(nameof(Width));
                }
            }
        }

        [XmlAttribute]
        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    NotifyPropertyChanged(nameof(Height));
                }
            }
        }

        [XmlAttribute]
        public WindowState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    NotifyPropertyChanged(nameof(State));
                }
            }
        }
    }

    [Serializable]
    public class Settings
    {
        public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QIP Logger");
        public static string SettingsPath => Path.Combine(AppDataPath, "settings.xml");

        public static Settings Instance { get; set; } = Load();

        private WindowStateSettings _mainWindow;
        public WindowStateSettings MainWindow
        {
            get
            {
                if (_mainWindow == null)
                    MainWindow = new WindowStateSettings() { Width = 640, Height = 480 };
                return _mainWindow;
            }
            set
            {
                _mainWindow = value;
                _mainWindow.PropertyChanged += SaveByPropertyChanged;
            }
        }


        public Settings() { }

        private static Settings Load()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            if (File.Exists(SettingsPath))
            {
                using (var stream = File.OpenRead(SettingsPath))
                {
                    try
                    {
                        return (Settings)serializer.Deserialize(stream);
                    }
                    catch { }
                }
            }
            return new Settings();
        }

        private void SaveByPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        public static void Save()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            try
            {
                using (var stream = File.Open(SettingsPath, FileMode.Create))
                {
                    serializer.Serialize(stream, Instance);
                }
            }
            catch { }
        }
    }
}
