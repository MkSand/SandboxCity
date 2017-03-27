using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityRenderer : MonoBehaviour {

	public GameObject roadSegment;
	public GameObject buildingPrefab;
	public float maxBuildingDepth = 0.2f;
	public float maxBuildingWidth = 0.4f;

	private List<GameObject> m_Roads;
	private List<GameObject> m_Buildings;

	public List<Collider2D> DrawRoads(CityGraph graph, float pointSpacing)
	{
		ClearLastObjects ();
		List<Collider2D> newObstructions = new List<Collider2D> ();

		CityGraph.Edge[] graphEdges = graph.GetEdges ();
		for (int i = 0; i < graphEdges.Length; i++) {
			GameObject segment = Instantiate<GameObject> (roadSegment);
			m_Roads.Add (segment);
			LineRenderer segmentRederer = segment.GetComponent<LineRenderer> ();
			Vector3[] positions = new Vector3[2];
			positions [0] = graphEdges [i].pointA.position.ConvertToWorldPoint(pointSpacing);
			positions [1] = graphEdges [i].pointB.position.ConvertToWorldPoint(pointSpacing);
			segmentRederer.SetPositions (positions);
			segmentRederer.widthMultiplier *= Mathf.Min(Vector3.Distance (positions [0], positions [1]), 2);
		}

		CityGraph.Lot[] lots = graph.GetLots ();

		for (int i = 0; i < lots.Length; i++) {
			GameObject building = Instantiate<GameObject> (buildingPrefab);
			m_Buildings.Add (building);
			Vector2 upVector = lots [i].corners [1].ConvertToWorldPoint (pointSpacing) - lots [i].corners [0].ConvertToWorldPoint (pointSpacing);
			Vector2 rightVector = lots [i].corners [2].ConvertToWorldPoint (pointSpacing) - lots [i].corners [1].ConvertToWorldPoint (pointSpacing);


			float width = Mathf.Min(maxBuildingWidth, rightVector.magnitude) * 0.9f;
			float depth = Mathf.Min(maxBuildingDepth,upVector.magnitude) * 0.9f;
			float height = (width + depth) / 2;
			building.transform.localScale = new Vector3(depth, width, height);
			float x = Vector2.Dot (rightVector, Vector2.right);
			float y = Vector2.Dot (rightVector, Vector2.up);
			float angle = Mathf.Rad2Deg * Mathf.Acos (x / Mathf.Max(rightVector.magnitude, 0.01f));

			if (y < 0) {
				angle *= -1;
			}
			building.transform.localEulerAngles = new Vector3 (0, 0, angle);
			Vector3 position = (Vector3)lots [i].GetCenter (pointSpacing);
			position.z = -height / 2;
			building.transform.localPosition = position;
			newObstructions.Add (building.GetComponent<Collider2D> ());
		}

		return newObstructions;
	}

	private void ClearLastObjects()
	{
		if (m_Roads == null) {
			m_Roads = new List<GameObject> ();
		} else {
			for (int i = 0; i < m_Roads.Count; i++) {
				Destroy (m_Roads [i]);
			}

			m_Roads.Clear ();
		}

		if (m_Buildings == null) {
			m_Buildings = new List<GameObject> ();
		} else {
			for (int i = 0; i < m_Buildings.Count; i++) {
				Destroy (m_Buildings [i]);
			}

			m_Buildings.Clear ();
		}
	}
		
}
