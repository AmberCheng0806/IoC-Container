using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceCollection
    {
        public Dictionary<Type, List<ServiceItem>> keyValuePairs = new Dictionary<Type, List<ServiceItem>>();

        public void AddSingleton<T1, T2>()
        {
            if (keyValuePairs.ContainsKey(typeof(T1))) { keyValuePairs[typeof(T1)].Add(new ServiceItem(LifeTimeEnum.Singleton, typeof(T2), null)); return; };
            keyValuePairs[typeof(T1)] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Singleton, typeof(T2), null) };
        }

        public void AddTransient<T1, T2>()
        {
            if (keyValuePairs.ContainsKey(typeof(T1))) { keyValuePairs[typeof(T1)].Add(new ServiceItem(LifeTimeEnum.Transient, typeof(T2), null)); return; };
            keyValuePairs[typeof(T1)] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Transient, typeof(T2), null) };
        }
        public void AddSingleton(Type type1, Type type2)
        {
            if (keyValuePairs.ContainsKey(type1)) { keyValuePairs[type1].Add(new ServiceItem(LifeTimeEnum.Singleton, type2, null)); return; };
            keyValuePairs[type1] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Singleton, type2, null) };
        }

        public void AddTransient(Type type1, Type type2)
        {
            if (keyValuePairs.ContainsKey(type1)) { keyValuePairs[type1].Add(new ServiceItem(LifeTimeEnum.Transient, type2, null)); return; };
            keyValuePairs[type1] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Transient, type2, null) };
        }

        public void AddSingleton(Type type, Func<ServiceProvider, object> func)
        {
            if (keyValuePairs.ContainsKey(type)) { keyValuePairs[type].Add(new ServiceItem(LifeTimeEnum.Singleton, type, null, func)); return; };
            keyValuePairs[type] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Singleton, type, null, func) };
        }
        public void AddTransient(Type type, Func<ServiceProvider, object> func)
        {
            if (keyValuePairs.ContainsKey(type)) { keyValuePairs[type].Add(new ServiceItem(LifeTimeEnum.Transient, type, null, func)); return; };
            keyValuePairs[type] = new List<ServiceItem>() { new ServiceItem(LifeTimeEnum.Transient, type, null, func) };
        }
        public ServiceProvider BuildServiceProvider()
        {
            return new ServiceProvider(keyValuePairs);
        }
    }
}
