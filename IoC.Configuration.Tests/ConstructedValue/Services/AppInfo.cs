namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class AppInfo : IAppInfo
    {
        public AppInfo(int id) : this(id, null)
        {
            
        }

        public AppInfo(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public int Id { get; }
        public string Description { get; set; }
    }
}