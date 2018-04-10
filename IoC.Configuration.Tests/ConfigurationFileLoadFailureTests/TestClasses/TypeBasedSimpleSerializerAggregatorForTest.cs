using OROptimizer.Serializer;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests.TestClasses
{
    public class TypeBasedSimpleSerializerAggregatorForTest : TypeBasedSimpleSerializerAggregator
    {
        #region  Constructors

        public TypeBasedSimpleSerializerAggregatorForTest(int param1)
        {
            Property1 = param1;
        }

        #endregion

        #region Member Functions

        public int Property1 { get; }

        #endregion
    }
}