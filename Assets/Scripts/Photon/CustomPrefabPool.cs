using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CustomPrefabPool : IPunPrefabPool
{
    private readonly Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();

    public void RegisterPrefab(string prefabId, GameObject prefab)
    {
        if (!prefabCache.ContainsKey(prefabId)) prefabCache.Add(prefabId, prefab);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (prefabCache.TryGetValue(prefabId, out GameObject prefab))
        {
            GameObject instance = Object.Instantiate(prefab, position, rotation);
            instance.SetActive(false);
            return instance;
        }

        Debug.LogError("Prefab not found in CustomPrefabPool: " + prefabId);
        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}