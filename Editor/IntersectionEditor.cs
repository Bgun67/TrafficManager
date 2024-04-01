using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(Intersection))]
class IntersectionEditor : Editor
{
	private Intersection Target;
	[HideInInspector] public Mesh mesh;
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();


	void OnEnable()
	{
		Target = (Intersection)target;
	}


	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if (Target.connectors[0] == null)
		{
			for (int i = 0; i < Target.connectors.Length; i++)
			{
				Transform connector = new GameObject().transform;
				connector.parent = Target.transform;
				connector.name = "Connector" + i;
				Target.connectors[i] = connector;
			}
		}

		Vector3 connector0Position = Target.transform.position + Target.transform.forward * Target.intersectionSize.y;
		Vector3 connector1Position = Target.transform.position - Target.transform.forward * Target.intersectionSize.y; ;
		Vector3 connector2Position = Target.transform.position +
		Mathf.Cos(Target.crossingAngle * Mathf.PI / 180f) * Target.transform.forward * Target.intersectionSize.x +
		Mathf.Sin(Target.crossingAngle * Mathf.PI / 180f) * Target.transform.right * Target.intersectionSize.x;

		Vector3 connector3Position = Target.transform.position -
		Mathf.Cos(Target.crossingAngle * Mathf.PI / 180f) * Target.transform.forward * Target.intersectionSize.x -
		Mathf.Sin(Target.crossingAngle * Mathf.PI / 180f) * Target.transform.right * Target.intersectionSize.x;


		Target.connectors[0].transform.position = connector0Position;
		Target.connectors[1].transform.position = connector1Position;
		Target.connectors[2].transform.position = connector2Position;
		Target.connectors[3].transform.position = connector3Position;

