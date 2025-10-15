using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSaveUI : MonoBehaviour {
    public LevelSaverLoader saver;

    public void OnSaveClick() {
        string sceneName = SceneManager.GetActiveScene().name;
        string path = Application.streamingAssetsPath + "/" + sceneName + ".json";
        saver.SaveLevel(path);
        Debug.Log("Niveau sauvegardé : " + path);
    }

    public void OnLoadClick() {
        string sceneName = SceneManager.GetActiveScene().name;
        string path = Application.streamingAssetsPath + "/" + sceneName + ".json";
        saver.LoadLevel(path);
        Debug.Log("Niveau chargé : " + path);
    }
}
