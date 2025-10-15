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
            Debug.Log("TestActivateButton: Script d�marr� sur " + gameObject.name);
            Debug.Log("Activate Action trouv�e: " + (controller.activateAction != null));
        }
        else
        {
            Debug.LogError("Pas de ActionBasedController trouv� !");
        }
    }

    void Update()
    {
        // Teste si le contr�leur peut envoyer des activations
        if (interactor != null && interactor.interactablesHovered.Count > 0)
        {
            Debug.Log("Un objet est hovr� !");

            // V�rifie si l'action activate existe et est press�e
            if (controller != null && controller.activateAction != null)
            {
                if (controller.activateAction.action != null)
                {
                    float value = controller.activateAction.action.ReadValue<float>();
                    if (value > 0.1f)
                    {
                        Debug.Log("BOUTON ACTIVATE PRESS� ! Valeur: " + value);
                    }
                }
                else
                {
                    Debug.LogWarning("L'action activate n'a pas d'action assign�e !");
                }
            }
        }
    }
}