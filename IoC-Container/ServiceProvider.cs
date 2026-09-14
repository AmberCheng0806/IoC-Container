using IoC_Container.Attributes;
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
    public class ServiceProvider : IServiceProvider
    {
        public Dictionary<Type, List<ServiceDescriptor>> KeyValuePairs;
        public Dictionary<ServiceDescriptor, object> Instances = new Dictionary<ServiceDescriptor, object>();
        public ServiceProvider(Dictionary<Type, List<ServiceDescriptor>> keyValuePairs)
        {
            KeyValuePairs = keyValuePairs;
        }
        private T Get<T>()
        {
            return (T)Get(typeof(T));
        }
        private List<T> GetList<T>()
        {
            return (List<T>)GetList(typeof(T));
        }
        private object Get(Type type)
        {
            List<ServiceDescriptor> serviceItems = KeyValuePairs[type];
            ServiceDescriptor serviceItem = serviceItems.LastOrDefault();
            return GetInstance(serviceItem, type);
        }
        private object Get(Type type, string key)
        {
            List<ServiceDescriptor> serviceItems = KeyValuePairs[type];
            ServiceDescriptor serviceItem = serviceItems.LastOrDefault(x => x.ServiceKey != null && x.ServiceKey.ToString() == key);
            return GetInstance(serviceItem, type);
        }
        private object GetList(Type type)     // object => List<object>  /   object => List<IPeople>
        {
            List<ServiceDescriptor> serviceItems = KeyValuePairs[type];
            Type listType = typeof(List<>).MakeGenericType(type);
            IList list = (IList)Activator.CreateInstance(listType); //list<type>
            serviceItems.ForEach(x =>
            {
                object item = GetInstance(x, type);
                list.Add(item);
            });
            return list;
        }
        private object GetInstance(ServiceDescriptor descriptor, Type serviceType)
        {
            if (descriptor.Lifetime == ServiceLifetime.Singleton && Instances.ContainsKey(descriptor))
            {
                return Instances[descriptor];
            }
            object item;
            if (descriptor.ImplementationInstance != null)
            {
                item = descriptor.ImplementationInstance;
            }
            else if (descriptor.ImplementationFactory != null)
            {
                item = descriptor.ImplementationFactory(this);
            }
            else if (descriptor.ImplementationType != null)
            {
                item = CreateInstance(descriptor.ImplementationType);
            }
            else
            {
                item = CreateInstance(descriptor.KeyedImplementationType);
            }
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                Instances[descriptor] = item;
            }
            return item;
        }

        private object CreateInstance(Type type)
        {
            ConstructorInfo[] constructorInfos = type.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).ToArray();
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parms = constructorInfo.GetParameters();
                object item = null;
                if (parms.Length != 0)
                {
                    var parmInstances = parms.Select(x =>
                    {
                        //Ienumerable<Animal>
                        if (x.ParameterType.IsGenericType && (x.ParameterType.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || x.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                        {
                            Type argument = x.ParameterType.GetGenericArguments()[0];
                            if (!KeyValuePairs.ContainsKey(argument)) return null;
                            return GetList(argument);
                        }


                        //空泛型介面 PCgame<>
                        // 拿到的: IPlatformGame<GTA> (無法new interface出來) 實際上要注入的: PCGame<GTA>
                        // 需要從IPlatformGame 反找到PCGame
                        // 再將 GTA 放入 PCGame
                        // 最後才能 new PCGame
                        if (x.ParameterType.IsGenericType && KeyValuePairs.ContainsKey(x.ParameterType.GetGenericTypeDefinition()))
                        {
                            Type genericType = x.ParameterType.GetGenericTypeDefinition();
                            Type implentmentType = KeyValuePairs[genericType].Last().ImplementationType != null ?
                           KeyValuePairs[genericType].Last().ImplementationType : KeyValuePairs[genericType].Last().KeyedImplementationType;
                            Type[] arguments = x.ParameterType.GetGenericArguments();
                            Type result = implentmentType.MakeGenericType(arguments); //PCgame<GTA>
                            return CreateInstance(result);
                        }
                        //其他類別+介面
                        if (!KeyValuePairs.ContainsKey(x.ParameterType)) return null;
                        //Type implentment = KeyValuePairs[x.ParameterType].Last().ImplementationType;
                        //return CreateInstance(implentment);

                        //從建構元參數拿注入實體key
                        var key = x.GetCustomAttribute<GetInstanceAttribute>()?.ServiceKey;
                        if (!string.IsNullOrEmpty(key))
                        {
                            return Get(x.ParameterType, key);
                        }
                        return Get(x.ParameterType);
                    }).ToArray();
                    if (parmInstances.Any(x => x == null)) continue;
                    item = Activator.CreateInstance(type, parmInstances);
                }
                else
                {
                    item = Activator.CreateInstance(type);
                }
                return item;
            }
            return null;
        }

        public object GetService(Type serviceType)
        {
            if (!KeyValuePairs.ContainsKey(serviceType))
            {
                return null;
            }
            if (KeyValuePairs[serviceType].Count > 1)
            {
                return GetList(serviceType);
            }
            return Get(serviceType);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
    }
}
