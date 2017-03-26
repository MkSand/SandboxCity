using System.Collections;
namespace SandboxCity
{
    public struct Vector2D
    {
        public float x;
        public float y;

        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        static public Vector2D Zero { get { return new Vector2D(0, 0); } }
        static public Vector2D One { get { return new Vector2D(1, 1); } }
    }

    public struct Vector3D
    {
        public float x;
        public float y;
        public float z;

        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        static public Vector3D Zero { get { return new Vector3D(0, 0, 0); } }
        static public Vector3D One { get { return new Vector3D(1, 1, 1); } }
    }

}