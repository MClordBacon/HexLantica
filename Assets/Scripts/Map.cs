using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    public static Map Instance;
    public GameObject BaseHex;
    public bool StartGenerating;
    public int MapRadius;
    public int MapSlices;
    public float HexRadius;
    public Dictionary<int, Dictionary<int, Hex>> Hexes = new Dictionary<int, Dictionary<int, Hex>>();

    private const float hexSize = 0.86602540378f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (StartGenerating)
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
                    float y = Random.Range(0, 3);
                    y = y * y / 5f;
                    PlaceHex(new Vector3(x + (MapSlices-remainingSlices), y, z));
                }
                if (minZ - 1 > -MapRadius)
                {
                    minZ--;
                }
                else if(remainingSlices == 0)
                {
                    maxZ--;
                }
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
        obj.GetComponentInChildren<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
        var hex = obj.AddComponent<Hex>();
        hex.HexPos = pos;
        obj.gameObject.name = "Hex(" + (int) pos.x + "/" + (int) pos.z + ")";
        SetHex((int)pos.x, (int)pos.z, hex);
        return hex;
    }

    void Update()
    {
    }
}

public class Hex : MonoBehaviour
{
    public Vector3 HexPos;

    public Hex GetNeigbour(HexDir dir)
    {
        switch (dir)
        {
            case HexDir.NE:
                return Map.Instance.GetHex((int) HexPos.x, (int) HexPos.z + 1);
            case HexDir.E:
                return Map.Instance.GetHex((int)HexPos.x + 1, (int)HexPos.z);
            case HexDir.SE:
                return Map.Instance.GetHex((int)HexPos.x + 1, (int)HexPos.z - 1);
            case HexDir.SW:
                return Map.Instance.GetHex((int)HexPos.x, (int)HexPos.z - 1);
            case HexDir.W:
                return Map.Instance.GetHex((int)HexPos.x - 1, (int)HexPos.z);
            case HexDir.NW:
                return Map.Instance.GetHex((int)HexPos.x - 1, (int)HexPos.z + 1);
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }
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