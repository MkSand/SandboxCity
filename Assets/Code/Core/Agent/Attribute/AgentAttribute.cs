using System.Collections;
using UnityEngine;
namespace SandboxCity
{
    public class AgentAttribute : MonoBehaviour, IEntityAttribute
    {
		[SerializeField]
        private string m_AttributeName;
		[SerializeField]
		private string m_Description;

        public string Name { 
            get
            { return m_AttributeName;}
        }

        public string Description
        {
            get { return m_Description; }        
		}

		[System.Serializable]
		public class AttributeValueInstance
		{
			public AttributeValue valueType;
			public int value;
		}

		public AttributeValueInstance[] values;
    }

}