using Iridium.DB;
using QipLogger.Core.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using User = QipLogger.Core.Data.User;

namespace QipLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var dc = new DataContext();

            var chat = new Chat();
            chat.Save();

            var user1 = new Core.Data.User() { UIN = "user1_uin" };
            user1.Save(u => u.UserInfos, u => u.MainUserInfo);

            var user1_nick1 = new UserInfo() { Name = "user1_nick1", User = user1 };
            user1_nick1.Save(n => n.User);

            var user1_nick2 = new UserInfo() { Name = "user1_nick2", User = user1 };
            user1_nick2.Save(n => n.User);

            var user2 = new Core.Data.User() { UIN = "user2_uin" };
            user2.Save(u => u.UserInfos, u => u.MainUserInfo);

            var user2_nick1 = new UserInfo() { Name = "user2_nick1", User = user2 };
            user2_nick1.Save(n => n.User);

            new Message() { Chat = chat, Author = user1_nick1, Date = new DateTime(2019, 01, 01), Text = "message1 from user1" }.Save(m => m.Author, m => m.Chat);
            new Message() { Chat = chat, Author = user2_nick1, Date = new DateTime(2019, 01, 02), Text = "message2 from user2" }.Save(m => m.Author, m => m.Chat);
            new Message() { Chat = chat, Author = user1_nick1, Date = new DateTime(2019, 01, 03), Text = "message3 from user1" }.Save(m => m.Author, m => m.Chat);
            new Message() { Chat = chat, Author = user2_nick1, Date = new DateTime(2019, 01, 04), Text = "message4 from user2" }.Save(m => m.Author, m => m.Chat);
            new Message() { Chat = chat, Author = user1_nick2, Date = new DateTime(2019, 01, 05), Text = "message5 from user1" }.Save(m => m.Author, m => m.Chat);
            new Message() { Chat = chat, Author = user2_nick1, Date = new DateTime(2019, 01, 06), Text = "message6 from user2" }.Save(m => m.Author, m => m.Chat);

            chat = dc.Read<Chat>(1);
            foreach (var message in chat.Messages)
                message.WithRelations(m => m.Author);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title =
#if DEBUG
                $"QIP Logger [DEBUG] v. {Settings.Version}";
#else
                "QIP Logger";
#endif
        }
    }
}
