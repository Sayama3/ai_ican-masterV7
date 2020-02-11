using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rocket pool struct
/// Optimise : get first available rocket without loop
/// </summary>
public struct RocketPool
{
    private Rocket[] pool;
    private GameObject poolParent;
    
    public void InitPool(int size, GameObject prefab)
    {
        this.poolParent = new GameObject("RocketPool");
        this.pool = new Rocket[size];
        for (int i = 0; i < this.pool.Length; i++)
        {
            this.pool[i] = Object.Instantiate(prefab,this.poolParent.transform).GetComponent<Rocket>();
            this.pool[i].GoToSleep();
        }
    }

    public GameObject[] GetAliveRocket()
    {
        var rockets = new List<GameObject>();
        foreach (var rocket in this.pool)
        {
            if (rocket.gameObject.activeSelf)
            {
                rockets.Add(rocket.gameObject);
            }            
        }
        return rockets.ToArray();
    }

    /// <summary>
    /// Return the first available rocket
    /// </summary>
    /// <returns></returns>
    public Rocket GetAvailableRocket()
    {
        foreach (var rocket in this.pool)
        {
            if (rocket.Available)
            {
                return rocket;
            }
        }
        return null;
    }

    public void Clear()
    {
        foreach (var rocket in this.pool)
        {
            Object.Destroy(rocket.gameObject);
        }

        this.pool = null;
    }
}