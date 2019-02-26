using System.Collections.Generic;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.Collection.Services
{
    public class CollectionsTestClass1
    {
        public CollectionsTestClass1(IReadOnlyList<int> readOnlyListParam, IInterface1[] arrayParam)
        {
            ReadOnlyListValues = readOnlyListParam;
            ArrayValues = arrayParam;
        }

        public IReadOnlyList<int> ReadOnlyListValues { get; }
        public IInterface1[] ArrayValues { get; }

        public IEnumerable<IInterface1> EnumerableValues { get; set; }
        public List<IInterface1> ListValues { get; set; }
    }
}
