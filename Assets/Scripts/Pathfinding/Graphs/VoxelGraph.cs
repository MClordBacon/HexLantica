using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Pathfinder;
using Assets.Scripts.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts.Pathfinding.Graphs
{
    public class VoxelGraph
    {
        private readonly Grid3D<Node> _grid = new Grid3D<Node>();
        private readonly Grid3D<SuperNode> _gridT1 = new Grid3D<SuperNode>();
        private readonly HashSet<Node> _dirtyNodes = new HashSet<Node>();
        private readonly PathRegistry _pathRegistry = new PathRegistry();
        private int _gridSize;

        public void AddNode(int xPos, int yPos, int zPos)
        {
            AddNode(new Vector3I(xPos, yPos, zPos));
        }

        public void AddNode(Vector3I pos)
        {
            var node = new T0Node(pos)
            {
                WorldPosition = Map.Instance.GetHex(pos.x, pos.z).transform.position + Vector3.up*2
            };
            _grid[pos.x, pos.y, pos.z] = node;
            ConnectNeighbours(pos.x, pos.y, pos.z);
            if (_gridSize > 0)
            {
                node.ConnectToSupernodes();
            }
        }

        public void Visualize()
        {
            foreach (var node in _grid)
            {
                foreach (var edge in node.GetNeighbours())
                {
                    Debug.DrawLine(node.Position, edge.To.Position, Color.blue, 1000);
                }
            }
        }

        public void MarkDirty(Node node)
        {
            _dirtyNodes.Add(node);
        }

        public Vector3I GetSize()
        {
            return _grid.GetSize();
        }

        public Node GetNode(Vector3I from)
        {
            return _grid[from.x, from.y, from.z];
        }
        
        public Node GetClosestNode(Vector3I pos, int range)
        {
            return _grid.GetNearestItem(pos, range);
        }

        public PathRegistry GetPathRegistry()
        {
            return _pathRegistry;
        }

        public IEnumerable<Node> GetT1Nodes()
        {
            return _gridT1.Cast<Node>().ToList();
        }

        public void ConnectNeighbours(int xPos, int yPos, int zPos)
        {
            var node = _grid[xPos, yPos, zPos];
            if (node == null)
                return;
            var hex = Map.Instance.Hexes[xPos][zPos];
            foreach (HexDir dir in Enum.GetValues(typeof(HexDir)))
            {
                var neighbour = hex.GetNeigbour(dir);
                if (neighbour == null)
                    continue;
                var t0Node = _grid[(int) neighbour.HexPos.x, (int) neighbour.HexPos.y, (int) neighbour.HexPos.z] as T0Node;
                if(t0Node != null)
                    t0Node.ConnectTo(node, 1);
            }
            /*for (var dX = -1; dX <= 1; dX++)
            {
                for (var dY = -1; dY <= 1; dY++)
                {
                    for (var dZ = -1; dZ <= 1; dZ++)
                    {
                        if (dX == 0 && dY == 0 && dZ == 0)
                            continue;
                        if (_grid[xPos + dX, yPos + dY, zPos + dZ] != null)
                        {
                            var t0Node = _grid[xPos + dX, yPos + dY, zPos + dZ] as T0Node;
                            if (t0Node != null)
                                t0Node.ConnectTo(node, Mathf.Sqrt(dX * dX + dY * dY + dZ * dZ));
                        }
                    }
                }
            }*/
        }
        public void RemoveNode(Vector3I pos)
        {
            var node = _grid.Get(pos.x, pos.y, pos.z);
            if (node == null)
                return;
            node.Delete(this);
            _grid.Remove(pos);
            ProcessDirtyNodes();
            _pathRegistry.RemovedNode(node, this);
        }

        private void ProcessDirtyNodes()
        {
            foreach (var dirtyNode in _dirtyNodes.ToArray())
            {
                var node = GetNode(dirtyNode.Position);
                if (node == null)
                    continue;
                if (!dirtyNode.HasDirectSupernode)
                {
                    var supernode = new SuperNode(dirtyNode.Position, _gridSize);
                    _gridT1[dirtyNode.Position.x, dirtyNode.Position.y, dirtyNode.Position.z] = supernode;

                    Dijkstra.Fill(node, _gridSize, supernode);
                    EnsureNeighbourCoverage(_gridSize, new List<SuperNode>(){supernode});
                }
            }
            _dirtyNodes.Clear();
        }

        public void AddTier1Nodes(int gridSize)
        {
            _gridSize = gridSize;
            var size = GetSize();
            AddSupernodeGrid(gridSize, size);
            FillSupernodeHoles(gridSize, size, Vector3I.zero);
            EnsureNeighbourCoverage(gridSize);
        }
        
        private void AddSupernodeGrid(int gridSize, Vector3I size)
        { 
            for (var dX = Math.Min(gridSize / 2, size[0]); dX <= size[0]; dX += gridSize)
            {
                for (var dY = Math.Min(gridSize / 2, size[1]); dY <= size[1]; dY += gridSize)
                {
                    for (var dZ = Math.Min(gridSize / 2, size[2]); dZ <= size[2]; dZ += gridSize)
                    {
                        var node = _grid.GetNearestItem(new Vector3I(dX, dY, dZ), 5);
                        if (node == null)
                            continue;
                        var supernode = new SuperNode(node.Position, gridSize);
                        _gridT1[dX, dY, dZ] = supernode;

                        Dijkstra.Fill(GetNode(supernode.Position), gridSize, supernode);
                    }
                }
            }
        }
        
        private void FillSupernodeHoles(int gridSize, Vector3I size, Vector3I startPosition)
        {
            for (var dX = startPosition.x; dX <= size[0]; dX++)
            {
                for (var dY = startPosition.y; dY <= size[1]; dY++)
                {
                    for (var dZ = startPosition.z; dZ <= size[2]; dZ++)
                    {
                        var node = _grid[dX, dY, dZ];
                        if (node == null || node.SuperNodes.Count > 0)
                            continue;
                        var supernode = new SuperNode(node.Position, gridSize);
                        _gridT1[dX, dY, dZ] = supernode;

                        Dijkstra.Fill(GetNode(supernode.Position), gridSize, supernode);
                    }
                }
            }
            return;
        }


        private void EnsureNeighbourCoverage(int gridSize, List<SuperNode> nodes = null)
        {
            nodes = nodes ?? _gridT1.ToList();
            foreach (var superNode in nodes)
            {
                Dijkstra.FillNeigbours(superNode, gridSize);
            }
        }
    }
}
