using Microsoft.Xna.Framework;
using ProjectDwarf.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Tasks
{
    public class BaseTask
    {
        public TaskPriority Priority;
        public Vector3 Position;

        public Dwarf Taker;

        public bool IsDone = false;

        public bool IsTaken()
        {
            return Taker != null;
        }

        public virtual void Free()
        {
            Taker.workload.Remove(this);
            TaskManager.Instance.Tasks.Remove(this);

            Taker.IsWorking = false;
            Taker = null;
        }

        public BaseTask(Vector3 position)
        {
            Position = position;
            Priority = TaskPriority.Medium;
        }

        public BaseTask(Vector3 position, TaskPriority priority)
        {
            Position = position;
            Priority = priority;

        }

        public virtual void Do(Dwarf doer)
        {
            Taker = doer;
        }
    }
}
