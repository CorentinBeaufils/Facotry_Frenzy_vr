using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class TestActivateButton : MonoBehaviour
{
    private XRBaseControllerInteractor interactor;
    private ActionBasedController controller;

    void Start()
    {
        interactor = GetComponent<XRBaseControllerInteractor>();
        controller = GetComponent<ActionBasedController>();

        if (controller != null)
        {
            Debug.Log("TestActivateButton: Script démarré sur " + gameObject.name);
            Debug.Log("Activate Action trouvée: " + (controller.activateAction != null));
        }
        else
        {
            Debug.LogError("Pas de ActionBasedController trouvé !");
        }
    }

    void Update()
    {
        // Teste si le contrôleur peut envoyer des activations
        if (interactor != null && interactor.interactablesHovered.Count > 0)
        {
            Debug.Log("Un objet est hovré !");

            // Vérifie si l'action activate existe et est pressée
            if (controller != null && controller.activateAction != null)
            {
                if (controller.activateAction.action != null)
                {
                    float value = controller.activateAction.action.ReadValue<float>();
                    if (value > 0.1f)
                    {
                        Debug.Log("BOUTON ACTIVATE PRESSÉ ! Valeur: " + value);
                    }
                }
                else
                {
                    Debug.LogWarning("L'action activate n'a pas d'action assignée !");
                }
            }
        }
    }
}