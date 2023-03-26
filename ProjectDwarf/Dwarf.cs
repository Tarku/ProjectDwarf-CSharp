using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectDwarf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDwarf
{
    public class Dwarf
    {
        static int spawnScattering = 5;

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        Vector3 pathfindingTarget;


        public string Name { get; set; }
        public int Age { get; set; }

        float moveTimer = 0f;
        float timeToMove = 500f;

        byte spawnY;
        bool isPathfinding = false;

        public Dwarf(string name, int age)
        {
            Name = name;
            Age = age;

            spawnY = Constants.SurfaceLevel;

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

        void MoveIfPossibleTo(Vector3 target)
        {
            if (Parcel.Instance.IsPassableAt(target))
            {
                Position = target;
            }
            else
            {
                if (Parcel.Instance.IsPassableAt(target + Vector3.Up))
                {
                    Position = target + Vector3.Up;
                }
            }
        }

        Vector3 PositionIfMovePossible(Vector3 target)
        {
            if (Parcel.Instance.IsPassableAt(target))
            {
                return target;
            }
            else
            {
                if (Parcel.Instance.IsPassableAt(target + Vector3.Up))
                {
                    return target + Vector3.Up;

                } else
                {
                    return Position;
                }
            }
        }
        
        public void SetPathfindingTarget(Vector3 target)
        {
            isPathfinding = true;

            pathfindingTarget = target;
        }

        public void PathfindTo()
        {
            PathfindingNode startNode = new PathfindingNode(Position);
            PathfindingNode endNode = new PathfindingNode(pathfindingTarget);

            PathfindingNode currentNode;



            List<PathfindingNode> openList = new() { startNode };
            List<PathfindingNode> closedList = new();

            

            while (openList.Count > 0)
            {
                currentNode = openList[0];

                foreach (PathfindingNode node in openList)
                {
                    if (node.f < currentNode.f)
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == endNode)
                {

                }
            }

        }

        public void Update(GameTime gameTime)
        {
            moveTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Parcel.Instance.IsPassableAt(Position - new Vector3(0, 1, 0)))
                Position -= new Vector3(0, 1, 0);

            if (moveTimer >= timeToMove)
            {
                if (isPathfinding)
                    PathfindTo();
                if (!isPathfinding)
                    Wander();

                MoveIfPossibleTo(Position + Direction);

                moveTimer = 0;
            }
        }
    }
}
