using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "SandboxCity/Attribute Value", fileName = "New Attribute Value")]
public class AttributeValue : ScriptableObject
{
	public string name;
	public int min = 0;
	public int max = 1000;
}
