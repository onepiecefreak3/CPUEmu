using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    class PluginLoader
    {
        private static readonly Lazy<PluginLoader> Lazy = new Lazy<PluginLoader>(() => new PluginLoader());
        public static PluginLoader Instance => Lazy.Value;

        [ImportMany]
        public IEnumerable<IAssemblyAdapter> Adapters { get; set; }

        public PluginLoader()
        {
            ComposeImports();
        }

        private void ComposeImports()
        {
            var catalog = new AssemblyCatalog(typeof(PluginLoader).Assembly);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}
