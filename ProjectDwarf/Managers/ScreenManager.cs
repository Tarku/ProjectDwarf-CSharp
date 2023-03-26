using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDwarf.Screens;

namespace ProjectDwarf.Managers
{
    public class ScreenManager
    {
        private static ScreenManager instance;

        public BaseScreen CurrentScreen { private set; get; }

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();

                return instance;
            }
        }

        public void ChangeScreen(BaseScreen screen)
        {
            CurrentScreen?.UnloadContent();

            CurrentScreen = screen;
            CurrentScreen.LoadContent();
        }
    }
}
