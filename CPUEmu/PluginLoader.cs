using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    class PluginLoader
    {
        private WindsorContainer _container;
        private ILogger _logger;
        private (Type, UniqueIdentifierAttribute)[] _assemblyAdapters;

        private Type _currentLoggerType;

        private static readonly Lazy<PluginLoader> Lazy = new Lazy<PluginLoader>(() => new PluginLoader());
        public static PluginLoader Instance => Lazy.Value;

        public PluginLoader()
        {
            _container = new WindsorContainer();

            RegisterAdapters();

            _container.Register(Component.For<ILogger>().UsingFactoryMethod(CreateLogger));
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public IAssemblyAdapter CreateAssemblyAdapter(string assemblyAdapterId)
        {
            if (_assemblyAdapters.All(x => x.Item2.UniqueIdentifier != assemblyAdapterId))
                throw new InvalidOperationException($"Unknown assemblyAdapterId '{assemblyAdapterId}'.");

            var assemblyType = _assemblyAdapters.First(x => x.Item2.UniqueIdentifier == assemblyAdapterId).Item1;
            return (IAssemblyAdapter)_container.Resolve(assemblyType);
        }

        private void RegisterAdapters()
        {
            _assemblyAdapters = typeof(PluginLoader).Assembly.GetTypes()
                .Where(x => x.IsClass && typeof(IAssemblyAdapter).IsAssignableFrom(x))
                .Where(x => x.GetCustomAttribute(typeof(UniqueIdentifierAttribute)) != null)
                .Select(x => (x, x.GetCustomAttribute<UniqueIdentifierAttribute>()))
                .ToArray();

            foreach (var assemblyAdapter in _assemblyAdapters)
            {
                _container.Register(Component.For(assemblyAdapter.Item1));
            }
        }

        private ILogger CreateLogger()
        {
            return _logger;
        }
    }
}
