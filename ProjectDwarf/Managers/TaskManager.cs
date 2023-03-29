using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDwarf.Tasks;

namespace ProjectDwarf.Managers
{
    public class TaskManager
    {
        private static TaskManager instance;

        public static TaskManager Instance
        {
            get {
                if (instance == null)
                    instance = new TaskManager();

                return instance;
            }
        }

        public LinkedList<BaseTask> Tasks;

        public TaskManager()
        {
            Tasks = new LinkedList<BaseTask>();
        }

        public void AddTask(BaseTask task)
        {
            Tasks.AddFirst(task);
        }

    }
}
