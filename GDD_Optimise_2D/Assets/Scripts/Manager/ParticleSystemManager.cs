using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : Singleton<ParticleSystemManager>
{
    public ParticleSystem successParticleSystem, failureParticleSystem;

    public void PlaySuccessParticle(Vector3 position)
    {
        ParticleSystem ps = SpawnSuccessParticle();
        ps.transform.localScale = new Vector3(10, 10, 1);
        ps.transform.position = position;
    }

    private ParticleSystem SpawnSuccessParticle()
    {
        GameObject psObject;
        bool existsInPool = ObjectPoolManager.Instance.SuccessParticleSystemPool.TryFetchObjectFromPool(out psObject);

        ParticleSystem ps;

        // Create one particle system since the object pool has no more particle system.
        if (!existsInPool)
        {
            ps = Instantiate(successParticleSystem);
            ObjectPoolManager.Instance.SuccessParticleSystemPool.AddObjectToPool(ps.gameObject);
        }
        else
        {
            psObject.SetActive(true);
            ps = psObject.GetComponent<ParticleSystem>();
            ps.Play();
        }

        return ps;
    }

    public void PlayFailedParticle(Vector3 position)
    {
        ParticleSystem ps = SpawnFailedParticleSystem();
        ps.transform.localScale = new Vector3(15, 15, 1);
        ps.transform.position = position;
    }

    private ParticleSystem SpawnFailedParticleSystem()
    {
        GameObject psObject;
        bool existsInPool = ObjectPoolManager.Instance.FailureParticleSystemPool.TryFetchObjectFromPool(out psObject);

        ParticleSystem ps;

        if (!existsInPool)
        {
            ps = Instantiate(failureParticleSystem);
            ObjectPoolManager.Instance.FailureParticleSystemPool.AddObjectToPool(ps.gameObject);
        }
        else
        {
            psObject.SetActive(true);
            ps = psObject.GetComponent<ParticleSystem>();
            ps.Play();
        }
        return ps;
    }
}
