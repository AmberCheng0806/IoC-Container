using IoC_Container;
using IoC_Container.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Factory
{
    public class PresenterFactory : IPresenterFactory
    {
        private ServiceProvider provider;

        public PresenterFactory(IServiceProvider provider)
        {
            this.provider = (ServiceProvider)provider;
        }
        public TPresenter Create<TPresenter>(object view)
        {
            var descriptor = provider.KeyValuePairs[typeof(TPresenter)].Last();
            Type type = descriptor.ImplementationType != null ? descriptor.ImplementationType : descriptor.KeyedImplementationType;
            foreach (var item in type.GetConstructors().OrderByDescending(x => x.GetParameters().Length))
            {
                object[] args = item.GetParameters().Select(x =>
                { if (x.ParameterType.IsInstanceOfType(view)) { return view; }; return provider.GetService(x.ParameterType); }
                ).ToArray();
                return (TPresenter)Activator.CreateInstance(type, args);
            }
            return (TPresenter)Activator.CreateInstance(type, view);
        }
    }
}
