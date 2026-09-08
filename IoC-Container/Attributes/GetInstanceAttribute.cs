using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Attributes
{
    public class GetInstanceAttribute : Attribute
    {
        public string ServiceKey { get; set; }
        public GetInstanceAttribute(string key = "") { ServiceKey = key; }
    }
}
