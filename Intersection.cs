using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
	[HideInInspector] public Transform[] connectors = new Transform[4];
	public Road[] connectedRoads = new Road[4];

	[SerializeField] public Vector2 intersectionSize;
	[SerializeField] public float thickness = 0.5f;
	[SerializeField] public float crossingAngle = 90f;
	[SerializeField] public Vector2 roadDimensions;
	[Range(1f,40f)]
	[SerializeField] public float uvLength;
	[HideInInspector] public bool isDirty;
	[HideInInspector] public MeshFilter meshFilter;
	[HideInInspector] public MeshCollider collider;

	void OnValidate()
	{
		isDirty = true;
	}
}
