using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Attributes
{
    public class SingletonAttribute : Attribute
    {
        public string ServiceKey { get; set; }
        public SingletonAttribute(string serviceKey = "")
        {
            this.ServiceKey = serviceKey;
        }
    }
}
