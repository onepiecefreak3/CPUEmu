using System.Collections.Generic;

namespace CpuContract.DependencyInjection
{
    public interface IServiceProvider<TService>
    {
        TService GetService(string serviceName);

        IEnumerable<TService> EnumerateServices();

        void ReleaseService(TService service);
    }
}
