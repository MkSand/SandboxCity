using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SandboxCity
{
    public abstract class BasicEntity : MonoBehaviour, IEntity
    {
		//private List<AgentAttribute> m_Attributes;
        private string m_Name;
        //Age is number of minutes
        private int m_Age;

        public string Name
        {
            get { return m_Name; }
        }

        public int Age
        {
            get { return m_Age; }
        }
			

		public IEntityAttribute[] GetAttributes()
		{
			return GetComponents<AgentAttribute> () as IEntityAttribute[];
			//return m_Attributes.ToArray() as IEntityAttribute[];
		}

		public void AddAttribute(IEntityAttribute newAttribute)
		{
			AgentAttribute newAgentAttribute = gameObject.AddComponent<AgentAttribute> ();

			//if (newAgentAttribute != null)
			//{
			//	m_Attributes.Add(newAgentAttribute);
			//}
		}
    }
}