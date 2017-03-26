using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPoint : System.IEquatable<GridPoint>
{
	public int xPos;
	public int yPos;

	public GridPoint(int x, int y)
	{
		xPos = x;
		yPos = y;
	}

	public override bool Equals (object obj)
	{
		if (obj is GridPoint) {
			return Equals ((GridPoint)obj);
		}

		return false;
	}

	public bool Equals (GridPoint gPoint)
	{
		return xPos == gPoint.xPos && yPos == gPoint.yPos;
	}

	public override int GetHashCode ()
	{
		return xPos.GetHashCode () * yPos.GetHashCode ();
	}

	public static bool operator ==(GridPoint point1, GridPoint point2)
	{
		return point1.xPos == point2.xPos && point1.yPos == point2.yPos;
	}

	public static bool operator !=(GridPoint point1, GridPoint point2)
	{
		return point1.xPos != point2.xPos || point1.yPos != point2.yPos;
	}

	public Vector2 ConvertToWorldPoint(float pointSpacing)
	{
		return new Vector2 (xPos * pointSpacing, yPos * pointSpacing);
	}
}