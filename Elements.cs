using Simulation;
using System.Numerics;

namespace Elements
{
    internal class Grass : Element
    {
        public Grass()
        {
            Description.Name4 = "GRAS";
            Description.Category = "Organics";
        }
        //on spawn
        public override void Spawn()
        {

        }
        //on update
        public override void Update()
        {

        }


    }

    internal class Brick : Element
    {
        public Brick()
        {
            Description.Name4 = "BRCK";
            Description.Category = "Materials";
        }

        //on spawn
        public override void Spawn()
        {

        }
        //on update
        public override void Update()
        {

        }


    }
}