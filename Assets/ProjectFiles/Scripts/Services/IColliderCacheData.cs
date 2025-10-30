using System.Collections.Generic;
using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IColliderCacheData<T>
    {
        List<T> CachedElements { get; }
        List<Collider> CachedColliders { get; }
    }
}