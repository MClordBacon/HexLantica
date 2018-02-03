using System;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
    public class Hex : MonoBehaviour
    {
        public Vector3 HexPos;
        public bool IsWalkable { get { return HexPos.y < 1; } }

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
}