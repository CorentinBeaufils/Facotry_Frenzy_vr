using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class DeleteInputManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tag des objets qui peuvent �tre supprim�s (optionnel)")]
    public string deletableTag = "Deletable";

    [Tooltip("Si true, v�rifie le tag. Si false, tous les objets peuvent �tre supprim�s")]
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

        Debug.Log("DeleteInputManager initialis� sur " + gameObject.name);
    }

    void Update()
    {
        if (controller == null || controller.activateAction == null) return;

        // Lit la valeur du bouton activate
        float activateValue = controller.activateAction.action.ReadValue<float>();
        bool isActivatePressed = activateValue > 0.5f;

        // D�tecte le moment o� le bouton est press� (pas maintenu)
        if (isActivatePressed && !wasActivatePressed)
        {
            TryDeleteObject();
        }

        wasActivatePressed = isActivatePressed;
    }

    private void TryDeleteObject()
    {
        GameObject objectToDelete = null;

        // Priorit� 1: Objet actuellement grabbed (selected)
        if (directInteractor != null && directInteractor.hasSelection)
        {
            objectToDelete = directInteractor.interactablesSelected[0].transform.gameObject;
            Debug.Log("Objet grabbed d�tect�: " + objectToDelete.name);
        }
        // Priorit� 2: Objet hovr� par le ray interactor
        else if (rayInteractor != null && rayInteractor.interactablesHovered.Count > 0)
        {
            objectToDelete = rayInteractor.interactablesHovered[0].transform.gameObject;
            Debug.Log("Objet hovr� d�tect�: " + objectToDelete.name);
        }
        // Priorit� 3: Objet hovr� par le direct interactor
        else if (directInteractor != null && directInteractor.interactablesHovered.Count > 0)
        {
            objectToDelete = directInteractor.interactablesHovered[0].transform.gameObject;
            Debug.Log("Objet hovr� (direct) d�tect�: " + objectToDelete.name);
        }

        // Supprime l'objet si trouv� et autoris�
        if (objectToDelete != null)
        {
            // V�rifie le tag si n�cessaire
            if (checkTag && !objectToDelete.CompareTag(deletableTag))
            {
                Debug.Log("Objet " + objectToDelete.name + " ne peut pas �tre supprim� (mauvais tag)");
                return;
            }

            Debug.Log("SUPPRESSION de " + objectToDelete.name);
            Destroy(objectToDelete);
        }
        else
        {
            Debug.Log("Aucun objet � supprimer");
        }
    }
}