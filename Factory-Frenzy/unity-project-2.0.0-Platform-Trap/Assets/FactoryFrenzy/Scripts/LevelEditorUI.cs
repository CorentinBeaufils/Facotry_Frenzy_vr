using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorUI : MonoBehaviour
{
    public LevelEditor editor;         
    public Transform playerCamera;     

    public void SpawnPrefab(GameObject prefab)
    {
        float spawnDistance = 2f;
        Vector3 pos = playerCamera.position + playerCamera.forward * spawnDistance;

        Quaternion rot = Quaternion.identity;

        editor.PlaceObject(prefab, pos, rot);
    }
}
