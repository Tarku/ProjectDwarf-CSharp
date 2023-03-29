using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public class Colony
    {
        private static Colony instance;

        public string Name { get; set; }

        public List<Dwarf> Members { get; set; }

        public static Colony Instance
        {
            get
            {
                if (instance == null)
                    instance = new Colony();

                return instance;
            } 
        }

        public Colony()
        {
            Members = new List<Dwarf>(Constants.PopulationCap);

            for (int i = 0; i < 20; i++)
                Members.Add(new Dwarf($"Dumbass {i + 1}", RNG.Int(20, 50)));
        }

        public void Update(GameTime gameTime)
        {
            foreach (Dwarf dwarf in Members)
                dwarf.Update(gameTime);
        }
    }
}
