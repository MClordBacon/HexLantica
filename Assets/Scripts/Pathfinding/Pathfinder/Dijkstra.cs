using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Pathfinding.Graphs;
using Assets.Scripts.Pathfinding.Utils;

namespace Assets.Scripts.Pathfinding.Pathfinder
{
    public class Dijkstra
    {
        public static void Fill(Node position, int range, SuperNode supernode)
        {
            var openQueue = new Utils.PriorityQueue<VisitedNode>();
            var pathNodeMap = new Dictionary<Node, VisitedNode>();
            pathNodeMap[position] = new VisitedNode(position, null, 0);
            openQueue.Enqueue(pathNodeMap[position], 0);
            while (!openQueue.IsEmpty())
            {
                var current = openQueue.Dequeue();
                current.GridNode.ConnectSuperNode(current.Prev != null ? current.Prev.GridNode : null, supernode, current.GScore);
                foreach (var neighbour in current.GridNode.GetNeighbours())
                {
                    if (!neighbour.To.SuperNodes.ContainsKey(supernode) && neighbour.Length + current.GScore <= range)
                    {
                        var newNode = new VisitedNode(neighbour.To, current, neighbour.Length);
                        if (pathNodeMap.ContainsKey(neighbour.To))
                        {
                            if(openQueue.Update(pathNodeMap[neighbour.To], pathNodeMap[neighbour.To].GScore, newNode, newNode.GScore))
                                pathNodeMap[neighbour.To] = newNode;
                        }
                        else
                        {
                            openQueue.Enqueue(newNode, newNode.GScore);
                            pathNodeMap[neighbour.To] = newNode;
                        }
                    }
                }
            }
        }
        
        public static List<Vector3I> GetArea(Vector3I center, int range, VoxelGraph voxelGraph)
        {
            var openQueue = new Utils.PriorityQueue<VisitedNode>();
            var pathNodeMap = new Dictionary<Node, VisitedNode>();
            var position = voxelGraph.GetClosestNode(center, 1);
            pathNodeMap[position] = new VisitedNode(position, null, 0);
            openQueue.Enqueue(pathNodeMap[position], 0);
            var reachableHexes = new List<Vector3I>();
            while (!openQueue.IsEmpty())
            {
                var current = openQueue.Dequeue();
                reachableHexes.Add(current.GridNode.Position);
                foreach (var neighbour in current.GridNode.GetNeighbours())
                {
                    if (neighbour.Length + current.GScore <= range)
                    {
                        var newNode = new VisitedNode(neighbour.To, current, neighbour.Length);
                        if (pathNodeMap.ContainsKey(neighbour.To))
                        {
                            if(openQueue.Update(pathNodeMap[neighbour.To], pathNodeMap[neighbour.To].GScore, newNode, newNode.GScore))
                                pathNodeMap[neighbour.To] = newNode;
                        }
                        else
                        {
                            openQueue.Enqueue(newNode, newNode.GScore);
                            pathNodeMap[neighbour.To] = newNode;
                        }
                    }
                }
            }
            return reachableHexes;
        }

        public static void FillNeigbours(SuperNode superNode, int gridSize)
        {
            var nodesToFill = superNode.GetNeighbours().Select(n => n.To).ToArray();
            var openQueue = new Utils.PriorityQueue<VisitedNode>();
            var pathNodeMap = new Dictionary<Node, VisitedNode>();
            foreach (var childNode in superNode.ChildNodes)
            {
                if (childNode.SuperNodes[superNode].Length > gridSize * 0.8)
                {
                    pathNodeMap[childNode] = new VisitedNode(childNode, null, childNode.SuperNodes[superNode].Length);
                    openQueue.Enqueue(pathNodeMap[childNode], pathNodeMap[childNode].GScore);
                }
            }
            while (!openQueue.IsEmpty())
            {
                var current = openQueue.Dequeue();
                if(!current.GridNode.SuperNodes.ContainsKey(superNode))
                    current.GridNode.ConnectSuperNode(current.Prev != null ? (current.Prev.GridNode ?? superNode) : superNode, superNode, current.GScore);
                foreach (var neighbour in current.GridNode.GetNeighbours())
                {
                    if (!neighbour.To.SuperNodes.ContainsKey(superNode) && neighbour.To.SuperNodes.Any(k => k.Value.Length <= gridSize && nodesToFill.Contains(k.Key)))
                    {
                        var newNode = new VisitedNode(neighbour.To, current, neighbour.Length);
                        if (pathNodeMap.ContainsKey(neighbour.To))
                        {
                            if (openQueue.Update(pathNodeMap[neighbour.To], pathNodeMap[neighbour.To].GScore, newNode, newNode.GScore))
                                pathNodeMap[neighbour.To] = newNode;
                        }
                        else
                        {
                            openQueue.Enqueue(newNode, newNode.GScore);
                            pathNodeMap[neighbour.To] = newNode;
                        }
                    }
                }
            }
        }
    }
}
