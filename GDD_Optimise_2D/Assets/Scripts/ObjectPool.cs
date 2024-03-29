﻿using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private HashSet<GameObject> pool;

    public ObjectPool()
    {
        pool = new HashSet<GameObject>();
    }

    public void AddObjectToPool(GameObject obj, bool setInactive = false)
    {

        if (setInactive)
        {
            obj.SetActive(false);
        }

        pool.Add(obj);
    }

    /// <summary>
    /// Tries to fetch a game object from the object pool.
    /// </summary>
    /// <param name="fetchedObjects">The fetched game object. (Null if doesn't exists)</param>
    /// <returns>True if a game object was fetched. False if none was found.</returns>
    public bool TryFetchObjectFromPool(out GameObject fetchedObjects)
    {

        fetchedObjects = null;
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                fetchedObjects = obj;
                break;
            }
        }

        return fetchedObjects != null;
    }
}