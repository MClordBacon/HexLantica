using System.Collections;
using System.Collections.Generic;
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
	private List<Vector3I> _reachableArea;

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
		foreach (var unit in World.Instance.AllUnits)
		{
			if (unit.Selected && unit != this)
			{
				unit.Selected = false;
			}
		}
		VisualizeMovement();
	}

	public void Deselect()
	{
		foreach (var hexVector3I in _reachableArea)
		{
			Map.Instance.GetHex(hexVector3I.x, hexVector3I.z).IsSelected = false;
		}
	}

	private void VisualizeMovement()
	{
		var hexPos = Map.Instance.ToHexPos(transform.position);
		_reachableArea = Dijkstra.GetArea(hexPos, Movement, Map.Instance.PathfindingVoxelGraph);

		foreach (var hexVector3I in _reachableArea)
		{
			Map.Instance.GetHex(hexVector3I.x, hexVector3I.z).IsSelected = true;
		}
	}
}