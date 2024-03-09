using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
	public List<Vector3> waypoints = new List<Vector3>();
	public float PathLength = 10f;
	public List<Vector3> tangents = new List<Vector3>();

	public Vector3 EvaluatePosition(float pos)
	{

		return waypoints[0];
	}

	public Vector3 EvaluateTangent(float pos)
	{
		return waypoints[0];
	}
}
