using UnityEngine;
using System.Collections;
namespace SandboxCity
{
    public interface IEntityAttribute : INameable
    {
        string Description { get;}
    }
}