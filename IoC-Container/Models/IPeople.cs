using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    internal interface IPeople
    {
        string Name { get; set; }
        void talk();
    }
}
