using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Configuration
{
    public static class Instantiator
    {
        public static T FromConfig<T>(IConfig<T> config) where T : IConfigurableObject
        {
            throw new NotImplementedException();
        }

        public static T FromConfig<T>(IConfig<T> config, Locator locator) where T : IConfigurableObject
        {
            throw new NotImplementedException();
        }
    }
}
