using System.Collections;
namespace SandboxCity
{
    public class Relation
    {
        public enum RelationType { Biological, Affinity, Role}
        private RelationType m_Type;
        private Agent m_Target;
        private uint m_RelationID;

        public RelationType Type
        {
            get
            {
                return m_Type;
            }
        }

        public Agent Target
        {
            get
            {
                return m_Target;
            }
        }

        public uint RelationID
        {
            get
            {
                return m_RelationID;
            }
        }

        public Relation(RelationType type, Agent target, uint relationID)
        {
            m_Type = type;
            m_Target = target;
            m_RelationID = relationID;
        }
    }
}