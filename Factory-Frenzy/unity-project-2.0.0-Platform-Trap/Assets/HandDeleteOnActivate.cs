using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDeleteOnActivate : MonoBehaviour
{
    private XRBaseInteractor interactor;          // La main ou le rayon
    private GameObject hoveredObject;             // L’objet actuellement survolé

    [Header("Bouton d'activation")]
    public InputActionProperty activateAction;    // Référence vers l’action "Activate" (Trigger)

    void Start()
    {
        interactor = GetComponent<XRBaseInteractor>();

        if (interactor == null)
        {
            Debug.LogError("Aucun XRBaseInteractor trouvé sur " + gameObject.name);
            return;
        }

        // Abonnement aux événements de survol
        interactor.hoverEntered.AddListener(OnHoverEnter);
        interactor.hoverExited.AddListener(OnHoverExit);

        // Abonnement à l’action d’activation
        if (activateAction != null && activateAction.action != null)
        {
            activateAction.action.performed += OnActivatePressed;
        }
        else
        {
            Debug.LogWarning("Aucune action 'Activate' assignée sur " + gameObject.name);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        hoveredObject = args.interactableObject.transform.gameObject;
        Debug.Log("Hover enter : " + hoveredObject.name);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (hoveredObject == args.interactableObject.transform.gameObject)
        {
            hoveredObject = null;
            Debug.Log("Hover exit : " + args.interactableObject.transform.name);
        }
    }

    private void OnActivatePressed(InputAction.CallbackContext ctx)
    {
        if (hoveredObject != null)
        {
            Debug.Log("Suppression de : " + hoveredObject.name);
            Destroy(hoveredObject);
        }
    }

    void OnDestroy()
    {
        if (interactor != null)
        {
            interactor.hoverEntered.RemoveListener(OnHoverEnter);
            interactor.hoverExited.RemoveListener(OnHoverExit);
        }

        if (activateAction != null && activateAction.action != null)
        {
            activateAction.action.performed -= OnActivatePressed;
        }
    }
}
