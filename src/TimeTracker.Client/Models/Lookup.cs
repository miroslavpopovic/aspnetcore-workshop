namespace TimeTracker.Client.Models
{
    public class Lookup
    {
        public Lookup()
        {
        }

        public Lookup(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
