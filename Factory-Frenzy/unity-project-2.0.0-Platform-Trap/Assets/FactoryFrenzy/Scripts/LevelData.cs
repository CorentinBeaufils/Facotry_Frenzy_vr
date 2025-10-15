using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData {
    public List<PlacedObject> objects = new List<PlacedObject>();
}

[Serializable]
public class PlacedObject {
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
}
