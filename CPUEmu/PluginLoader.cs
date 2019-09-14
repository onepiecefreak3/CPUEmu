using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.DependencyInjection;
using CpuContract.Executor;
using CpuContract.Logging;
using CPUEmu.ServiceProviders;

namespace CPUEmu
{
    class PluginLoader
    {
        private WindsorContainer _container;
        private IList<object> _serviceProviders;

        public string PluginFolder { get; }

        public PluginLoader(ILogger logger, string pluginFolder)
        {
            PluginFolder = pluginFolder;

            _container = new WindsorContainer();
            _serviceProviders = new List<object>();
            RegisterServices();

            _container.Register(Component.For<ILogger>().Instance(logger));
        }

        public IServiceProvider<TService> GetServiceProvider<TService>()
        {
            var serviceProvider = _serviceProviders.FirstOrDefault(x => x is IServiceProvider<TService>);

            return (IServiceProvider<TService>)serviceProvider;
        }

        private void RegisterServices()
        {
            // Gather all assemblies
            var assemblies = GetAssemblies();

            // Register service providers
            RegisterServiceProvider<IAssemblyAdapter>(assemblies);
            RegisterServiceProvider<IExecutor>(assemblies);
            RegisterServiceProvider<ICpuState>(assemblies);
            RegisterServiceProvider<IInterruptBroker>(assemblies);
            RegisterServiceProvider<IArchitectureParser>(assemblies);
        }

        private Assembly[] GetAssemblies()
        {
            return Directory.GetFiles(PluginFolder, "*.dll", SearchOption.AllDirectories)
                .Select(x => Assembly.LoadFile(Path.GetFullPath(x)))
                .Concat(new[] { Assembly.GetAssembly(typeof(PluginLoader)) })
                .ToArray();
        }

        private void RegisterServiceProvider<TService>(Assembly[] assemblies)
        {
            var types = GetTypes<TService>(assemblies);

            // Register service provider
            var serviceProvider = new DefaultServiceProvider<TService>(_container, types);
            _serviceProviders.Add(serviceProvider);
            _container.Register(Component.For(typeof(IServiceProvider<TService>)).
                Instance(serviceProvider));

            // Register types themselves
            foreach (var type in types)
                _container.Register(Component.For(type.Item2));
        }

        private (string, Type)[] GetTypes<TService>(Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetExportedTypes())
                .Where(type => typeof(TService).IsAssignableFrom(type) && type.IsClass)
                .Where(type => type.GetCustomAttribute<UniqueIdentifierAttribute>() != null);

            return types.Select(type => (type.GetCustomAttribute<UniqueIdentifierAttribute>().UniqueIdentifier, type))
                .ToArray();
        }
    }
}
