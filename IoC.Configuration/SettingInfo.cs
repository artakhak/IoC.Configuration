using System;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    //public enum SettingValueType
    //{
    //    Byte,
    //    Int16,
    //    Int32,
    //    Int64,
    //    Double,
    //    Boolean,
    //    DateTime,
    //    String,
    //    Object
    //}
    public class SettingInfo
    {
        #region  Constructors

        public SettingInfo([NotNull] string name, [NotNull] Type valueDataType)
        {
            Name = name;
            ValueDataType = valueDataType;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public string Name { get; }

        [NotNull]
        public Type ValueDataType { get; }

        #endregion
    }
}