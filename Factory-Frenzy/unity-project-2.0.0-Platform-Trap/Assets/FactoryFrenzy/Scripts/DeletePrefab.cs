using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DeleteOnClick : MonoBehaviour
{
    private XRGrabInteractable interactable;

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
    }

    public void OnObjectActivated(ActivateEventArgs args)
    {
        // Supprime l'objet quand il est activé (hover + bouton activate)
        Destroy(gameObject);
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