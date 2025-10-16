using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour {
    public List<GameObject> availablePrefabs;
    [HideInInspector]
    public List<GameObject> placedObjects = new List<GameObject>();

    public void PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation) {
        GameObject obj = Instantiate(prefab, position, rotation);
        obj.name = prefab.name; 
        placedObjects.Add(obj);
    }

    public void RemoveObject(GameObject obj) {
        placedObjects.Remove(obj);
        Destroy(obj);
    }
}
