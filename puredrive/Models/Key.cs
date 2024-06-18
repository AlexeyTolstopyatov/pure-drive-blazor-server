
namespace puredrive.Models
{
    public class Key
    {
        public Key(string str)
        {
            Value = str;
        }

        public Key(bool rule, string value)
        {
            Rule = rule;
            Value = value;
        }

        public Key(bool rule)
        {
            Rule = rule;
            Value = "";
        }
        public Key() 
        {
            Value = string.Empty;
            Rule = false;
        }
        public bool Rule { get; private set; }
        public string Value { get; private set; }
    }
}
