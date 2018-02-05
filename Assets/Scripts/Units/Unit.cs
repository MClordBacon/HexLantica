using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pathfinding.Pathfinder;
using Assets.Scripts.Pathfinding.Utils;
using NUnit.Framework.Internal;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public int Movement;
	private bool _selected;
	private List<VisitedNode> _reachableArea;

	public bool Selected
	{
		get { return _selected; }
		set
		{
			_selected = value;
			if (_selected)
			{
				Select();
			}
			else
			{
				Deselect();
			}
		}
	}

	// Use this for initialization
	void Start()
	{

	}

	public Vector3I	GetPosition()
	{
		return Map.Instance.ToHexPos(transform.position);
	}
	void OnMouseDown()
	{
		
		Selected = !Selected;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void Select()
	{
		if (World.Instance.ActiveUnit != null)
		{
			World.Instance.ActiveUnit.Selected = false;
			World.Instance.ResetActivePath();
		}
		World.Instance.ActiveUnit = this;
		VisualizeMovement();
	}

	public void Deselect()
	{
		foreach (var visitedNode in _reachableArea)
		{
			Map.Instance.GetHex(visitedNode.GridNode.Position.x, visitedNode.GridNode.Position.z).IsSelected = false;
		}
		World.Instance.ResetActivePath();
	}

	private void VisualizeMovement()
	{
		var hexPos = GetPosition();
		_reachableArea = Dijkstra.GetArea(hexPos, Movement, Map.Instance.PathfindingVoxelGraph);

		foreach (var visitedNode in _reachableArea)
		{
			Map.Instance.GetHex(visitedNode.GridNode.Position.x, visitedNode.GridNode.Position.z).IsSelected = true;
		}
	}

	public void ShowPreviewTo(Hex hex)
	{
		var node = _reachableArea.FirstOrDefault(vn => vn.GridNode.Position.x == (int) hex.HexPos.x && vn.GridNode.Position.z == (int)hex.HexPos.z);

		while (node.Prev != null)
		{

			var prevHex = Map.Instance.GetHex(node.GridNode.Position.x, node.GridNode.Position.z);
			prevHex.IsPath = true;
			World.Instance.ActivePath.Add(prevHex);
				
			node = node.Prev;
		}

		var unitHex = Map.Instance.GetHex(GetPosition().x, GetPosition().z);
		unitHex.IsPath = true;
		World.Instance.ActivePath.Add(unitHex);
	}
}