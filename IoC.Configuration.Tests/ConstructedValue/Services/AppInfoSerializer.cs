namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class AppInfoSerializer : OROptimizer.Serializer.TypeBasedSimpleSerializerAbstr<IAppInfo>
    {
        public IAppDescriptionFormatter AppDescriptionFormatter { get; }

        public AppInfoSerializer(IAppDescriptionFormatter appDescriptionFormatter)
        {
            AppDescriptionFormatter = appDescriptionFormatter;
        }
       
        public override bool TryDeserialize(string valueToDeserialize, out IAppInfo appInfo)
        {
            appInfo = null;

            var values = valueToDeserialize.Split(',');

            if (values?.Length != 2)
                return false;

            if (!int.TryParse(values[0], out var id))
                return false;

            appInfo = new AppInfo(id, values[1]);

            appInfo = AppDescriptionFormatter.FormatDescription(appInfo);
            return true;
        }

        public override string Serialize(IAppInfo appInfo)
        {
            return $"{appInfo.Id}, {appInfo.Description}";
        }

        
        public override string GenerateCSharpCode(IAppInfo appInfo)
        {
            return $"new {typeof(AppInfo).FullName}({appInfo.Id}, @\"{appInfo.Description}\")";
        }
    }
}
