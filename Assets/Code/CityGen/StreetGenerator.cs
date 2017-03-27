using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreetGenerator : MonoBehaviour {

	public LineRenderer initialRoad;
	public int iterations = 100;
	public int generations = 5;
	public int populationPerGeneration = 25;
	[Range(0.01f, 1)]
	public float populationDensity = 0.5f;
	public float increment = 1;
	public int reduceIncEvery = 100;
	public float reduceBy = 0.25f;
	public float minIncrement = 0.25f;
	public bool drawGraph = true;
	public Vector2[] initVertices;
	public Transform[] highValuePoints;
	public Vector2 center = new Vector2(50, 50);
	public Vector2 size = new Vector2(100, 100);
	public bool restrictToCircle;
	public GameObject obstructionsParent;
	public CityRenderer cityRenderer;
	private List<Collider2D> m_Obstructions;
	private List<Collider2D> m_BaseObstructions;

	public float pointDensity = 50;
	private float m_PointSpacing;
	private CityGraph m_Graph;
	private float left, right, top, bottom;
	private int m_LastIterations;
	List<GridPoint> m_RegionPoints;
	List<GridPoint> m_RemainingHighValuePoints;
//	List<GridPoint> m_FreePoints;
	Dictionary<GridPoint, GridPoint> m_FreePointsTable;

	public void Start()
	{
		left = center.x - size.x / 2;
		right = center.x + size.x / 2;
		top = center.y + size.y / 2;
		bottom = center.y - size.y / 2;
		m_BaseObstructions = new List<Collider2D>(obstructionsParent.GetComponentsInChildren<Collider2D> ());
		m_Obstructions = new List<Collider2D> (m_BaseObstructions);
		SetupPoints ();

		if (initialRoad != null) {
			Vector3[] lineRendererPositions = new Vector3[initialRoad.numPositions];
			initialRoad.GetPositions (lineRendererPositions);
			initVertices = new Vector2[lineRendererPositions.Length];

			for (int i = 0; i < initVertices.Length; i++) {
				initVertices [i] = lineRendererPositions [i];
			}
		}

		GridPoint[] initPoints = new GridPoint[initVertices.Length];

		for (int i = 0; i < initPoints.Length; i++) {
			initPoints [i] = ConvertToGridPoint (initVertices [i]);
		}
			
		CityGraph init = new CityGraph (initPoints, connectVerts: true);
		m_LastIterations = iterations;
		StartCoroutine (GenerateCity (init, iterations, generations, increment));
	}

	private IEnumerator GenerateCity(CityGraph init, int iterationsPerStep, int generations, float baseRoadIncrement)
	{
		//Each generation iteration is 
		//	1. [FIND PATHS] find paths in current area with destinations in mind	
		//	2. [PLACE BUILDINGS] place buildings of varying size based on distance to city 'centres' 
		//	3. [SIMULATE TRAVEL] simulate trips from sources (dwellings & outer roads) to destinations (all points of interest including dwellings and outer roads) 
		//	4. [PLACE ROADS] tally the number of times an edge is used in a path to destinations, discard edges that are not used in trips, the rest become roads. Base road width on path usage. 
		//	5. Repeat with the current graph as the initial state and buildings as obstructions
		m_Graph = init;

		for (int i = 0; i < generations; i++) {

			//1. FIND PATHS
			m_Graph = BuildRRT (m_Graph, iterationsPerStep, baseRoadIncrement);
			//2. PLACE BUILDINGS
			m_Graph = AddBuildingsToCity(m_Graph, populationPerGeneration * (i+1));
			//3. SIMULATE TRAVEL

			//4. PLACE ROADS
			m_Obstructions = new List<Collider2D> (m_BaseObstructions);
			m_Obstructions.AddRange(cityRenderer.DrawRoads (m_Graph, m_PointSpacing));
			yield return null;
		}



	}

	private void SetupPoints()
	{
		m_RegionPoints = new List<GridPoint> ();
		//x*x = density per 1 square. x = 1/x
		m_PointSpacing = 1/pointDensity;

		int leftX = Mathf.RoundToInt(left / m_PointSpacing);
		int rightX = Mathf.RoundToInt(right / m_PointSpacing);
		int topY = Mathf.RoundToInt(top / m_PointSpacing);
		int bottomY = Mathf.RoundToInt(bottom / m_PointSpacing);
		m_FreePointsTable = new Dictionary<GridPoint, GridPoint> ();


		for (int x = leftX; x < rightX; x++) {
			for (int y = bottomY; y < topY; y++) {
				GridPoint newPoint = new GridPoint (x, y);
				m_RegionPoints.Add(newPoint);
				m_FreePointsTable.Add (newPoint, newPoint);
			}
		}

		m_RemainingHighValuePoints = new List<GridPoint> ();

		for (int i = 0; i < highValuePoints.Length; i++) {
			m_RemainingHighValuePoints.Add (ConvertToGridPoint ((Vector2)highValuePoints [i].transform.position));
		}

	}

	private void RemovePoint(GridPoint point)
	{
		m_FreePointsTable.Remove (point);
	}

	public void Update()
	{
		if (drawGraph && m_Graph != null) 
		{
			Debug.DrawLine (new Vector2 (left, top), new Vector2 (right, top), Color.blue);
			Debug.DrawLine (new Vector2 (right, top), new Vector2 (right, bottom), Color.blue);
			Debug.DrawLine (new Vector2 (right, bottom), new Vector2 (left, bottom), Color.blue);
			Debug.DrawLine (new Vector2 (left, bottom), new Vector2 (left, top), Color.blue);

			CityGraph.Edge[] graphEdges = m_Graph.GetEdges ();
			for (int i = 0; i < graphEdges.Length; i++) 
			{
				Debug.DrawLine (
					graphEdges[i].pointA.position.ConvertToWorldPoint(m_PointSpacing), 
					graphEdges [i].pointB.position.ConvertToWorldPoint(m_PointSpacing), 
					Color.red);
			}
		}

		if (iterations > m_LastIterations) 
		{
			int newIterations = iterations - m_LastIterations;
			m_LastIterations = iterations;
			m_Graph = BuildRRT (m_Graph, newIterations, increment);
		}
	}

	private CityGraph BuildRRT(CityGraph init, int numVerticesK, float incDist)
	{		
		CityGraph outputGraph = init;
		RemoveObstructedPoints ();

		if (restrictToCircle) {
			
		}

		for (int k = 0; k < numVerticesK; k++) 
		{
			Vector2 qRand = GetRandomPoint ();
			GridPoint qNear = GetNearestPoint (qRand, init);
			float curInc = incDist - ((outputGraph.GetVertexCount() / reduceIncEvery) * reduceBy);
			curInc = Mathf.Max (curInc, minIncrement);
			GridPoint qNew = NewConfig (qNear.ConvertToWorldPoint(m_PointSpacing), qRand, curInc);
			outputGraph.AddVertex(qNew);
			outputGraph.AddEdge(qNear, qNew);
		}

		return outputGraph;
	}

	private CityGraph AddBuildingsToCity(CityGraph currentCity, int lotsToPlace)
	{
		//Sort Edges by distance from the center in travel time. 
		List<CityGraph.Edge> allEdges = new List<CityGraph.Edge>(currentCity.GetEdges());
		allEdges.Sort (CompareEdges);
		//place lots on edges (left or right)
		//Lots are made up of at least 4 points. 
		float totalLotsToPlace = lotsToPlace;

		for (int i = 0; i < allEdges.Count && lotsToPlace > 0; i++) {

			if (Random.value < (1-populationDensity))
				continue;

			if (allEdges [i].NeighbouringLotsCount < 2) {
				GridPoint point1 = allEdges [i].pointA.position;
				GridPoint point2 = allEdges [i].pointB.position;
				Vector2 point1Vec = point1.ConvertToWorldPoint (m_PointSpacing);
				Vector2 point2Vec = point2.ConvertToWorldPoint (m_PointSpacing);
				Vector2 edgeVector = point2Vec - point1Vec;
				Vector2 rhNormal = new Vector2 (edgeVector.y, -edgeVector.x);
				Vector2 lhNormal = new Vector2 (-edgeVector.y, edgeVector.x);

				CityGraph.Lot rhLot = new CityGraph.Lot (point1, point2, ConvertToGridPoint (point1Vec + rhNormal), ConvertToGridPoint (point2Vec + rhNormal));
				CityGraph.Lot lhLot = new CityGraph.Lot (point1, point2, ConvertToGridPoint (point1Vec + lhNormal), ConvertToGridPoint (point2Vec + lhNormal));

				bool coinFlip = Random.value > 0.5f;

				CityGraph.Lot lotToAdd = rhLot;
				if (coinFlip == true)
					lotToAdd = lhLot;
					
				bool success = currentCity.AddLot (allEdges [i], lotToAdd);
				if (!success) {
					lotToAdd = coinFlip ? rhLot : lhLot;
					success = currentCity.AddLot (allEdges [i], lotToAdd);
				}

				if (success) {
					lotsToPlace--;
				}
			}
		}

		return currentCity;
	}
		
	private int CompareEdges(CityGraph.Edge edge1, CityGraph.Edge edge2)
	{
		Vector2 point1a = edge1.pointA.position.ConvertToWorldPoint (m_PointSpacing);
		Vector2 point1b = edge1.pointB.position.ConvertToWorldPoint (m_PointSpacing);
		Vector2 point2a = edge2.pointA.position.ConvertToWorldPoint (m_PointSpacing);
		Vector2 point2b = edge2.pointB.position.ConvertToWorldPoint (m_PointSpacing);

		float dist1 = (Vector2.Distance (point1a, Vector2.zero) + Vector2.Distance (point1b, Vector2.zero)) * 0.5f;
		float dist2 = (Vector2.Distance (point2a, Vector2.zero) + Vector2.Distance (point2b, Vector2.zero)) * 0.5f;

		return dist1.CompareTo (dist2);
	}

	private void RemoveObstructedPoints()
	{
		List<GridPoint> toRemove = new List<GridPoint> ();

		foreach(GridPoint point in m_FreePointsTable.Keys)
		{
			Vector2 worldPoint = point.ConvertToWorldPoint (m_PointSpacing);
			bool overlaps = false;
			for (int o = 0; o < m_Obstructions.Count && !overlaps; o++) {
				overlaps |= m_Obstructions [o].OverlapPoint (worldPoint);
			}

			bool removePoint = overlaps;

			if (!removePoint && restrictToCircle) {
				float distToCenter = Vector2.Distance (worldPoint, center);
				removePoint = (distToCenter > (size.x/2.0f));
			}

			if (removePoint) {
				toRemove.Add (point);
			}
		}

		for (int i = 0; i < toRemove.Count; i++) {
			m_FreePointsTable.Remove (toRemove [i]);
		}
	}

	private Vector2 GetRandomPoint()
	{
		if (m_RemainingHighValuePoints.Count > 0 && Random.value > 0.4f) {
			int randomHVIndex = Random.Range (0, m_RemainingHighValuePoints.Count);
			GridPoint highValuePoint = m_RemainingHighValuePoints [randomHVIndex];
			return highValuePoint.ConvertToWorldPoint (m_PointSpacing);

		} else {
			int randPointIndex = Random.Range (0, m_FreePointsTable.Count);
			GridPoint newPoint = m_FreePointsTable.ElementAt (randPointIndex).Key;
			RemovePoint (newPoint);
			return newPoint.ConvertToWorldPoint (m_PointSpacing);
		}
	}

	private GridPoint GetNearestPoint(Vector2 point, CityGraph graph)
	{
		float lowest = float.MaxValue;
		int nearestIndex = -1;

		for (int i = 0; i < graph.GetVertexCount(); i++) 
		{
			float curDist = Vector2.Distance (graph.GetVertexAt(i).position.ConvertToWorldPoint(m_PointSpacing), point);

			if (curDist < lowest) 
			{
				lowest = curDist;
				nearestIndex = i;
			}
		}

		return graph.GetVertexAt (nearestIndex).position;
	}

	private GridPoint NewConfig(Vector2 nearPoint, Vector2 randPoint, float dist)
	{
		Vector2 directionVector = randPoint - nearPoint;
		Vector2 newPoint =  nearPoint + (directionVector.normalized * dist);
		GridPoint newGridPoint = ConvertToGridPoint(newPoint);
		RemovePoint(newGridPoint);
		TryRemoveHighValuePoint (newGridPoint);
		return newGridPoint;
	}	

	private void TryRemoveHighValuePoint(GridPoint point)
	{
		Vector2 worldPoint = point.ConvertToWorldPoint (m_PointSpacing);

		for (int i = m_RemainingHighValuePoints.Count-1; i >= 0; i--) {
			if (Vector2.Distance (m_RemainingHighValuePoints [i].ConvertToWorldPoint (m_PointSpacing), worldPoint) < 0.5f) {
				m_RemainingHighValuePoints.RemoveAt (i);
			}
		}
	}

	private Vector2 SnapToGrid(Vector2 point)
	{
		int xCount = Mathf.RoundToInt(point.x / m_PointSpacing);
		int yCount = Mathf.RoundToInt(point.y / m_PointSpacing);

		return new Vector2 (xCount * m_PointSpacing, yCount * m_PointSpacing);
	}

	private GridPoint ConvertToGridPoint(Vector2 point)
	{		
		int xCount = Mathf.RoundToInt(point.x / m_PointSpacing);
		int yCount = Mathf.RoundToInt(point.y / m_PointSpacing);

		return new GridPoint (xCount, yCount);
	}
		
}


