using IoC_Container.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var collection = new ServiceCollection();


            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<GTA, GTA>();
            services.AddSingleton(typeof(IPlatformGame<>), typeof(PCGame<>));
            services.AddSingleton(typeof(IAnimal), typeof(Animal));
            services.AddSingleton(typeof(Computer), typeof(Computer));
            services.AddSingleton(typeof(IPeople), typeof(People));
            object obj = services.BuildServiceProvider().Get(typeof(IPeople));
            ((IPeople)obj).talk();
            //services.AddTransient(typeof(IPeople), typeof(People));

            ////IPeople people1 = (IPeople)services.Get(typeof(IPeople));
            //List<IPeople> people1 = (List<IPeople>)services.GetAll(typeof(IPeople));
            //people1.First().Name = "test1";
            ////IPeople people2 = (IPeople)services.Get(typeof(IPeople));
            //List<IPeople> people2 = (List<IPeople>)services.GetAll(typeof(IPeople));
            //Console.WriteLine(people2.First().Name);


            //services.AddSingleton(typeof(IPeople), () =>
            //{
            //    return new People(new PCGame<GTA>());
            //});
            //IPeople people = (IPeople)services.Get(typeof(IPeople));
            //people.talk();
            Console.ReadKey();
        }

    }
}
