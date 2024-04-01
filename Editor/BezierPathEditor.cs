using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
	BezierPath Target;
	List<Vector3> minTangents = new List<Vector3>();
	List<Vector3> maxTangents = new List<Vector3>();
	void Awake()
	{
		Target = (BezierPath)target;
	}

	void OnSceneGUI()
	{
		if (Target.tangents.Count != Target.waypoints.Count)
		{
			Target.tangents = new List<Vector3>(Target.waypoints);
			minTangents = new List<Vector3>(Target.waypoints);
			maxTangents = new List<Vector3>(Target.waypoints);
		}
		Debug.Log("Drawing");
		//Handles.DrawBezier(
		//Handles.MakeBezierPoints
		for (int i = 0; i < Target.waypoints.Count; i++)
		{
			Target.waypoints[i] = DrawPositionControl(Target.waypoints[i], Target.transform.localToWorldMatrix, Target.transform.rotation);
			Vector3 tangentP2 = DrawPositionControl(Target.waypoints[i]+Target.tangents[i]/2f, Target.transform.localToWorldMatrix, Target.transform.rotation);
			Vector3 tangentP1 = DrawPositionControl(Target.waypoints[i]-Target.tangents[i]/2f, Target.transform.localToWorldMatrix, Target.transform.rotation);

			Target.tangents[i] = tangentP2 - tangentP1;

			if (i < Target.waypoints.Count - 1)
			{
				Vector3 tangent1 = Target.tangents[i];
				Vector3 tangent2 = Target.tangents[i + 1];
				Vector3[] bezierPoints = Handles.MakeBezierPoints(Target.transform.TransformPoint(Target.waypoints[i]), Target.transform.TransformPoint(Target.waypoints[i + 1]), Target.transform.TransformVector(tangent1), Target.transform.TransformVector(tangent2), 10);
				Handles.DrawAAPolyLine(2f, bezierPoints);
			}
		}

	}

	Vector3 DrawPositionControl(Vector3 wp, Matrix4x4 localToWorld, Quaternion localRotation)
	{
		Vector3 pos = localToWorld.MultiplyPoint(wp);

		Handles.color = Color.green;
		Quaternion rotation = (Tools.pivotRotation == PivotRotation.Local)
				? localRotation : Quaternion.identity;
		float size = HandleUtility.GetHandleSize(pos) * 0.1f;
		Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);

		pos = Handles.PositionHandle(pos, rotation);

		Undo.RecordObject(target, "Move Waypoint");
		wp = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);

		return wp;

	}

	Vector3 DrawTangentControl(Vector3 wp, Vector3 tangent, Matrix4x4 localToWorld, Quaternion localRotation)
	{
		Vector3 pos = localToWorld.MultiplyPoint(wp+tangent);

		Handles.color = Color.green;
		Quaternion rotation = (Tools.pivotRotation == PivotRotation.Local)
				? localRotation : Quaternion.identity;
		float size = HandleUtility.GetHandleSize(pos) * 0.1f;
		Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);

		pos = Handles.PositionHandle(pos, rotation);

		Undo.RecordObject(target, "Move Waypoint");
		wp = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);

		return wp;

	}


	
}
