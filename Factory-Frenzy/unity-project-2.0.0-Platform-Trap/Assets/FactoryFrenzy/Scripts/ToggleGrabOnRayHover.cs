using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ToggleGrabOnRayHover : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Référence au bouton A de la manette (Primary Button)")]
    public InputActionProperty primaryButtonAction;

    [Header("Layers")]
    [Tooltip("Layer pour les objets normaux (interactables)")]
    public string interactableLayerName = "Default";
    
    [Tooltip("Layer pour les objets verrouillés (non-grabbable par ray)")]
    public string lockedLayerName = "LockedObjects";

    [Header("Audio (optionnel)")]
    public AudioClip lockSound;
    public AudioClip unlockSound;
    public float volume = 1.0f;

    [Header("Visuel du Ray")]
    [Tooltip("Couleur du ray quand il hover un objet verrouillé")]
    public Color lockedRayColor = Color.red;

    private XRRayInteractor rayInteractor;
    private XRInteractorLineVisual lineVisual;
    private bool wasButtonPressed = false;
    private GameObject currentHoveredObject;
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    private Gradient originalValidGradient;
    private Gradient originalInvalidGradient;

    private int interactableLayer;
    private int lockedLayer;

    void Start()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();

        if (rayInteractor == null)
        {
            Debug.LogError("[ToggleGrabOnRayHover] Pas de XRRayInteractor sur " + gameObject.name);
            enabled = false;
            return;
        }

        if (lineVisual == null)
        {
            Debug.LogWarning("[ToggleGrabOnRayHover] Pas de XRInteractorLineVisual trouvé, la couleur du ray ne changera pas");
        }
        else
        {
            // Sauvegarde les gradients originaux du ray
            originalValidGradient = lineVisual.validColorGradient;
            originalInvalidGradient = lineVisual.invalidColorGradient;
        }

        // Récupère les layers
        interactableLayer = LayerMask.NameToLayer(interactableLayerName);
        lockedLayer = LayerMask.NameToLayer(lockedLayerName);

        if (interactableLayer == -1)
        {
            Debug.LogWarning("[ToggleGrabOnRayHover] Layer '" + interactableLayerName + "' n'existe pas! Utilisation de 'Default'");
            interactableLayer = 0;
        }

        if (lockedLayer == -1)
        {
            Debug.LogError("[ToggleGrabOnRayHover] Layer '" + lockedLayerName + "' n'existe pas!");
            Debug.LogError("Créez le layer: Edit > Project Settings > Tags and Layers");
            enabled = false;
            return;
        }

        // Abonne aux événements
        rayInteractor.hoverEntered.AddListener(OnHoverEntered);
        rayInteractor.hoverExited.AddListener(OnHoverExited);

        if (primaryButtonAction.action != null)
        {
            primaryButtonAction.action.Enable();
            Debug.Log("[ToggleGrabOnRayHover] Bouton A configuré");
        }
        else
        {
            Debug.LogWarning("[ToggleGrabOnRayHover] Aucune action assignée pour le bouton A");
        }

        Debug.Log("[ToggleGrabOnRayHover] Layer interactable: " + LayerMask.LayerToName(interactableLayer));
        Debug.Log("[ToggleGrabOnRayHover] Layer locked: " + LayerMask.LayerToName(lockedLayer));
    }

    void Update()
    {
        if (primaryButtonAction.action == null) return;

        // Détecte l'appui sur le bouton A
        bool isButtonPressed = primaryButtonAction.action.ReadValue<float>() > 0.5f;

        if (isButtonPressed && !wasButtonPressed)
        {
            OnButtonAPressed();
        }

        wasButtonPressed = isButtonPressed;
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        GameObject hoveredObject = args.interactableObject.transform.gameObject;
        currentHoveredObject = hoveredObject;

        bool isLocked = hoveredObject.layer == lockedLayer;
        
        if (isLocked)
        {
            // DÉSACTIVE le grab du ray quand on hover un objet verrouillé
            rayInteractor.allowSelect = false;
            
            // CHANGE LA COULEUR DU RAY en rouge UNIQUEMENT pour les objets verrouillés
            SetRayColor(lockedRayColor);
            
            Debug.Log("[ToggleGrabOnRayHover] ⛔ Hover sur objet VERROUILLÉ: " + hoveredObject.name + " - Grab désactivé, ray rouge");
        }
        else
        {
            // Pour les objets normaux, on ne change PAS la couleur (garde le comportement normal du XR Toolkit)
            Debug.Log("[ToggleGrabOnRayHover] Hover sur objet déverrouillé: " + hoveredObject.name);
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        GameObject hoveredObject = args.interactableObject.transform.gameObject;

        bool wasLocked = hoveredObject.layer == lockedLayer;
        
        if (wasLocked)
        {
            // RÉACTIVE le grab du ray quand on quitte un objet verrouillé
            rayInteractor.allowSelect = true;
            
            // RESTAURE LES GRADIENTS ORIGINAUX DU RAY
            RestoreOriginalRayColor();
            
            Debug.Log("[ToggleGrabOnRayHover] Hover quitté (objet verrouillé): " + hoveredObject.name + " - Grab réactivé, ray normal");
        }

        if (currentHoveredObject == hoveredObject)
        {
            currentHoveredObject = null;
        }
    }

    private void OnButtonAPressed()
    {
        if (currentHoveredObject != null)
        {
            // Sauvegarde le layer original si pas déjà fait
            if (!originalLayers.ContainsKey(currentHoveredObject))
            {
                originalLayers[currentHoveredObject] = currentHoveredObject.layer;
            }

            // Toggle entre locked et unlocked
            if (currentHoveredObject.layer == lockedLayer)
            {
                // DÉVERROUILLAGE
                int originalLayer = originalLayers[currentHoveredObject];
                SetLayerRecursively(currentHoveredObject, originalLayer);
                
                // Réactive immédiatement le grab car l'objet n'est plus locked
                rayInteractor.allowSelect = true;
                
                // RESTAURE LES GRADIENTS ORIGINAUX DU RAY
                RestoreOriginalRayColor();
                
                Debug.Log("[ToggleGrabOnRayHover] ✓ " + currentHoveredObject.name + " DÉVERROUILLÉ");
                
                if (unlockSound != null)
                {
                    AudioSource.PlayClipAtPoint(unlockSound, currentHoveredObject.transform.position, volume);
                }
            }
            else
            {
                // VERROUILLAGE
                SetLayerRecursively(currentHoveredObject, lockedLayer);
                
                // Désactive immédiatement le grab car l'objet est maintenant locked
                rayInteractor.allowSelect = false;
                
                // CHANGE LA COULEUR DU RAY en rouge
                SetRayColor(lockedRayColor);
                
                Debug.Log("[ToggleGrabOnRayHover] ✕ " + currentHoveredObject.name + " VERROUILLÉ");
                
                if (lockSound != null)
                {
                    AudioSource.PlayClipAtPoint(lockSound, currentHoveredObject.transform.position, volume);
                }
            }
        }
        else
        {
            Debug.Log("[ToggleGrabOnRayHover] Aucun objet pointé");
        }
    }

    private void SetRayColor(Color color)
    {
        if (lineVisual != null)
        {
            lineVisual.invalidColorGradient = CreateGradient(color);
            lineVisual.validColorGradient = CreateGradient(color);
        }
    }

    private void RestoreOriginalRayColor()
    {
        if (lineVisual != null && originalValidGradient != null && originalInvalidGradient != null)
        {
            lineVisual.validColorGradient = originalValidGradient;
            lineVisual.invalidColorGradient = originalInvalidGradient;
        }
    }

    private Gradient CreateGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        return gradient;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void OnDestroy()
    {
        if (primaryButtonAction.action != null)
        {
            primaryButtonAction.action.Disable();
        }

        if (rayInteractor != null)
        {
            rayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            rayInteractor.hoverExited.RemoveListener(OnHoverExited);
            
            // Assure que le grab est réactivé à la destruction
            rayInteractor.allowSelect = true;
        }

        // Restaure les gradients originaux du ray
        RestoreOriginalRayColor();

        // Restaure les layers originaux
        foreach (var kvp in originalLayers)
        {
            if (kvp.Key != null)
            {
                SetLayerRecursively(kvp.Key, kvp.Value);
            }
        }
    }
}