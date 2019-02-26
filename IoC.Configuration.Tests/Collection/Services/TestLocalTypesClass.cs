namespace IoC.Configuration.Tests.Collection.Services
{
    public class TestLocalTypesClass
    {
        public interface IInterface1
        {
            int Value { get; }
        }

        public class Interface1_Impl1 : IInterface1
        {
            public Interface1_Impl1(int value)
            {
                Value = value;
            }
            public int Value { get; }
        }
    }
}
