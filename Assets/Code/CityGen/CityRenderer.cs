using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityRenderer : MonoBehaviour {

	public GameObject roadSegment;

	public void DrawRoads(CityGraph graph, float pointSpacing)
	{
		CityGraph.Edge[] graphEdges = graph.GetEdges ();
		for (int i = 0; i < graphEdges.Length; i++) {
			GameObject segment = Instantiate<GameObject> (roadSegment);
			LineRenderer segmentRederer = segment.GetComponent<LineRenderer> ();
			Vector3[] positions = new Vector3[2];
			positions [0] = graphEdges [i].pointA.position.ConvertToWorldPoint(pointSpacing);
			positions [1] = graphEdges [i].pointB.position.ConvertToWorldPoint(pointSpacing);
			segmentRederer.SetPositions (positions);
			segmentRederer.widthMultiplier *= Mathf.Min(Vector3.Distance (positions [0], positions [1]), 2);
		}
	}
		
}
