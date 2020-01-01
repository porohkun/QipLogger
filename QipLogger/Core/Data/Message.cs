using Iridium.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QipLogger.Core.Data
{
    public class Message : IEntity
    {
        [Column.PrimaryKey(AutoIncrement = true)]
        public int ID { get; set; }


        public int ChatID;
        [Relation.ManyToOne(LocalKey = nameof(ChatID), ForeignKey = nameof(Data.Chat.ID))]
        public Chat Chat
        {
            get => _chat;
            set { ChatID = value.ID; _chat = value; }
        }
        private Chat _chat;


        public int AuthorID;
        [Relation.ManyToOne(LocalKey = nameof(AuthorID), ForeignKey = nameof(UserInfo.ID))]
        public UserInfo Author
        {
            get => _author;
            set { AuthorID = value.ID; _author = value; }
        }
        private UserInfo _author;


        [Column.NotNull]
        public DateTime Date { get; set; }


        [Column.LargeText]
        public string Text { get; set; }
    }
}
