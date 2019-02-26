namespace IoC.Configuration.Tests.ClassMember.Services
{
    public class ConstAndStaticAppIds
    {
        public const int AppId1 = 17;
        public const string App1Description = "App 1";

        public static int AppId2 = 18;
        public static string App2Description = "App 2";

        public static int AppId3 => 19;
        public static string GetApp3Description() => "App 3";

        public static int GetAppId4() => 20;

        public const int DefaultAppId = int.MaxValue;
        public const string DefaultAppDescription = "Default App";
        
    }
}