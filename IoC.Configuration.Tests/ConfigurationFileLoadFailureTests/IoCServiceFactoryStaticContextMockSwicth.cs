using System;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    /// <summary>
    ///     Sets the value of <see cref="IoCServiceFactoryAmbientContext.Context" /> temporarily t
    ///     to a <see cref="IoCServiceFactoryMock" /> object. The <see cref="Dispose" />() method switches
    ///     the context back to the default value.
    /// </summary>
    public class IoCServiceFactoryStaticContextMockSwicth : IDisposable
    {
        #region  Constructors

        /// <summary>
        ///     The constructor sets <see cref="IoCServiceFactoryAmbientContext.Context" />
        ///     to a <see cref="IoCServiceFactoryMock" /> object. The <see cref="Dispose" />() method switches
        ///     the context back to the default value.
        /// </summary>
        public IoCServiceFactoryStaticContextMockSwicth(TypesListFactoryTypeGeneratorMock.ValidationFailureMethod typesListFactoryValidationFailureMethod)
        {
            IoCServiceFactoryAmbientContext.Context = new IoCServiceFactoryMock(IoCServiceFactoryAmbientContext.Context, typesListFactoryValidationFailureMethod);
        }

        #endregion

        #region IDisposable Interface Implementation

        public void Dispose()
        {
            IoCServiceFactoryAmbientContext.SetDefaultContext();
        }

        #endregion
    }
}