		Target.connectors[0].transform.rotation = Quaternion.LookRotation(connector0Position - Target.transform.position, Target.transform.up);
		Target.connectors[1].transform.rotation = Quaternion.LookRotation(connector1Position - Target.transform.position, Target.transform.up);
		Target.connectors[2].transform.rotation = Quaternion.LookRotation(connector2Position - Target.transform.position, Target.transform.up);
		Target.connectors[3].transform.rotation = Quaternion.LookRotation(connector3Position - Target.transform.position, Target.transform.up);
	}

	void OnSceneGUI()
	{
		if (!Target.meshFilter)
		{
			Target.meshFilter = Target.GetComponent<MeshFilter>();
		}
		if (!Target.collider)
		{
			Target.collider = Target.meshFilter.GetComponent<MeshCollider>();
		}
		if (!mesh)
		{
			mesh = new Mesh();
		}
		else
		{
		}

		if (Target.isDirty)
		{
			mesh.Clear();

			vertices.Clear();
			triangles.Clear();
			uvs.Clear();

			CreateBox(
				new Vector3(Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(Target.roadDimensions.x, 0f, Target.intersectionSize.y),
				new Vector3(-Target.roadDimensions.x, 0f, Target.intersectionSize.y),
				new Vector3(-Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				Target.thickness
			);

			CreateBox(
				new Vector3(-Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, -Target.intersectionSize.y),
				new Vector3(Target.roadDimensions.x, 0f, -Target.intersectionSize.y),
				new Vector3(Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				Target.thickness
			);

			CreateBox(
				new Vector3(Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				new Vector3(Target.intersectionSize.x, 0f, -Target.roadDimensions.y),
				new Vector3(Target.intersectionSize.x, 0f, Target.roadDimensions.y),
				new Vector3(Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				Target.thickness
			);

			CreateBox(
				new Vector3(-Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.intersectionSize.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.intersectionSize.x, 0f, -Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				Target.thickness
			);

			//Center Box
			CreateBox(
				new Vector3(Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				new Vector3(Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				Target.thickness
			);

			int startIndex = vertices.Count;

			vertices = CreateBoxVertices(
				new Vector3(Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				new Vector3(Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, Target.roadDimensions.y),
				new Vector3(-Target.roadDimensions.x, 0f, -Target.roadDimensions.y),
				Target.thickness,
				vertices
			);

			List<int> centerTriangles = new List<int>();
			centerTriangles = CreateBoxTriangles(startIndex, centerTriangles);
			uvs = CreateBoxUVs(uvs);
			mesh.subMeshCount = 2;
			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.SetTriangles(centerTriangles, 1);
			mesh.uv = uvs.ToArray();

			Target.meshFilter.sharedMesh = mesh;
			Target.meshFilter.sharedMesh.UploadMeshData(false);
			Target.collider.sharedMesh = mesh;
		}
	}

	void CreateBox(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float thickness)
	{
		int startIndex = vertices.Count;

		vertices = CreateBoxVertices(p1, p2, p3, p4, thickness, vertices);
		triangles = CreateBoxTriangles(startIndex, triangles);
		uvs = CreateBoxUVs(uvs);

	}

	List<Vector3> CreateBoxVertices(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float thickness, List<Vector3> tempVerts)
	{
		tempVerts.Add(p1);
		tempVerts.Add(p2);
		tempVerts.Add(p3);
		tempVerts.Add(p4);

		tempVerts.Add(p1 + Vector3.up * thickness);
		tempVerts.Add(p2 + Vector3.up * thickness);
		tempVerts.Add(p3 + Vector3.up * thickness);
		tempVerts.Add(p4 + Vector3.up * thickness);
		return tempVerts;
	}

	List<int> CreateBoxTriangles(int startIndex, List<int> tempTriangles)
	{
		//Bottom
		tempTriangles.Add(startIndex + 0);
		tempTriangles.Add(startIndex + 1);
		tempTriangles.Add(startIndex + 2);

		tempTriangles.Add(startIndex + 0);
		tempTriangles.Add(startIndex + 2);
		tempTriangles.Add(startIndex + 3);

		//top
		tempTriangles.Add(startIndex + 4);
		tempTriangles.Add(startIndex + 6);
		tempTriangles.Add(startIndex + 5);

		tempTriangles.Add(startIndex + 4);
		tempTriangles.Add(startIndex + 7);
		tempTriangles.Add(startIndex + 6);

		//right
		tempTriangles.Add(startIndex + 0);
		tempTriangles.Add(startIndex + 4);
		tempTriangles.Add(startIndex + 1);

		tempTriangles.Add(startIndex + 4);
		tempTriangles.Add(startIndex + 5);
		tempTriangles.Add(startIndex + 1);

		//Front
		tempTriangles.Add(startIndex + 1);
		tempTriangles.Add(startIndex + 5);
		tempTriangles.Add(startIndex + 2);

		tempTriangles.Add(startIndex + 5);
		tempTriangles.Add(startIndex + 6);
		tempTriangles.Add(startIndex + 2);

		//left
		tempTriangles.Add(startIndex + 2);
		tempTriangles.Add(startIndex + 7);
		tempTriangles.Add(startIndex + 3);

		tempTriangles.Add(startIndex + 2);
		tempTriangles.Add(startIndex + 6);
		tempTriangles.Add(startIndex + 7);

		return tempTriangles;
	}

	List<Vector2> CreateBoxUVs(List<Vector2> tempUVs)
	{
		tempUVs.Add(new Vector2(0, 1));
		tempUVs.Add(new Vector2(1, 1));
		tempUVs.Add(new Vector2(1, 0));
		tempUVs.Add(new Vector2(0, 0));

		tempUVs.Add(new Vector2(0, 1));
		tempUVs.Add(new Vector2(1, 1));
		tempUVs.Add(new Vector2(1, 0));
		tempUVs.Add(new Vector2(0, 0));
		return tempUVs;
	}


	[DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
		 | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(Intersection))]
	static void DrawGizmos(Intersection intersection, GizmoType selectionType)
	{
		var isActive = Selection.activeGameObject == intersection.gameObject;
		if (isActive)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(intersection.connectors[0].position, intersection.connectors[1].position);
			Gizmos.DrawLine(intersection.connectors[2].position, intersection.connectors[3].position);
		}
	}
}
