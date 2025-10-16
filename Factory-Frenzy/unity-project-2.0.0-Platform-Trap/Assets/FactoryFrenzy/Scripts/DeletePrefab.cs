using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeleteOnClick : MonoBehaviour
{
    private XRGrabInteractable interactable;
    private LevelEditor levelEditor;
    void Start()
    {
        // R�cup�re le composant XR Interactable
        interactable = GetComponent<XRGrabInteractable>();

        if (interactable != null)
        {
            interactable.activated.AddListener(OnObjectActivated);
        }
        else
        {
            Debug.LogError("Aucun composant XRBaseInteractable trouv� sur " + gameObject.name);
        }
        levelEditor = FindObjectOfType<LevelEditor>();
    }

    public void OnObjectActivated(ActivateEventArgs args)
    {
        if (levelEditor != null)
        {
            levelEditor.RemoveObject(gameObject); // supprime proprement
        }
        else
        {
            Destroy(gameObject); // fallback si pas trouv�
        }
        // Supprime l'objet quand il est activ� (hover + bouton activate)
        // Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Nettoie l'abonnement pour �viter les fuites m�moire
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnObjectActivated);
        }
    }
}