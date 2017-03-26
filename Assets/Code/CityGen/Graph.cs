using System.Collections;
using System.Collections.Generic;

//public class CityGraph
//{
//	public class Edge
//	{
//		public Vertex pointA;
//		public Vertex pointB;
//		public int travelTally;
//
//		public Lot[] neighbouringLots;
//		public Lot leftLot;
//		public Lot rightLot;
//
//		public Edge (Vertex a, Vertex b)
//		{
//			pointA = a;
//			pointB = b;
//			travelTally = 0;
//			neighbouringLots = new Lot[2];
//		}
//
//		public int NeighbouringLotsCount
//		{
//			get {
//
//				return (leftLot [0] == null ? 0 : 1) + (rightLot [1] == null ? 0 : 1);
//			}
//		}
//	}
//
//	public class Lot
//	{
//		public GridPoint[] corners;
//		private int m_MinX, m_MaxX, m_MinY, m_MaxY;
//
//		public Lot(GridPoint point1, GridPoint point2, GridPoint point3, GridPoint point4)
//		{
//			GridPoint[] unsortedCorners = new GridPoint[]{point1, point2, point3, point4};
//
//			m_MinX = int.MaxValue;
//			m_MaxX = int.MinValue;
//			m_MinY = int.MaxValue;
//			m_MaxY = int.MinValue;
//
//			for(int i = 0; i < corners.Length; i++)
//			{
//				if(corners[i].xPos < m_MinX)
//				{
//					m_MinX = corners[i].xPos;
//				}
//
//				if(corners[i].xPos > m_MaxX)
//				{
//					m_MaxX = corners[i].xPos;
//				}
//
//				if(corners[i].yPos < m_MinY)
//				{
//					m_MinY = corners[i].yPos;
//				}
//
//				if(corners[i].yPos > m_MaxY)
//				{
//					m_MaxY = corners[i].yPos;
//				}
//			}
//
//			corners = new GridPoint[4];
//			//Corners are sorted from bottom left to top right -> BL, TL, BR, TR
//
//			for(int i = 0; i < unsortedCorners.Length; i++)
//			{
//				//First is the smallest X that is lower that largest Y
//				if(unsortedCorners[i].xPos == m_MinX && unsortedCorners[i].yPos < m_MaxY)
//				{
//					corners[0] = unsortedCorners[i];
//				}
//				//Second is the largest Y that is lower than the largest X
//				else if(unsortedCorners[i].yPos == m_MaxY && unsortedCorners[i].xPos < m_MaxX)
//				{
//					corners[1] = unsortedCorners[i];
//				}
//				//Third is the largest X that is greater than the smallest Y
//				else if(unsortedCorners[i].xPos == m_MaxX && unsortedCorners[i].yPos > m_MinY)
//				{
//					corners[2] = unsortedCorners[i];
//				}
//				//Fourth is the smallest Y that is lower than the largest X
//				else if(unsortedCorners[i].yPos == m_MinY && unsortedCorners[i].xPos < m_MaxX)
//				{
//					corners[3] = unsortedCorners[i];
//				}
//			}				
//		}
//
//		public bool Overlaps(Lot other)
//		{
//			bool atLeastOneOverlaps = false;
//
//			for (int i = 0; i < other.corners.Length; i++) {
//				
//			}
//		}
//
//		public override bool Equals (object obj)
//		{
//			Lot other = obj as Lot;
//
//			if (other != null) {
//
//				bool allMatch = corners.Length == other.corners.Length;
//
//				for (int i = 0; i < corners.Length && allMatch; i++) {
//					allMatch &= corners [i].Equals(other.corners [i]);
//				}
//
//				return allMatch;
//			}
//
//			return false;
//		}
//	}
//
//	public class Vertex
//	{
//		public GridPoint position;
//		public List<Edge> connectedEdges;
//
//		public Vertex(GridPoint gridPos)
//		{
//			position = gridPos;
//			connectedEdges = new List<Edge>();
//		}
//	}
//
//
//	private List<Vertex> vertices;
//	private List<Edge> edges;
//	private Dictionary<GridPoint, Vertex> m_VertexTable;
//
//	public void AddVertex(GridPoint vertexPoint)
//	{
//		Vertex newVertex;
//
//		if (!m_VertexTable.TryGetValue (vertexPoint, out newVertex)) {
//			newVertex = new Vertex (vertexPoint);
//
//			m_VertexTable.Add (vertexPoint, newVertex);
//			vertices.Add (newVertex);
//		}
//	}
//
//	public void AddEdge(GridPoint pointA, GridPoint pointB)
//	{
//		Vertex vertexA, vertexB;
//
//		if (!m_VertexTable.TryGetValue (pointA, out vertexA)) {
//			vertexA = new Vertex (pointA);
//		}
//
//		if (!m_VertexTable.TryGetValue (pointB, out vertexB)) {
//			vertexB = new Vertex (pointB);
//		}
//
//		Edge newEdge = new Edge (vertexA, vertexB);
//
//		edges.Add (newEdge);
//		vertexA.connectedEdges.Add (newEdge);
//		vertexB.connectedEdges.Add (newEdge);
//	}
//
//	public Vertex GetVertexAt(int index)
//	{
//		if (index >= 0 && index < vertices.Count) {
//			return vertices [index];
//		} else {
//			return null;
//		}
//	}
//
//	public Edge[] GetEdges()
//	{
//		return edges.ToArray ();
//	}
//
//	public Vertex[] GetVertices()
//	{
//		return vertices.ToArray ();
//	}
//
//	public int GetVertexCount()
//	{
//		return vertices.Count;
//	}
//
//	public CityGraph()
//	{
//		vertices = new List<Vertex>();
//		edges = new List<Edge>();
//		m_VertexTable = new Dictionary<GridPoint, Vertex>();
//	}
//
//	public CityGraph(GridPoint[] initVerts, bool connectVerts)
//	{
//		vertices = new List<Vertex>();
//		edges = new List<Edge>();
//		m_VertexTable = new Dictionary<GridPoint, Vertex>();
//
//		for(int i = 0; i < initVerts.Length; i++)
//		{
//			AddVertex(initVerts[i]);
//
//			if(i > 0)
//			{
//				AddEdge(initVerts[i-1], initVerts[i]);
//			}
//		}
//	}
//}


