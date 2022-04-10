using System;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests.AutoServiceCustom.SimpleDataRepository.RepositoryAttributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DataRepositoryAttribute : Attribute
    {
        public DataRepositoryAttribute([NotNull] Type databaseEntityType)
        {
            DatabaseEntityType = databaseEntityType;
        }

        [NotNull]
        public Type DatabaseEntityType { get; }
    }
}
