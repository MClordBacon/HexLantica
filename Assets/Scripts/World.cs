using Assets.Scripts.EngineLayer.AI;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class World : MonoBehaviour
    {
        void Start ()
        {
            for (var i = 0; i < 20; i++)
            {
                SpawnUnit();
            }
        }

        private void SpawnUnit()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.transform.localScale = Vector3.one * 0.3f;
            var start = FindWalkable();
            var end = FindWalkable();
            if (start == null || end == null)
            {
                Debug.Log("Couldnt find start or end nodes");
                return;
            }
            obj.transform.position = Map.Instance.GetHex(start.x, start.z).transform.position + Vector3.up * 2;
            var wc = obj.gameObject.AddComponent<WalkingController>();
            wc.MoveSpeed = 1;
            wc.MoveTo(end);
        }
        

        private Vector3I FindWalkable()
        {
            var tries = 0;
            Vector3I vec = null;
            while (vec == null && tries++ < 10)
            {
                var hex = Map.Instance.GetHex(Random.Range(-Map.Instance.MapRadius + 1, Map.Instance.MapRadius), Random.Range(-Map.Instance.MapRadius + 1, Map.Instance.MapRadius));
                if (hex != null && hex.IsWalkable)
                    vec = hex.HexPos;
            }
            return vec;
        }

        void Update () {
        }
    }
}
