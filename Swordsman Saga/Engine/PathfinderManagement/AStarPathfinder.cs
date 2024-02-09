using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataTypes;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Swordsman_Saga.Engine.PathfinderManagement
{ internal class AStarPathfinder
    {
        private readonly DiamondGrid mGrid;
        public AStarPathfinder(DiamondGrid grid)
        {
            mGrid = grid;
        }

        public List<Vector2> FindPath(Vector2 start, Vector2 goal)
        {
            List<Vector2> path;
            // Convert start and goal from world coordinates to mGrid  Vector2 coordinates
            var startSlot = mGrid.TranslateToGridV(start);
            var goalSlot = mGrid.TranslateToGridV(goal);

            // Adjust screen position to account for isometric distortion
            var realStartPos = new Vector2(start.X, start.Y * 2f);
            var realGoalPos = new Vector2(goal.X, goal.Y * 2f);

            // Initialize the open and closed lists
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();

            // Create the start node and the goal node
            var startDistance = Vector2.Distance(realStartPos, realGoalPos);
            var startNode = new Node(startSlot, null, 0, startDistance, mGrid);
            var goalNode = new Node(goalSlot, null, 0, 0, mGrid);

            // Initialize the nearest node
            Node nearestNode = startNode;
            var nearestDistance = startDistance;

            // Add the start node to the open list
            openList.Add(startNode);

            // Loop until the open list is empty or maxIteration times
            const int maxIterations = 10000;
            var n = 0;
            while (openList.Count > 0 &&  n < maxIterations)
            {
                // Find the node with the lowest F score
                var currentNode = openList[0];
                foreach (var node in openList)
                {
                    if (node.F < currentNode.F)
                    {
                        currentNode = node;
                    }
                }

                // Check if we have reached the goal
                if (currentNode.Slot == goalNode.Slot)
                {
                    path = RetracePath(currentNode.Parent);
                    path.Add(goal);
                    return path;
                }

                // Move the current node to the closed list
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Iterate through the current node's neighbors
                foreach (var neighborSlot in mGrid.GetNeighbors(currentNode.Slot))
                {
                    // Create a neighbor node
                    var neighborPos = mGrid.TranslateFromGridV(neighborSlot);
                    var realNeighborPos = new Vector2(neighborPos.X, neighborPos.Y * 2f);
                    var realCurrentPos = new Vector2(currentNode.Position.X, currentNode.Position.Y * 2f);
                    var movingDistance = Vector2.Distance(realCurrentPos, realNeighborPos);
                    var distanceToGoal = Vector2.Distance(realNeighborPos, realGoalPos);
                    var neighborNode = new Node(neighborSlot, currentNode, currentNode.G + movingDistance, distanceToGoal, mGrid);

                    // Check if the neighbor is in the closed list or is not walkable
                    if (closedList.Contains(neighborNode) || !mGrid.IsPathable(neighborSlot))
                    {
                        continue;
                    }

                    // Check if the neighbor is in the open list and if it has a lower G score
                    if (openList.Contains(neighborNode) && neighborNode.G < currentNode.G)
                    {
                        openList.Remove(neighborNode);
                    }

                    // Add the neighbor to the open list if it's not there
                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }

                    // Check if the neighbor is nearer to the goal
                    if (distanceToGoal < nearestDistance)
                    {
                        nearestNode = neighborNode;
                        nearestDistance = distanceToGoal;
                    }
                }
                n++;
            }
            // If no path to the goal is found, check if a nearest reachable node is available
            path = nearestNode.Parent is null ? new List<Vector2>() : RetracePath(nearestNode);
            var realGoalSlotPos = new Vector2(goalNode.Position.X, goalNode.Position.Y * 2f);
            path.Add(GetNearestOuterNavPoint(nearestNode, realGoalSlotPos));
            return path;
        }

        public static void DrawPath(SpriteBatch spriteBatch, List<Vector2> path)
        {
            for (var i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                {
                    //spriteBatch.DrawRectangle(new RectangleF(path[i].X - 4, path[i].Y - 4, 8, 8), Color.Red);
                    spriteBatch.DrawLine(new Vector2(path[i].X - 8, path[i].Y + 8), new Vector2(path[i].X + 8, path[i].Y - 8), Color.Red, 4f);
                    spriteBatch.DrawLine(new Vector2(path[i].X - 8, path[i].Y - 8), new Vector2(path[i].X + 8, path[i].Y + 8), Color.Red, 4f);
                    continue;
                }
                spriteBatch.DrawLine(path[i], path[i + 1], Color.LightCoral, 2f);
            }
        }

        private List<Vector2> RetracePath(Node endNode)
        {
            List<Vector2> path = new();
            if (endNode == null) return path;
            Node currentNode = endNode;

            while (currentNode.Parent is not null)
            {
                path.Add(mGrid.TranslateFromGridV(currentNode.Slot));
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        private Vector2 GetNearestOuterNavPoint(Node node, Vector2 realGoalPos)
        {
            Vector2 nearestOuterNavPoint = mGrid.TranslateToGridV(node.Slot);
            Vector2 realNearestOuterNavPointPos = new Vector2(nearestOuterNavPoint.X, nearestOuterNavPoint.Y * 2f);
            float nearestDistance = Vector2.Distance(realNearestOuterNavPointPos, realGoalPos);

            // Search nearest outer nav point and add it to the path
            foreach (Vector2 outerNavPoint in mGrid.GetOuterNavPoints(node.Slot))
            {
                Vector2 realOuterNavPointPos = new Vector2(outerNavPoint.X, outerNavPoint.Y * 2f);
                float distanceToGoal = Vector2.Distance(realOuterNavPointPos, realGoalPos);
                if (distanceToGoal < nearestDistance)
                {
                    nearestOuterNavPoint = outerNavPoint;
                    nearestDistance = distanceToGoal;
                }
            }
            return nearestOuterNavPoint;
        }

        private class Node : IEquatable<Node>
        {
            public Vector2 Slot { get; }
            public Vector2 Position { get; }
            public Node Parent { get; }
            public float G { get; }
            private float H { get; }
            public float F => G + H;

            public Node(Vector2 slot, Node parent, float g, float h, DiamondGrid grid)
            {
                Slot = slot;
                Parent = parent;
                G = g;
                H = h;
                Position = grid.TranslateFromGridV(slot);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Node);
            }

            public bool Equals(Node other)
            {
                return other != null && Slot.Equals(other.Slot);
            }

            public override int GetHashCode()
            {
                return Slot.GetHashCode();
            }
        }
    }
    
}
