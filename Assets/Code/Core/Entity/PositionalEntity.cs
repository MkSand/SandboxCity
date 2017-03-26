using System.Collections;
namespace SandboxCity
{
    public abstract class PositionalEntity : BasicEntity
    {
        private Vector3D m_WorldPosition;

        public Vector3D WorldPosition
        {
            get
            { return m_WorldPosition; }

            set
            //Trigger some update here possibly?
            { m_WorldPosition = value; }            
        }
    }
}