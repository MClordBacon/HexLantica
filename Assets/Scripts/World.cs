using System.Collections.Generic;
using Assets.Scripts.EngineLayer.AI;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class World : MonoBehaviour
    {
        public List<Unit> AllUnits;
        public Unit ActiveUnit;
        public List<Hex> ActivePath;
        public static World Instance;

        void Awake()
        {
            Instance = this;
        }

        void Start ()
        {
            for (var i = 0; i < 1; i++)
            {
                SpawnUnit();
            }
        }

        private void SpawnUnit()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.transform.localScale = Vector3.one * 0.3f;
            var start = FindWalkable();
            obj.transform.position = Map.Instance.GetHex(start.x, start.z).transform.position + Vector3.up * 2;
            var wc = obj.gameObject.AddComponent<WalkingController>();
            var unit = obj.gameObject.AddComponent<Unit>();
            unit.Movement = 2;
            AllUnits.Add(unit);
            wc.MoveSpeed = 1;
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

        public void ResetActivePath()
        {
            foreach (var hex in ActivePath)
            {
                hex.IsPath = false;
            }
        }
    }
}
