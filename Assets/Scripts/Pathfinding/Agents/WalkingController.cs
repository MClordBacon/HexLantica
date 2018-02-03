﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Graphs;
using Assets.Scripts.Pathfinding.Pathfinder;
using Assets.Scripts.Pathfinding.Utils;
using UnityEngine;

namespace Assets.Scripts.EngineLayer.AI
{
    public class WalkingController : MonoBehaviour
    {
        public Path PathToTarget;
        public float MoveSpeed;
        private int _pathIndex;
        private Node _currentNode;
        
        public bool IsIdle
        {
            get { return PathToTarget == null; }
        }

        void Start()
        {
        }

        void Update()
        {
            if (PathToTarget == null || !PathToTarget.Finished || PathToTarget.State != PathState.Ready)
            {
                if (PathToTarget != null && PathToTarget.State == PathState.Invalid)
                {
                    PathToTarget.Dispose();
                    PathToTarget = null;
                }
                return;
            }
            if (_currentNode == null)
            {
                _currentNode = PathToTarget.GetNode(0);
                if (_currentNode == null)
                {
                    PathToTarget = null;
                    return;
                }
                _pathIndex = 0;
            }
            PathToTarget.Visualize(Color.red, _pathIndex);
            var moveDist = MoveSpeed * Time.deltaTime;
            while (moveDist > 0)
            {
                if ((transform.position - _currentNode.WorldPosition).magnitude > moveDist)
                {
                    transform.Translate((_currentNode.WorldPosition - transform.position).normalized * moveDist, Space.Self);
                    
                    return;
                }
                else
                {
                    moveDist -= (transform.position - _currentNode.WorldPosition).magnitude;
                    transform.position = _currentNode.WorldPosition;
                    _currentNode = PathToTarget.GetNode(++_pathIndex);
                    if (_currentNode == null)
                    {
                        PathToTarget = null;
                        return;
                    }
                }
            }
        }

        public void MoveTo(Vector3 target)
        {
            PathToTarget = Path.Calculate(Map.Instance.PathfindingVoxelGraph, Map.Instance.ToHexPos(transform.position), target, true);
        }

        public void MoveToAny(List<Vector3> targets)
        {
            PathToTarget = Path.Calculate(Map.Instance.PathfindingVoxelGraph, Map.Instance.ToHexPos(transform.position), targets.Select(t => new Vector3I(t)).ToList(), true);
        }
    }
}
