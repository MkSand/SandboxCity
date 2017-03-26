using UnityEngine;
using System.Collections;
namespace SandboxCity
{
    public interface IEntity : INameable
    {
        int Age { get; }
        IEntityAttribute[] GetAttributes();
        void AddAttribute(IEntityAttribute newAttribute);
    }
}
