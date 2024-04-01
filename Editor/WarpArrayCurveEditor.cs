using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WarpArrayCurve))]
public class WarpArrayCurveEditor : Editor
{
	WarpArrayCurve Target;
	Mesh mesh;
	MeshCollider collider;

	void OnEnable()
	{
		Target = (WarpArrayCurve)target;
		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		collider = Target.GetComponent<MeshCollider>();
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if(GUILayout.Button("Build"))
        {
			Target.isDirty = false;
			//Destroy all children
			for (int i = 0; i < Target.transform.childCount; i++)
			{
				GameObject toDestroy = Target.transform.GetChild(i).gameObject;

				DestroyImmediate(toDestroy);

			}

			float pathLength = Target.path.CalculateLength();
			//    meshes[1]._mesh = (Mesh)Instantiate (meshes[0]._mesh);

			//Instantiate prefabs
			List<CombineInstance> combineInstances = new List<CombineInstance>();
			for (float x = 0.01f; x < pathLength; x += Target.spacing)
			{
				foreach (MeshFilter filter in Target.prefabs[Random.Range(0, Target.prefabs.Length)].GetComponentsInChildren<MeshFilter>())
				{
					Mesh tempMesh = Instantiate(filter.sharedMesh);
					Vector3[] vertices = tempMesh.vertices;
					for (int i = 0; i < vertices.Length; i++)
					{
						Vector3 desiredPosition = Target.path.EvaluatePosition((x + vertices[i].z) / pathLength);
						Vector3 tangent = Target.path.EvaluateTangent((x + vertices[i].z) / pathLength);
						Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
						Vector3 normal = Target.alignToNormal?Vector3.Cross(tangent, right).normalized: Vector3.up;
						desiredPosition += right * (vertices[i].x + Target.offset.x) + normal * +(vertices[i].y + Target.offset.y);

						vertices[i] = Target.transform.InverseTransformPoint(desiredPosition);//+ rotation * vertices[i];// +right*Target.offset.x+normal*Target.offset.y;
					}
					tempMesh.vertices = vertices;
					CombineInstance combineInstance = new CombineInstance();
					combineInstance.mesh = tempMesh;
					combineInstance.transform = Matrix4x4.identity;//Target.transform.localToWorldMatrix;
					combineInstances.Add(combineInstance);
				}
			}
			mesh.CombineMeshes(combineInstances.ToArray());

			Target.meshFilter.sharedMesh = mesh;
			Target.meshFilter.sharedMesh.UploadMeshData(false);
			collider.sharedMesh = mesh;

		}

	}

}
