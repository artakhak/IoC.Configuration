using JetBrains.Annotations;

namespace IoC.Configuration.Tests
{
    public class DiImplementationInfo
    {
        #region  Constructors

        public DiImplementationInfo(DiImplementationType diImplementationType, [NotNull] string diManagerFolder,
                                    [NotNull] string diManagerAssemblyPath,
                                    [NotNull] string diManagerClassName,
                                    [NotNull] string diContainerClassName)
        {
            DiImplementationType = diImplementationType;
            DiManagerFolder = diManagerFolder;
            DiManagerAssemblyPath = diManagerAssemblyPath;
            DiManagerClassName = diManagerClassName;
            DiContainerClassName = diContainerClassName;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public string DiContainerClassName { get; }

        public DiImplementationType DiImplementationType { get; }

        [NotNull]
        public string DiManagerAssemblyPath { get; }

        [NotNull]
        public string DiManagerClassName { get; }

        [NotNull]
        public string DiManagerFolder { get; }

        #endregion
    }
}