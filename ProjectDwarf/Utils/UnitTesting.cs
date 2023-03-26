using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Utils
{
    public class UnitTesting
    {
        private static UnitTesting instance = null;

        public static UnitTesting Instance
        {
            get
            {
                if (instance == null)
                    instance = new UnitTesting();

                return instance;
            }
        }


        public void Do()
        {

        }

        // Unit testing code below
        UnitTesting() {}
    }
}
