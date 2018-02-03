using System.Collections.Generic;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Utils;

namespace Assets.Scripts.Pathfinding
{
    public class NodeBuilder
    {
        public static List<Vector3I> BuildAStarNetwork()
        {
            var nodes = CreateNodePositions(Map.Instance.Hexes);
            return nodes;
        }

        private static List<Vector3I> CreateNodePositions(Dictionary<int, Dictionary<int, Hex>> hexes)
        {
            var vec = new List<Vector3I>();
            foreach (var xKey in hexes.Keys)
            {
                foreach (var zKey in hexes[xKey].Keys)
                {
                    if(hexes[xKey][zKey].IsWalkable)
                        vec.Add(new Vector3I(xKey, 0, zKey));
                }
            }
            return vec;
        }
    }
}
