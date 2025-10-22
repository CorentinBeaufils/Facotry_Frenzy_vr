using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeleteOnClick : MonoBehaviour
{
    private XRGrabInteractable interactable;
    private LevelEditor levelEditor;
    public AudioClip deleteSound;
    public float volume = 1.0f;
    void Start()
    {
        // Récupère le composant XR Interactable
        interactable = GetComponent<XRGrabInteractable>();

        if (interactable != null)
        {
            interactable.activated.AddListener(OnObjectActivated);
        }
        else
        {
            Debug.LogError("Aucun composant XRBaseInteractable trouvé sur " + gameObject.name);
        }
        levelEditor = FindObjectOfType<LevelEditor>();
    }

    public void OnObjectActivated(ActivateEventArgs args)
    {
        if (levelEditor != null)
        {
            levelEditor.RemoveObject(gameObject); // supprime proprement
            if (deleteSound != null)
            {
                AudioSource.PlayClipAtPoint(deleteSound, transform.position, volume);
            }
        }
        else
        {
            Destroy(gameObject); // fallback si pas trouvé
        }
        // Supprime l'objet quand il est activé (hover + bouton activate)
        // Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Nettoie l'abonnement pour éviter les fuites mémoire
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnObjectActivated);
        }
    }
}