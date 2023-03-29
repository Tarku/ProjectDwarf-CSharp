using Microsoft.Xna.Framework;
using ProjectDwarf.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf.Tasks
{
    public class MiningTask : BaseTask
    {
        public MiningTask(Vector3 position) : base(position)
        {

        }

        public MiningTask(Vector3 position, TaskPriority priority) : base(position, priority)
        {

        }

        public override void Do(Dwarf doer)
        {
            base.Do(doer);

            doer.IsWorking = true;

            Parcel.Instance.SetTile(Position, 0);


            Free();
        }
        public override void Free()
        {
            Taker.workload.RemoveFirst();
            TaskManager.Instance.Tasks.Remove(this);

            Console.WriteLine($"Mining Task done, {TaskManager.Instance.Tasks.Where(item => item is MiningTask).Count()} mining tasks left.\n----");

            Taker.IsWorking = false;
            Taker = null;
        }
    }
}
