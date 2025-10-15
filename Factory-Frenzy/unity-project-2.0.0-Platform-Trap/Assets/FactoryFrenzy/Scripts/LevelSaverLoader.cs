using System.IO;
using UnityEngine;

public class LevelSaverLoader : MonoBehaviour {
    public LevelEditor editor;

    public void SaveLevel(string filePath) {
        LevelData data = new LevelData();

        foreach (var obj in editor.placedObjects) {
            var info = new PlacedObject {
                prefabName = obj.name,
                position = obj.transform.position,
                rotation = obj.transform.rotation
            };
            data.objects.Add(info);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Niveau sauvegardé : " + filePath);
    }

    public void LoadLevel(string filePath) {
        if (!File.Exists(filePath)) {
            Debug.LogError("Fichier non trouvé : " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        // Nettoie l'existant
        foreach (var obj in editor.placedObjects)
            Destroy(obj);
        editor.placedObjects.Clear();

        // Recharge les objets
        foreach (var info in data.objects) {
            GameObject prefab = editor.availablePrefabs.Find(p => p.name == info.prefabName);
            if (prefab != null) {
                editor.PlaceObject(prefab, info.position, info.rotation);
            } else {
                Debug.LogWarning("Prefab non trouvé : " + info.prefabName);
            }
        }

        Debug.Log("Niveau chargé : " + filePath);
    }
}
