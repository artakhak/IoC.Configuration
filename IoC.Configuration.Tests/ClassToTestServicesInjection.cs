using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests
{
    public class ClassToTestServicesInjection<TService>
    {
        #region Member Variables

        private readonly List<TService> _implmentations;

        #endregion

        #region  Constructors

        public ClassToTestServicesInjection([NotNull] IEnumerable<TService> services)
        {
            _implmentations = new List<TService>(services);
        }

        #endregion

        #region Member Functions

        [NotNull]
        public IList<TService> Implmentations => _implmentations;

        public Type ServiceType => typeof(TService);

        public void ValidateDoesNotHaveImplementation(string implementationType)
        {
            Assert.IsFalse(_implmentations.Any(x => x.GetType().FullName.Equals(implementationType, StringComparison.Ordinal)));
        }

        public void ValidateHasImplementation(Type implementationType)
        {
            Assert.IsTrue(_implmentations.Where(x => x.GetType() == implementationType).ToList().Count == 1);
        }

        public void ValidateImplementationTypes(IEnumerable<string> implementationTypeNames)
        {
            var implementationTypeNamesList = new List<string>(implementationTypeNames);

            Assert.AreEqual(implementationTypeNamesList.Count, Implmentations.Count);

            for (var i = 0; i < _implmentations.Count; ++i)
                Assert.AreEqual(_implmentations[i].GetType().FullName, implementationTypeNamesList[i], false);
        }

        #endregion
    }
}