using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bricklayer.API
{
    public interface IPluginHost
    {
        List<IPlugin> Plugins { get; set; }
        void RegisterPlugin(IPlugin plugin);
        void LoadPlugins();
    }
}
