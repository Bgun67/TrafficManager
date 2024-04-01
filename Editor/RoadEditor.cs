using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
	Road Target;
	Mesh mesh;
	List<Vector3> vertices = new List<Vector3>();
	List<Vector3> normals = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<int> triangles = new List<int>();
	Intersection[] intersections;


	void OnEnable()
	{
		Target = (Road)target;
	}
	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();
		if (Target.path == null)
		{
			return;
		}
		var isActive = Selection.activeGameObject == Target.gameObject;
		if (isActive)
		{
			Target.isDirty = false;
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
				mesh.Clear();
			}

			vertices.Clear();
			normals.Clear();
			uvs.Clear();
			triangles.Clear();

			int vertexNum = 0;

			float pathLength = Target.path.CalculateLength();


			for (float pathDist = 0f; pathDist < pathLength + Target.segmentLength; pathDist += Target.segmentLength)
			{
				float t = Mathf.Clamp01(pathDist / pathLength);
				Vector3 sampledPosition = Target.path.EvaluatePosition(t);
				Vector3 tangent = Target.path.EvaluateTangent(t);
				Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
				Vector3 normal = Vector3.Cross(tangent, right).normalized;



				Vector3 worldPosition0 = sampledPosition + right * Target.roadDimensions.x + right * Target.offset.x + normal * 0;
				Vector3 worldPosition1 = sampledPosition + right * Target.roadDimensions.x + right * Target.offset.x + normal * Target.roadDimensions.y;
				Vector3 worldPosition2 = sampledPosition - right * Target.roadDimensions.x + right * Target.offset.x + normal * Target.roadDimensions.y;
				Vector3 worldPosition3 = sampledPosition - right * Target.roadDimensions.x + right * Target.offset.x + normal * 0;

				vertices.Add(Target.transform.InverseTransformPoint(worldPosition0));
				normals.Add(right);

				vertices.Add(Target.transform.InverseTransformPoint(worldPosition1));
				normals.Add(normal);

				vertices.Add(Target.transform.InverseTransformPoint(worldPosition2));
				normals.Add(normal);

				vertices.Add(Target.transform.InverseTransformPoint(worldPosition3));
				normals.Add(-right);

				float uvYCoord = (t * pathLength) / Target.uvLength;

				uvs.Add(new Vector2(uvYCoord, 0));
				uvs.Add(new Vector2(uvYCoord, 1));
				uvs.Add(new Vector2(uvYCoord, 0));
				uvs.Add(new Vector2(uvYCoord, 1));

				if (vertexNum > 3)
				{
					AddQuad(vertexNum);
					AddQuad(vertexNum + 1);
					AddQuad(vertexNum + 2);
					//AddQuad(ref triangles, vertexNum + 3);



				}
				vertexNum += 4;

			}

			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.SetNormals(normals);
			mesh.uv = uvs.ToArray();


			Target.meshFilter.sharedMesh = mesh;
			Target.meshFilter.sharedMesh.UploadMeshData(false);
			Target.collider.sharedMesh = mesh;

		}

	}
	void AddQuad(int startingPoint)
	{
		triangles.Add(startingPoint);
		triangles.Add(startingPoint - 4);
		triangles.Add(startingPoint + 1);
		triangles.Add(startingPoint + -4);
		triangles.Add(startingPoint - 3);
		triangles.Add(startingPoint + 1);
	}

	void OnSceneGUI()
	{
		if (!Target.path)
		{
			return;
		}


		intersections = GameObject.FindObjectsOfType<Intersection>();
		var firstKnot = Target.path.Spline.ToArray()[0];
		Target.startConnector = GetSnappedConnector(firstKnot);
		SnapToConnector(Target, 0, Target.startConnector);

		var endKnot = Target.path.Spline.ToArray()[Target.path.Spline.Count - 1];
		Target.endConnector = GetSnappedConnector(endKnot);
		SnapToConnector(Target, Target.path.Spline.Count - 1, Target.endConnector, true);

	}

	Transform GetSnappedConnector(UnityEngine.Splines.BezierKnot knot)
	{
		Vector3 knotWorldPos = Target.transform.TransformPoint(knot.Position);

		foreach (Intersection intersection in intersections)
		{
			if (Vector3.Distance(knotWorldPos, intersection.transform.position) < 10)
			{
				for (int i = 0; i < intersection.connectors.Length; i++)
				{
					if (Vector3.Distance(knotWorldPos, intersection.connectors[i].position) < 1f)
					{
						intersection.connectedRoads[i] = Target;
						return intersection.connectors[i];

					}
				}
			}

		}
		return null;
	}

	static void SnapToConnector(Road Target, int index, Transform connector, bool invert = false)
	{
		if (connector == null)
		{
			return;
		}
		var knot = Target.path.Spline.ToArray()[index];
		Debug.DrawLine(Target.transform.TransformPoint(knot.Position), connector.position);

		knot.Position = Target.transform.InverseTransformPoint(connector.position);
		if (invert)
		{
			knot.Rotation = Quaternion.Inverse(Target.transform.rotation) * connector.rotation * Quaternion.Euler(Vector3.up * 180f);
		}
		else
		{
			knot.Rotation = Quaternion.Inverse(Target.transform.rotation) * connector.rotation;
		}
		Target.path.Spline.SetKnot(index, knot);
	}

	[DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
			 | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(Road))]
	static void DrawGizmos(Road Target, GizmoType selectionType)
	{
		if (!Target.path)
		{
			return;
		}
		if (Target.startConnector)
		{
			//SnapToConnector(Target, 0, Target.startConnector);
		}
		if (Target.endConnector)
		{
			//SnapToConnector(Target, Target.path.Spline.ToArray().Length-1, Target.endConnector, true);
		}

	}
}
