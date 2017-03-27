using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CityGraph
{
	public class Edge
	{
		public Vertex pointA;
		public Vertex pointB;
		public int travelTally;

		public Lot[] adjacentLots;

		public Edge (Vertex a, Vertex b)
		{
			pointA = a;
			pointB = b;
			travelTally = 0;
			adjacentLots = new Lot[2];
		}

		public int NeighbouringLotsCount
		{
			get {

				return (adjacentLots[0] == null ? 0 : 1) + (adjacentLots[1] == null ? 0 : 1);
			}
		}
	}

	public class Lot
	{
		public GridPoint[] corners;
		private int m_MinX, m_MaxX, m_MinY, m_MaxY;

		public Lot(GridPoint edgePoint1, GridPoint edgePoint2, GridPoint ep1Normal, GridPoint ep2Normal)
		{
			corners = new GridPoint[]{edgePoint1, edgePoint2, ep2Normal, ep1Normal};

			m_MinX = int.MaxValue;
			m_MaxX = int.MinValue;
			m_MinY = int.MaxValue;
			m_MaxY = int.MinValue;

			for(int i = 0; i < corners.Length; i++)
			{
				if(corners[i].xPos < m_MinX)
				{
					m_MinX = corners[i].xPos;
				}

				if(corners[i].xPos > m_MaxX)
				{
					m_MaxX = corners[i].xPos;
				}

				if(corners[i].yPos < m_MinY)
				{
					m_MinY = corners[i].yPos;
				}

				if(corners[i].yPos > m_MaxY)
				{
					m_MaxY = corners[i].yPos;
				}
			}

//			corners = new GridPoint[4];
//			//Corners are sorted from bottom left to top right -> BL, TL, BR, TR
//
//			for(int i = 0; i < corners.Length; i++)
//			{
//				//First is the smallest X that is lower that largest Y
//				if(corners[i].xPos == m_MinX && corners[i].yPos < m_MaxY)
//				{
//					corners[0] = corners[i];
//				}
//				//Second is the largest Y that is lower than the largest X
//				else if(corners[i].yPos == m_MaxY && corners[i].xPos < m_MaxX)
//				{
//					corners[1] = corners[i];
//				}
//				//Third is the largest X that is greater than the smallest Y
//				else if(corners[i].xPos == m_MaxX && corners[i].yPos > m_MinY)
//				{
//					corners[2] = corners[i];
//				}
//				//Fourth is the smallest Y that is lower than the largest X
//				else if(corners[i].yPos == m_MinY && corners[i].xPos < m_MaxX)
//				{
//					corners[3] = corners[i];
//				}
//			}				
		}

		public Vector2 GetCenter(float pointSpacing)
		{
			Vector2 point1 = corners [0].ConvertToWorldPoint (pointSpacing);
			Vector2 point2 = corners [1].ConvertToWorldPoint (pointSpacing);
			Vector2 point3 = corners [2].ConvertToWorldPoint (pointSpacing);
			Vector2 point4 = corners [3].ConvertToWorldPoint (pointSpacing);

			return (point1 + point2 + point3 + point4) / 4;
		}

		//TODO: Implement this properly
		public bool Overlaps(Lot other)
		{
			bool atLeastOneOverlaps = false;
			int numMatchingCorners = 0;

			for (int i = 0; i < other.corners.Length; i++) {
				for (int j = 0; j < corners.Length; j++) {
					if (corners [j] == other.corners [i])
						numMatchingCorners++;
				}
			}

			return numMatchingCorners <3;
		}


		public override bool Equals (object obj)
		{
			Lot other = obj as Lot;

			if (other != null) {

				bool allMatch = corners.Length == other.corners.Length;

				for (int i = 0; i < corners.Length && allMatch; i++) {
					allMatch &= corners [i].Equals(other.corners [i]);
				}

				return allMatch;
			}

			return false;
		}
	}

	public class Vertex
	{
		public GridPoint position;
		public List<Edge> connectedEdges;

		public Vertex(GridPoint gridPos)
		{
			position = gridPos;
			connectedEdges = new List<Edge>();
		}
	}


	private List<Vertex> vertices;
	private List<Edge> edges;
	private List<Lot> lots;
	private Dictionary<GridPoint, Vertex> m_VertexTable;

	public void AddVertex(GridPoint vertexPoint)
	{
		Vertex newVertex;

		if (!m_VertexTable.TryGetValue (vertexPoint, out newVertex)) {
			newVertex = new Vertex (vertexPoint);

			m_VertexTable.Add (vertexPoint, newVertex);
			vertices.Add (newVertex);
		}
	}

	public void AddEdge(GridPoint pointA, GridPoint pointB)
	{
		Vertex vertexA, vertexB;

		if (!m_VertexTable.TryGetValue (pointA, out vertexA)) {
			vertexA = new Vertex (pointA);
		}

		if (!m_VertexTable.TryGetValue (pointB, out vertexB)) {
			vertexB = new Vertex (pointB);
		}

		Edge newEdge = new Edge (vertexA, vertexB);

		edges.Add (newEdge);
		vertexA.connectedEdges.Add (newEdge);
		vertexB.connectedEdges.Add (newEdge);
	}

	public bool AddLot(Edge adjacentEdge, Lot lot)
	{
		bool overlapsFirstLot = adjacentEdge.adjacentLots[0] != null && adjacentEdge.adjacentLots[0].Overlaps(lot);
		bool overlapsSecondLot = adjacentEdge.adjacentLots[1] != null && adjacentEdge.adjacentLots[1].Overlaps(lot);
		bool success = false;

		if (adjacentEdge.adjacentLots [0] == null && !overlapsSecondLot){			
			adjacentEdge.adjacentLots [0] = lot;
			success = true;
		} else if (adjacentEdge.adjacentLots [1] == null && !overlapsFirstLot) {
			adjacentEdge.adjacentLots[1] = lot;
			success = true;
		}

		if (success) {
			lots.Add (lot);
		}

		return success;
	}

	public Vertex GetVertexAt(int index)
	{
		if (index >= 0 && index < vertices.Count) {
			return vertices [index];
		} else {
			return null;
		}
	}

	public Edge[] GetEdges()
	{
		return edges.ToArray ();
	}

	public Vertex[] GetVertices()
	{
		return vertices.ToArray ();
	}

	public Lot[] GetLots()
	{
		return lots.ToArray ();
	}

	public int GetVertexCount()
	{
		return vertices.Count;
	}

	public CityGraph()
	{
		vertices = new List<Vertex>();
		edges = new List<Edge>();
		lots = new List<Lot> ();
		m_VertexTable = new Dictionary<GridPoint, Vertex>();
	}

	public CityGraph(GridPoint[] initVerts, bool connectVerts)
	{
		vertices = new List<Vertex>();
		edges = new List<Edge>();
		lots = new List<Lot> ();
		m_VertexTable = new Dictionary<GridPoint, Vertex>();

		for(int i = 0; i < initVerts.Length; i++)
		{
			AddVertex(initVerts[i]);

			if(i > 0)
			{
				AddEdge(initVerts[i-1], initVerts[i]);
			}
		}
	}
}


