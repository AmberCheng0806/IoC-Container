using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Models
{
    internal class PCGame<T> : IPlatformGame<T>
    {
        private Computer Computer;
        public PCGame(Computer computer)
        {
            Computer = computer;
        }
        public void Talk()
        {
            Console.WriteLine(typeof(T).Name);
        }
        public void Play()
        {
            Computer.Play();
        }
    }
}
