using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Models
{
    internal interface IPlatformGame<T>
    {
        void Talk();
        void Play();
    }
}
