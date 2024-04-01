using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArrayCurve))]
public class ArrayCurveEditor : Editor
{
	ArrayCurve Target;

	void OnEnable()
	{
		Target = (ArrayCurve)target;
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		var isActive = Selection.activeGameObject == Target.gameObject;
		if (isActive)
		{
			Target.isDirty = false;
			//Destroy all children
			int numChildren = Target.transform.childCount;
			for (int i = 0; i < numChildren; i++)
			{
				GameObject toDestroy = Target.transform.GetChild(0).gameObject;

				DestroyImmediate(toDestroy);

			}

			float pathLength = Target.path.CalculateLength();

			//Instantiate prefabs
			for (float x = Target.offset.z; x < pathLength; x += Target.spacing)
			{

				Vector3 sampledPosition = Target.path.EvaluatePosition(x/pathLength);
				Vector3 tangent = Target.path.EvaluateTangent((x)/pathLength);
				Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
				Vector3 normal = Vector3.Cross(tangent, right).normalized;

				Quaternion rotation = Quaternion.LookRotation(Target.alignToNormal ? tangent : Vector3.ProjectOnPlane(tangent, Vector3.up), Target.alignToNormal ? normal : Vector3.up);
				GameObject go = Instantiate(Target.prefab, Target.transform);
				go.transform.position = sampledPosition + right * Target.offset.x + normal * Target.offset.y;
				go.transform.rotation = rotation;

				if (Target.mirror)
				{
					GameObject go2 = Instantiate(Target.prefab, Target.transform);
					go2.transform.position = sampledPosition - right * Target.offset.x + normal * Target.offset.y;
					go2.transform.rotation = rotation;
					go2.transform.localScale = Vector3.Scale(go2.transform.localScale, new Vector3(-1, 1, 1));
				}
			}

		}

	}

}
