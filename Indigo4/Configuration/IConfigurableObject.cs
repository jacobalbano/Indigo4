using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Configuration
{
    public interface IConfigurableObject
    {
    }

    public interface IConfig<T>
    {
        void Validate();
    }

    public interface ICanInstantiate<T, TSelf> where TSelf : IConfig<T>
    {
        T CreateInstance(Locator locator);
    }
}
