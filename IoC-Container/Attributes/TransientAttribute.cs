using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Attributes
{
    public class TransientAttribute : Attribute
    {
        public string ServiceKey { get; set; }
        public TransientAttribute(string key = "") { ServiceKey = key; }
    }
}
