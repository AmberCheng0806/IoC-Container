using IoC_Container.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    internal class People : IPeople
    {
        private string _name;
        public string Name { get => _name; set => _name = value; }
        private IPlatformGame<GTA> platformGame;
        private IAnimal animal;

        public People(IPlatformGame<GTA> game, IAnimal animal)
        {
            platformGame = game;
            this.animal = animal;
        }

        public People(IPlatformGame<GTA> game, IAnimal animal, string name)
        {
            platformGame = game;
            this.animal = animal;
            Name = name;
        }

        public void talk()
        {
            platformGame.Talk();
            platformGame.Play();
            animal.Talk();
        }
    }
}
