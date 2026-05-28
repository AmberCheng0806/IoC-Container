using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceProvider
    {
        public Dictionary<Type, List<ServiceItem>> KeyValuePairs;
        public ServiceProvider(Dictionary<Type, List<ServiceItem>> keyValuePairs)
        {
            KeyValuePairs = keyValuePairs;
        }
        public T Get<T>()
        {
            List<ServiceItem> serviceItems = KeyValuePairs[typeof(T)];
            ServiceItem serviceItem = serviceItems.FirstOrDefault();
            if (serviceItem.LifeTimeEnum == LifeTimeEnum.Singleton && serviceItem.Item != null) return (T)serviceItem.Item;
            object item;
            if (serviceItem.Func != null) { item = serviceItem.Func(this); }
            else { item = CreateInstance(serviceItem.Type); }
            if (serviceItem.LifeTimeEnum == LifeTimeEnum.Singleton)
            {
                serviceItem.Item = item;
            }
            return (T)item;
        }
        public List<T> GetAll<T>()
        {
            List<ServiceItem> serviceItems = KeyValuePairs[typeof(T)];
            return serviceItems.Select(x =>
            {
                if (x.LifeTimeEnum == LifeTimeEnum.Singleton && x.Item != null) return (T)x.Item;
                var item = (T)CreateInstance(x.Type);
                if (x.LifeTimeEnum == LifeTimeEnum.Singleton) x.Item = item;
                return item;
            }).ToList();

        }
        public object Get(Type type)
        {
            List<ServiceItem> serviceItems = KeyValuePairs[type];
            ServiceItem serviceItem = serviceItems.FirstOrDefault();
            if (serviceItem.LifeTimeEnum == LifeTimeEnum.Singleton && serviceItem.Item != null) return serviceItem.Item;
            object item;
            if (serviceItem.Func != null) { item = serviceItem.Func(this); }
            else { item = CreateInstance(serviceItem.Type); }
            if (serviceItem.LifeTimeEnum == LifeTimeEnum.Singleton)
            {
                serviceItem.Item = item;
            }
            return item;
        }
        public object GetAll(Type type)     // object => List<object>  /   object => List<IPeople>
        {
            List<ServiceItem> serviceItems = KeyValuePairs[type];
            Type listType = typeof(List<>).MakeGenericType(type);
            IList list = (IList)Activator.CreateInstance(listType); //list<type>
            serviceItems.ForEach(x =>
            {
                if (x.LifeTimeEnum == LifeTimeEnum.Singleton && x.Item != null) { list.Add(x.Item); }
                else
                {
                    var item = CreateInstance(x.Type);
                    if (x.LifeTimeEnum == LifeTimeEnum.Singleton) x.Item = item;
                    list.Add(item);
                }
            });
            return list;
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
                        if (x.ParameterType.IsGenericType && x.ParameterType.GetInterfaces().Any(y => y.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                        {
                            Type argument = x.ParameterType.GetGenericArguments()[0];
                            if (!KeyValuePairs.ContainsKey(argument)) return null;
                            return GetAll(argument);
                        }


                        //空泛型介面 PCgame<>
                        // 你拿到的: IPlatformGame<GTA> (無法new interface出來) 實際上要注入的: PCGame<GTA>
                        // 需要從IPlatformGame 反找到PCGame
                        // 再將 GTA 放入 PCGame
                        // 最後才能 new PCGame
                        if (x.ParameterType.IsGenericType && KeyValuePairs.ContainsKey(x.ParameterType.GetGenericTypeDefinition()))
                        {
                            Type genericType = x.ParameterType.GetGenericTypeDefinition();
                            Type implentmentType = KeyValuePairs[genericType].Last().Type;
                            Type argument = x.ParameterType.GetGenericArguments()[0];
                            Type result = implentmentType.MakeGenericType(argument); //PCgame<GTA>
                            return CreateInstance(result);
                        }
                        //其他類別+介面
                        if (!KeyValuePairs.ContainsKey(x.ParameterType)) return null;
                        Type implentment = KeyValuePairs[x.ParameterType].Last().Type;
                        return CreateInstance(implentment);
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
    }
}
