using SharedServices.Interfaces;
using System;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.AutoService.Services
{
#pragma warning disable CS0108
    public interface IMemberAmbiguityDemo_Parent1_Parent
    {
        /// <summary>
        /// This method matches exactly with the one in <see cref="IMemberAmbiguityDemo"/>
        /// with exception that return type is assignable from the return type of a similar method in <see cref="IMemberAmbiguityDemo"/>.
        /// The implementation will return <see cref="IMemberAmbiguityDemo.GetIntValues(int, string)"/>.
        /// </summary>
        IEnumerable<int> GetIntValues(int param1, string param2);

        /// <summary>
        /// A default implementation will be generated for this method.
        /// </summary>
        IEnumerable<int> GetIntValues(int param1, ref string param2);

        /// <summary>
        /// A default implementation will be generated for this method.
        /// </summary>
        IEnumerable<int> GetIntValues(out int param1, string param2);

        /// <summary>
        /// We have two methods in <see cref="IMemberAmbiguityDemo_Parent1_Parent"/> and <see cref="IMemberAmbiguityDemo_Parent2"/> with the same signature
        /// but different return types.
        /// We can configure both in configuration file, and auto-implement both.
        /// </summary>
        int GetNumericValue();
    }

    public interface IMemberAmbiguityDemo_Parent1 : IMemberAmbiguityDemo_Parent1_Parent
    {
        /// <summary>
        /// This method has the same signature as the one in <see cref="IMemberAmbiguityDemo"/>,
        /// however the return type is different.
        /// Therefore a default implementation will be generated.
        /// </summary>
        int GetIntValues(int param1, string param2);

        /// <summary>
        /// This method has the same signature and return value as the one in <see cref="IMemberAmbiguityDemo_Parent2"/>.
        /// Therefore, no additional implementation will be generated. Both interfaces will use the same implementation
        ///  based on data in configuration file
        /// </summary>
        IDbConnection GetDbConnection(Guid appGuid);

        /// <summary>
        /// An auto-implemented property will be generated based on data in configuration file.
        /// </summary>
        IDbConnection DefaultDbConnection { get; }

        /// <summary>
        /// An auto-implemented method will be generated based on data in configuration file.
        /// </summary>
        double NumericValue { get; }
    }
   
    public interface IMemberAmbiguityDemo_Parent2
    {
        /// <summary>
        /// This method has the same signature and return type as the one in <see cref="IMemberAmbiguityDemo"/>,
        /// therefore an implementation will be generated that returns <see cref="IMemberAmbiguityDemo.GetIntValues(int, string)"/>
        /// </summary>
        IReadOnlyList<int> GetIntValues(int param1, string param2);

        /// <summary>
        /// An auto-implemented method will be generated based on data in configuration file.
        /// </summary>
        IDbConnection GetDbConnection(Guid appGuid);

        /// <summary>
        /// We have two methods in <see cref="IMemberAmbiguityDemo_Parent1_Parent"/> and
        /// <see cref="IMemberAmbiguityDemo_Parent2"/> with the same signature
        /// but different return types.
        /// We can configure both in configuration file, and auto-implement both.
        /// </summary>
        double GetNumericValue();

        /// <summary>
        /// An auto-implemented property will be generated based on data in configuration file.
        /// </summary>
        IDbConnection DefaultDbConnection { get; }

        /// <summary>
        /// An auto-implemented method will be generated based on data in configuration file.
        /// </summary>
        int NumericValue { get; }
    }
    
    public interface IMemberAmbiguityDemo_Parent3
    {
        /// <summary>
        /// This method has the same signature and return type as the one in <see cref="IMemberAmbiguityDemo"/>.
        /// However, we provide an implementation in configuration file. Therefore, the returned values will be different>
        /// </summary>
        IReadOnlyList<int> GetIntValues(int param1, string param2);

        /// <summary>
        /// No auto-implemented property is configured in configuration file, however,
        /// an implementation will be generated that returns <see cref="IMemberAmbiguityDemo_Parent1.DefaultDbConnection"/>.
        /// </summary>
        IDbConnection DefaultDbConnection { get; set; }
    }
    
    public interface IMemberAmbiguityDemo : IMemberAmbiguityDemo_Parent1, IMemberAmbiguityDemo_Parent2, IMemberAmbiguityDemo_Parent3
    {
        /// <summary>
        /// An auto-implemented method will be generated based on data in configuration file.
        /// </summary>
        IReadOnlyList<int> GetIntValues(int param1, string param2);

        /// <summary>
        ///  An auto-implemented method will be generated based on data in configuration file.
        /// </summary>
        int MethodWithOptionalParameters(int param1, double param2 = 3.5, int param3=7);

        /// <summary>
        /// Currently there is no way to specify values for parameter "params int[] params4" in configuration
        /// file. This might change in the future. So for now, a default implementation will be generated for
        /// <see cref="MethodWithOptionalParameters(int, double, int, int[])"/>
        /// </summary>
        int MethodWithOptionalParameters(int param1, double param2 = 3.5, int param3 = 7, params int[] params4);
    }

#pragma warning restore CS0108
}
