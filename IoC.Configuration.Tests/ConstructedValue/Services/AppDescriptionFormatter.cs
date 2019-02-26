using System.Text;

namespace IoC.Configuration.Tests.ConstructedValue.Services
{
    public class AppDescriptionFormatter : IAppDescriptionFormatter
    {
        public AppDescriptionFormatter(string prefixToAddToDescription)
        {
            PrefixToAddToDescription = prefixToAddToDescription;
        }

        public string PrefixToAddToDescription { get; }
        public string PostfixToAddToDescription { get; set; } = string.Empty;

        public IAppInfo UnormatDescription(IAppInfo appInfo)
        {
            var unformattedDescription = new StringBuilder();

            int descriptionStartIndex = 0;
            int descriptionLength = appInfo.Description.Length;
            if (appInfo.Description.StartsWith(PrefixToAddToDescription))
            {
                descriptionStartIndex = PrefixToAddToDescription.Length;
                descriptionLength -= PrefixToAddToDescription.Length;
            }

            if (appInfo.Description.EndsWith(PostfixToAddToDescription))
                descriptionLength -= PostfixToAddToDescription.Length;

            if (descriptionLength != appInfo.Description.Length)
                return new AppInfo(appInfo.Id, appInfo.Description.Substring(descriptionStartIndex, descriptionLength));

            return appInfo;
        }

        public IAppInfo FormatDescription(IAppInfo appInfo)
        {
            var formattedDescription = new StringBuilder();

            if (!appInfo.Description.StartsWith(PrefixToAddToDescription))
                formattedDescription.Append(PrefixToAddToDescription);

            formattedDescription.Append(appInfo.Description);

            if (!appInfo.Description.EndsWith(PostfixToAddToDescription))
                formattedDescription.Append(PostfixToAddToDescription);

            return new AppInfo(appInfo.Id, formattedDescription.ToString());
        }
    }
}