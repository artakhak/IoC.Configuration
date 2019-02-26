using System;
using System.Collections.Generic;
using System.Text;

namespace SharedServices.Interfaces
{
    public interface IGenericInterface1<T> where T: class
    {
    }

    public interface IGenericInterface2<T,K> where T : class
    {
    }


}
