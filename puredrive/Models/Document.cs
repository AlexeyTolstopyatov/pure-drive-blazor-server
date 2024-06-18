namespace puredrive.Models
{
    public class Document
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Check { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }

        public Document(string id, string name, string check, string type, string size) 
        {
            Id = id;
            Name = name;
            Check = check;
            Type = type;
            Size = size;
        }
    }
}
