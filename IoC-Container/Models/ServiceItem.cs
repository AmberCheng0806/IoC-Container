using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class ServiceItem
    {
        public LifeTimeEnum LifeTimeEnum { get; set; }
        public Type Type { get; set; }
        public object Item { get; set; }
        public Func<ServiceProvider, object> Func { get; set; }
        public ServiceItem(LifeTimeEnum lifeTime, Type type, object item)
        {
            LifeTimeEnum = lifeTime;
            Type = type;
            Item = item;
        }

        public ServiceItem(LifeTimeEnum lifeTimeEnum, Type type, object item, Func<ServiceProvider, object> func) : this(lifeTimeEnum, type, item)
        {
            Func = func;
        }
    }
}
