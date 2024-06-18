namespace puredrive.Models
{
    public class Group
    {
        public static readonly string NO_GROUP = "[НЕТ]";
        public int Id { get; set; } 
        public string Content { get; set; }

        public Group(int id, string content)
        {
            Id = id;
            Content = content;
        }

        public Group() 
        {
            Id = 0;
            Content = NO_GROUP;
        }
    }
}
