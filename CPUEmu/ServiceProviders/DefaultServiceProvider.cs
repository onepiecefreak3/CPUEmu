using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using CpuContract.DependencyInjection;

namespace CPUEmu.ServiceProviders
{
    public class DefaultServiceProvider<TService> : IServiceProvider<TService>
    {
        private IWindsorContainer _container;
        private (string, Type)[] _adapterTypes;

        public DefaultServiceProvider(IWindsorContainer container, (string, Type)[] adapterTypes)
        {
            if (adapterTypes.Any(x => !typeof(TService).IsAssignableFrom(x.Item2)))
                throw new InvalidOperationException($"All types need to be assignable from '{typeof(TService)}'.");

            _container = container;
            _adapterTypes = adapterTypes;
        }

        public TService GetService(string serviceName)
        {
            var selectedType = _adapterTypes.FirstOrDefault(x => x.Item1 == serviceName).Item2;
            if (selectedType == null)
                return default;

            return (TService)_container.Resolve(selectedType);
        }

        public IEnumerable<TService> EnumerateServices()
        {
            return _adapterTypes.Select(x => (TService)_container.Resolve(x.Item2));
        }

        public void ReleaseService(TService service)
        {
            _container.Release(service);
        }
    }
}
