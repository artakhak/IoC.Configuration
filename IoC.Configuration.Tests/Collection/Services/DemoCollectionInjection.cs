using System.Collections.Generic;

namespace IoC.Configuration.Tests.Collection.Services
{
    public class DemoCollectionInjection
    {
        public DemoCollectionInjection(IReadOnlyList<int> intValues)
        {
            IntValues = intValues;
        }

        public IReadOnlyList<int> IntValues { get; }
        public IReadOnlyList<string> Texts { get; set; }
    }
}