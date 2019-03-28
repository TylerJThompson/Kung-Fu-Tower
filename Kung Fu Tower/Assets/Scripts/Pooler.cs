using UnityEngine;
using System.Collections.Generic;

public class Pooler : MonoBehaviour
{
    public GameObject pooledObject;

    private int alreadyPooled = 0;

    public List<GameObject> pool = new List<GameObject>();

    public void createPool(int poolSize)
    {
        int temp = alreadyPooled;
        for (int i = temp; i < poolSize; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pool.Add(obj);
            alreadyPooled++;
        }
    }

    public GameObject getPooledObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null && !pool[i].activeSelf)
            {
                return pool[i];
            }
        }
        return null;
    }
}