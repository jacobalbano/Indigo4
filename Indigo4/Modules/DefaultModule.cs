using Indigo.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indigo.Modules
{
    internal class DefaultModule : IModule
    {
        string IModule.Name => "Indigo.Default";
    }
}
