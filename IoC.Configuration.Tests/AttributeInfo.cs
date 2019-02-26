namespace IoC.Configuration.Tests
{
    public class AttributeInfo
    {
        public AttributeInfo(string name, string value)
        {
            Name = name;
            Value = value;

        }
        public string Name { get; }
        public string Value { get; }
    }
}