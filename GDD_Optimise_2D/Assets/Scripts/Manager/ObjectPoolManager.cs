using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public ObjectPool FrameObjectPool { get; private set; }

    public ObjectPool SuccessParticleSystemPool { get; private set; }

    public ObjectPool FailureParticleSystemPool { get; private set; }

    private void Awake()
    {
        FrameObjectPool = new ObjectPool();
        SuccessParticleSystemPool = new ObjectPool();
        FailureParticleSystemPool = new ObjectPool();
    }
}
