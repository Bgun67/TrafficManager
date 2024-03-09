using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class WarpArrayCurve : MonoBehaviour
{
    [SerializeField] public GameObject[] prefabs;
	[SerializeField] public SplineContainer path;
	[Range(0.05f, 100f)]
	[SerializeField] public float spacing = 5f;
	[SerializeField] public Vector3 offset;
	[SerializeField] public bool alignToNormal = true;
	[SerializeField] public bool mirror = false;
	public MeshFilter meshFilter;
	public bool isDirty;

	// Start is called before the first frame update
	void OnValidate()
    {
		isDirty = true;
	}
}
