using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class DeleteInputManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tag des objets qui peuvent être supprimés (optionnel)")]
    public string deletableTag = "Deletable";

    [Tooltip("Si true, vérifie le tag. Si false, tous les objets peuvent être supprimés")]
    public bool checkTag = false;

    private ActionBasedController controller;
    private XRRayInteractor rayInteractor;
    private XRDirectInteractor directInteractor;

    private bool wasActivatePressed = false;

    void Start()
    {
        controller = GetComponent<ActionBasedController>();
        rayInteractor = GetComponent<XRRayInteractor>();
        directInteractor = GetComponent<XRDirectInteractor>();

        Debug.Log("DeleteInputManager initialisé sur " + gameObject.name);
    }

    void Update()
    {
        if (controller == null || controller.activateAction == null) return;

        // Lit la valeur du bouton activate
        float activateValue = controller.activateAction.action.ReadValue<float>();
        bool isActivatePressed = activateValue > 0.5f;

        // Détecte le moment où le bouton est pressé (pas maintenu)
        if (isActivatePressed && !wasActivatePressed)
        {
            TryDeleteObject();
        }

        wasActivatePressed = isActivatePressed;
    }

    private void TryDeleteObject()
    {
        GameObject objectToDelete = null;

        // Priorité 1: Objet actuellement grabbed (selected)
        if (directInteractor != null && directInteractor.hasSelection)
        {
            objectToDelete = directInteractor.interactablesSelected[0].transform.gameObject;
            Debug.Log("Objet grabbed détecté: " + objectToDelete.name);
        }
        // Priorité 2: Objet hovré par le ray interactor
        else if (rayInteractor != null && rayInteractor.interactablesHovered.Count > 0)
        {
            objectToDelete = rayInteractor.interactablesHovered[0].transform.gameObject;
            Debug.Log("Objet hovré détecté: " + objectToDelete.name);
        }
        // Priorité 3: Objet hovré par le direct interactor
        else if (directInteractor != null && directInteractor.interactablesHovered.Count > 0)
        {
            objectToDelete = directInteractor.interactablesHovered[0].transform.gameObject;
            Debug.Log("Objet hovré (direct) détecté: " + objectToDelete.name);
        }

        // Supprime l'objet si trouvé et autorisé
        if (objectToDelete != null)
        {
            // Vérifie le tag si nécessaire
            if (checkTag && !objectToDelete.CompareTag(deletableTag))
            {
                Debug.Log("Objet " + objectToDelete.name + " ne peut pas être supprimé (mauvais tag)");
                return;
            }

            Debug.Log("SUPPRESSION de " + objectToDelete.name);
            Destroy(objectToDelete);
        }
        else
        {
            Debug.Log("Aucun objet à supprimer");
        }
    }
}