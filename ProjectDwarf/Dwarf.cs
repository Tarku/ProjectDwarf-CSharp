using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectDwarf.Utils;
using ProjectDwarf.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectDwarf.Managers;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework.Input;
using ProjectDwarf.Tiles;

namespace ProjectDwarf
{
    public class Dwarf
    {
        const int spawnScattering = 5;
        const float timeToMove = 500f;

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        Vector3 pathfindingTarget;

        bool isPathfinding = false;

        public LinkedList<BaseTask> workload;
        public BaseTask currentTask;

        public bool IsWorking = false;

        public string Name { get; set; }
        public int Age { get; set; }

        float moveTimer = 0f;

        float framesAlive = 0f;

        public Dwarf(string name, int age)
        {
            workload = new LinkedList<BaseTask>();


            Name = name;
            Age = age;

            byte spawnY = Constants.SurfaceLevel;

            int randX = RNG.InclInt(Constants.ParcelWidth / 2 - spawnScattering, Constants.ParcelWidth / 2 + spawnScattering);
            int randZ = RNG.InclInt(Constants.ParcelWidth / 2 - spawnScattering, Constants.ParcelWidth / 2 + spawnScattering);

            while (!Parcel.Instance.IsPassableAt(new Vector3(randX, spawnY, randZ)))
                spawnY++;

            Position = new Vector3(randX, spawnY, randZ);

        }

        void Wander()
        {
            Vector3 randDir = RNG.Choice(Directions2D.All);

            Direction = new Vector3(randDir.X, 0, randDir.Z);
        }
        
        public void SetPathfindingTarget(Vector3 target)
        {
            isPathfinding = true;

            pathfindingTarget = target;
        }

        public void PathfindTo()
        {
            Vector3 currentPosition = Position;

            Vector3 direction = Vector3.Zero;
            Vector3 secondBestDirection = Vector3.Zero;

            float minHeuristics = float.MaxValue;
            float secondBestHeuristics = minHeuristics;

            foreach (Vector3 dir in Directions2D.All)
            {
                float dst = Distance.Get3D_NotPythagorean(dir + currentPosition, pathfindingTarget);

                if (dst < minHeuristics)
                {
                    secondBestDirection = direction;
                    secondBestHeuristics = minHeuristics;

                    direction = dir;
                    minHeuristics = dst;
                }
            }

            if (minHeuristics < secondBestHeuristics)
            {
                Direction = direction;
            }
            else
            {
                if (RNG.InclInt(0, 1) == 0)
                    Direction = direction;
                else
                    Direction = secondBestDirection;
            }


        }

        void MoveIfPossibleTo(Vector3 position)
        {
            if (TileRegistry.TileAt(position).IsPassable)
                Position = position;
            else
            {
                if (TileRegistry.TileAt(position + Vector3.Up).IsPassable)
                    Position = position + Vector3.Up;

            }
        }

        bool CouldAddGeneralTaskToWorkload()
        {
            var orderedPossibleTasks = TaskManager.Instance.Tasks.OrderBy(item => Distance.Get3D(Position, item.Position));

            foreach (BaseTask task in orderedPossibleTasks.Where(item => !item.IsTaken()))
            {
                workload.AddLast(task);
                task.Taker = this;

                return true;
            }
            return false;
        }

        void CheckForTasks()
        {

            if (framesAlive % 30 == 0)
            {
                if (CouldAddGeneralTaskToWorkload())
                {
                    if (workload.Any() && !IsWorking)
                    {
                        currentTask = workload.First();

                        Console.WriteLine($"Dwarf {Name} took new task ({currentTask.ToString()}). Distance: {Distance.Get3D(Position, currentTask.Position)}.");
                    }
                }

                if (currentTask != null && Distance.Get3D(Position, currentTask.Position) <= MathF.Sqrt(2))
                    currentTask.Do(doer: this);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            moveTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            
            KeyboardState kb = Keyboard.GetState();

            if (kb.GetPressedKeys().Contains(Keys.C))
                workload.Clear();

            // The Dwarf will check for tasks every second or so
            CheckForTasks();

            // Rough gravity simulation
            if (Parcel.Instance.IsPassableAt(Position - new Vector3(0, 1, 0)))
                Position -= new Vector3(0, 1, 0);

            if (moveTimer >= timeToMove)
            {
                if (currentTask != null)
                {
                    SetPathfindingTarget(currentTask.Position);
                    PathfindTo();
                }

                MoveIfPossibleTo(Position + Direction);

                moveTimer = 0;
            }

            framesAlive++;
        }
    }
}
