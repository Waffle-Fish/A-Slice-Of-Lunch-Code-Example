using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaucePool : MonoBehaviour
{
    public static SaucePool Instance;

    [Header("Pooling Settings")]
    public GameObject splatPrefab;
    public int initialPoolSize = 20;
    public float spriteSize = 2;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject splat = Instantiate(splatPrefab, this.transform);
            splat.SetActive(false);
            pool.Add(splat);
        }
    }

    public GameObject GetSplat(Vector3 position, Quaternion rotation)
    {
        foreach (var splat in pool)
        {
            if (!splat.activeInHierarchy)
            {
                ActivateSplat(splat, position, rotation);
                return splat;
            }
        }

        // Optional: grow the pool if needed
        GameObject newSplat = Instantiate(splatPrefab);
        ActivateSplat(newSplat, position, rotation);
        pool.Add(newSplat);
        return newSplat;
    }

    private void ActivateSplat(GameObject splat, Vector3 position, Quaternion rotation)
    {
        splat.transform.position = position;
        splat.transform.rotation = rotation;
        splat.transform.localScale = new Vector3(spriteSize, spriteSize, spriteSize);
        splat.SetActive(true);
    }
}
