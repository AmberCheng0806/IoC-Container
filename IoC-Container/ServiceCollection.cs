using IoC_Container.Attributes;
using IoC_Container.Factory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceCollection : IServiceCollection
    {
        public Dictionary<Type, List<ServiceDescriptor>> keyValuePairs = new Dictionary<Type, List<ServiceDescriptor>>();

        public int Count => keyValuePairs.Count;
        public bool IsReadOnly => false;

        public ServiceDescriptor this[int index]
        {
            get
            {
                int count = 0;
                foreach (var item in keyValuePairs)
                {
                    if (count != index) { count++; continue; };
                    return item.Value.LastOrDefault();
                }
                return null;
            }
            set
            {
                int count = 0;
                foreach (var item in keyValuePairs)
                {
                    if (count != index) { count++; continue; };
                    item.Value.Add(value);
                }
            }
        }

        public void AddSingleton<T1, T2>(string key = "")
        {
            Add(new ServiceDescriptor(typeof(T1), key, typeof(T2), ServiceLifetime.Singleton));
        }

        public void AddTransient<T1, T2>(string key = "")
        {
            Add(new ServiceDescriptor(typeof(T1), key, typeof(T2), ServiceLifetime.Transient));
        }
        public void AddSingleton(Type type1, Type type2, string key = "")
        {
            Add(new ServiceDescriptor(type1, key, type2, ServiceLifetime.Singleton));
        }

        public void AddTransient(Type type1, Type type2, string key = "")
        {
            Add(new ServiceDescriptor(type1, key, type2, ServiceLifetime.Transient));
        }

        public void AddSingleton(Type type, Func<IServiceProvider, object?, object> func, string key = "")
        {
            Add(new ServiceDescriptor(type, key, func, ServiceLifetime.Singleton));
        }
        public void AddTransient(Type type, Func<IServiceProvider, object?, object> func, string key = "")
        {
            Add(new ServiceDescriptor(type, func, ServiceLifetime.Transient));
        }

        public void AddSingleton<T>(Func<IServiceProvider, object?, object> func, string key = "")
        {
            Add(new ServiceDescriptor(typeof(T), key, func, ServiceLifetime.Singleton));
        }
        public void AddTransient<T>(Func<IServiceProvider, object?, object> func, string key = "")
        {
            Add(new ServiceDescriptor(typeof(T), key, func, ServiceLifetime.Transient));
        }
        public ServiceProvider BuildServiceProvider()
        {
            ServiceProvider serviceProvider = new ServiceProvider(keyValuePairs);
            Add(new ServiceDescriptor(typeof(IServiceProvider), serviceProvider));
            Add(new ServiceDescriptor(typeof(IPresenterFactory), "", typeof(PresenterFactory), ServiceLifetime.Singleton));
            AutoRegister();
            return serviceProvider;
        }

        private void AutoRegister()
        {
            var singleTonTypes = Assembly.GetEntryAssembly().GetTypes().Where(x => x.GetCustomAttribute<SingletonAttribute>() != null);
            foreach (var item in singleTonTypes)
            {
                string key = item.GetCustomAttribute<SingletonAttribute>().ServiceKey;
                if (key != "")
                {
                    AddSingleton(item.GetInterfaces().FirstOrDefault(), item, key);
                    continue;
                }
                AddSingleton(item, item, key);
            }
            var transientTonTypes = Assembly.GetEntryAssembly().GetTypes().Where(x => x.GetCustomAttribute<TransientAttribute>() != null);
            foreach (var item in transientTonTypes)
            {
                string key = item.GetCustomAttribute<TransientAttribute>().ServiceKey;
                if (key != "")
                {
                    AddTransient(item.GetInterfaces().FirstOrDefault(), item, key);
                    continue;
                }
                AddTransient(item, item, key);
            }
            //presenter
            var presenters = Assembly.GetEntryAssembly().GetTypes().Where(x => x.Name.EndsWith("Presenter") && x.IsClass);
            foreach (var item in presenters)
            {
                AddSingleton(item.GetInterfaces().FirstOrDefault(), item, "");
            }
        }

        public int IndexOf(ServiceDescriptor item)
        {
            int index = 0;
            foreach (var descriptor in keyValuePairs.SelectMany(x => x.Value))
            {
                if (descriptor.ImplementationType != item.ImplementationType) continue;
                index++;
                return index;
            };
            return index;
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(ServiceDescriptor item)
        {
            if (keyValuePairs.ContainsKey(item.ServiceType)) { keyValuePairs[item.ServiceType].Add(item); return; };
            keyValuePairs[item.ServiceType] = new List<ServiceDescriptor>() { item };
        }

        public void Clear()
        {
            keyValuePairs.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            foreach (var descriptor in keyValuePairs.SelectMany(x => x.Value))
            {
                if (descriptor.ImplementationType == item.ImplementationType) return true;
            };
            return false;
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            //keyValuePairs.SelectMany(x => x.Value).ToList().CopyTo(array, arrayIndex);
            throw new NotImplementedException();

        }

        public bool Remove(ServiceDescriptor item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
