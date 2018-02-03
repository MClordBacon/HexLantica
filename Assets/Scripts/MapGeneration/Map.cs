using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.Pathfinding.Graphs;
using Assets.Scripts.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
    public class Map : MonoBehaviour
    {
        public static Map Instance;
        public GameObject BaseHex;
        public bool StartGenerating;
        public int MapRadius;
        public int MapSlices;
        public float HexRadius;
        public Dictionary<int, Dictionary<int, Hex>> Hexes = new Dictionary<int, Dictionary<int, Hex>>();
        public VoxelGraph PathfindingVoxelGraph;

        private const float hexSize = 0.86602540378f;

        void Awake()
        {
            Instance = this;
            if (StartGenerating)
            {
                GenerateMap();
                PathfindingVoxelGraph = GeneratePathfindingGraph();
            }
        }

        private VoxelGraph GeneratePathfindingGraph()
        {
            var vg = new VoxelGraph();
            var vectors = NodeBuilder.BuildAStarNetwork();
            foreach (var vector3I in vectors)
            {
                vg.AddNode(vector3I);
            }
            return vg;
        }

        private void GenerateMap()
        {
            var remainingSlices = MapSlices;
            var minZ = 0;
            var maxZ = MapRadius;
            for (int x = -MapRadius + 1; x < MapRadius; x++)
            {
                if (x > 0 && remainingSlices > 0)
                {
                    x--;
                    remainingSlices--;
                }
                for (var z = minZ; z < maxZ; z++)
                {
                    float y = Random.Range(0, 10);
                    if (y < 5)
                        y = 0;
                    else if (y < 8)
                        y = 0.3f;
                    else
                        y = 1f;
                    PlaceHex(new Vector3(x + (MapSlices - remainingSlices), y, z));
                }
                if (minZ - 1 > -MapRadius)
                {
                    minZ--;
                }
                else if (remainingSlices == 0)
                {
                    maxZ--;
                }
            }
        }

        public Hex GetHex(int x, int z)
        {
            if (!Hexes.ContainsKey(x))
                return null;
            if (!Hexes[x].ContainsKey(z))
                return null;
            return Hexes[x][z];
        }

        public Hex SetHex(int x, int z, Hex hex)
        {
            if (!Hexes.ContainsKey(x))
                Hexes[x] = new Dictionary<int, Hex>();
            Hexes[x][z] = hex;
            return hex;
        }

        public Hex PlaceHex(Vector3 pos)
        {
            var obj = Instantiate(BaseHex);
            obj.transform.position = new Vector3(pos.x * hexSize * 2 + pos.z * hexSize, pos.y, pos.z * 1.5f * HexRadius);
            obj.transform.localScale = Vector3.one/HexRadius;
            obj.GetComponentInChildren<MeshRenderer>().material.color = new Color(pos.y, pos.y, pos.y);
            var hex = obj.AddComponent<Hex>();
            hex.HexPos = pos;
            obj.gameObject.name = "Hex(" + (int) pos.x + "/" + (int) pos.z + ")";
            SetHex((int)pos.x, (int)pos.z, hex);
            return hex;
        }

        void Update()
        {
        }

        public Vector3I ToHexPos(Vector3 transformPosition)
        {
            float closestDist = 0;
            Hex closestHex = null;
            foreach (var xk in Hexes.Keys)
            {
                foreach (var zk in Hexes[xk].Keys)
                {
                    var dist = Vector3.Distance(Hexes[xk][zk].transform.position, transformPosition);
                    if (closestHex == null || dist < closestDist)
                    {
                        closestHex = Hexes[xk][zk];
                        closestDist = dist;
                    }
                }
            }
            return closestHex.HexPos;
        }
    }

    public enum HexDir
    {
        NE,
        E,
        SE,
        SW,
        W,
        NW
    }
}