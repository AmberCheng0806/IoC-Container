using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    internal class Animal : IAnimal
    {
        public void Talk()
        {
            Console.WriteLine("456");
        }
    }
}
