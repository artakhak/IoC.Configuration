﻿using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface8_Impl1 : IInterface8
    {
        #region  Constructors

        public Interface8_Impl1()
        {
        }

        public Interface8_Impl1(IInterface9 param1)
        {
            Property1 = param1;
        }

        #endregion

        #region IInterface8 Interface Implementation

        public IInterface9 Property1 { get; }

        #endregion
    }
}