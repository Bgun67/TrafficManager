using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Road : MonoBehaviour
{
	[SerializeField] public Vector2 roadDimensions;
	public MeshFilter meshFilter;
	public MeshCollider collider;
	[SerializeField] public SplineContainer path;
	[Range(0.05f, 50f)]
	[SerializeField] public float segmentLength = 10;
	[Range(1f,40f)]
	[SerializeField] public float uvLength;

	[HideInInspector] public bool isDirty;
	[SerializeField] public Transform startConnector;
	[SerializeField] public Transform endConnector;
	[SerializeField] public Vector3 offset;

	// Start is called before the first frame update

	void OnValidate()
	{
		isDirty = true;

	}

	

}
