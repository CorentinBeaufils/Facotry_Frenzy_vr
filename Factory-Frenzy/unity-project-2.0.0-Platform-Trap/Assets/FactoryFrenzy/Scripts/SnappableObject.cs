using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(XRGrabInteractable))]
public class SnappableObject : MonoBehaviour
{
    [Header("Configuration Snap")]
    [Tooltip("Distance maximale pour détecter un snap point")]
    public float snapDistance = 0.2f;

    [Tooltip("Délai après le relâchement avant de tenter le snap")]
    public float snapDelay = 0.2f;

    [Tooltip("Vitesse du snap (0 = instantané)")]
    public float snapSpeed = 10f;

    [Header("Debug Visuel")]
    public bool showDebugSphere = true;
    public Color debugColor = Color.cyan;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private SnapPoint currentSnapPoint;
    private SnapPoint mySnapPoint;
    private bool isSnapping = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            Debug.Log("[SnappableObject] " + gameObject.name + " initialisé avec XRGrabInteractable");
        }
        else
        {
            Debug.LogError("[SnappableObject] Pas de XRGrabInteractable sur " + gameObject.name);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("[SnappableObject] " + gameObject.name + " a été attrapé");

        if (currentSnapPoint != null)
        {
            Debug.Log("[SnappableObject] Libération du SnapPoint: " + currentSnapPoint.gameObject.name);
            currentSnapPoint.Release();
            currentSnapPoint = null;
            mySnapPoint = null;
        }

        isSnapping = false;
        StopAllCoroutines();

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("[SnappableObject] " + gameObject.name + " a été relâché, délai de " + snapDelay + "s avant snap");
        StartCoroutine(DelayedSnapCheck());
    }

    private IEnumerator DelayedSnapCheck()
    {
        yield return new WaitForSeconds(snapDelay);

        Debug.Log("[SnappableObject] Vérification du snap pour " + gameObject.name);
        TrySnap();
    }

    private void TrySnap()
    {
        SnapPointPair snapPair = FindNearestSnapPointPair();

        if (snapPair.targetSnapPoint != null && snapPair.mySnapPoint != null)
        {
            if (snapPair.targetSnapPoint.CanAccept(gameObject))
            {
                Debug.Log("[SnappableObject] Snap détecté: " + snapPair.mySnapPoint.gameObject.name + " -> " + snapPair.targetSnapPoint.gameObject.name);
                StartCoroutine(SnapToPoint(snapPair.targetSnapPoint, snapPair.mySnapPoint));
            }
            else
            {
                Debug.LogWarning("[SnappableObject] SnapPoint " + snapPair.targetSnapPoint.gameObject.name + " ne peut pas accepter " + gameObject.name);
            }
        }
        else
        {
            Debug.Log("[SnappableObject] Aucune paire de SnapPoints trouvée dans un rayon de " + snapDistance + "m");
        }
    }

    private struct SnapPointPair
    {
        public SnapPoint mySnapPoint;
        public SnapPoint targetSnapPoint;
        public float distance;
    }

    private SnapDirection GetOppositeDirection(SnapDirection direction)
    {
        switch (direction)
        {
            case SnapDirection.Top: return SnapDirection.Bottom;
            case SnapDirection.Bottom: return SnapDirection.Top;
            case SnapDirection.Left: return SnapDirection.Right;
            case SnapDirection.Right: return SnapDirection.Left;
            case SnapDirection.Front: return SnapDirection.Back;
            case SnapDirection.Back: return SnapDirection.Front;
            case SnapDirection.Center: return SnapDirection.Center;
            default: return direction;
        }
    }

    private SnapPoint FindOppositeSnapPointOnMyObject(SnapPoint myDetectedSnapPoint, SnapDirection oppositeDirection)
    {
        // Chercher tous MES SnapPoints
        SnapPoint[] mySnapPoints = GetComponentsInChildren<SnapPoint>();

        foreach (SnapPoint snap in mySnapPoints)
        {
            if (snap.direction == oppositeDirection)
            {
                Debug.Log("[SnappableObject] Mon SnapPoint opposé trouvé: " + snap.gameObject.name + " (direction: " + oppositeDirection + ")");
                return snap;
            }
        }

        Debug.LogWarning("[SnappableObject] Aucun SnapPoint opposé trouvé pour la direction " + oppositeDirection + " sur mon objet");
        return null;
    }

    private SnapPointPair FindNearestSnapPointPair()
    {
        SnapPoint[] allSnapPoints = Object.FindObjectsOfType<SnapPoint>();
        SnapPoint[] mySnapPoints = GetComponentsInChildren<SnapPoint>();

        Debug.Log("[SnappableObject] " + allSnapPoints.Length + " SnapPoints trouvés dans la scène");
        Debug.Log("[SnappableObject] " + mySnapPoints.Length + " SnapPoints sur cet objet");

        SnapPointPair bestPair = new SnapPointPair();
        bestPair.distance = float.MaxValue;

        foreach (SnapPoint mySnap in mySnapPoints)
        {
            foreach (SnapPoint targetSnap in allSnapPoints)
            {
                // Ignorer les SnapPoints qui sont enfants de cet objet
                if (targetSnap.transform.IsChildOf(transform))
                {
                    continue;
                }

                float distance = Vector3.Distance(mySnap.transform.position, targetSnap.transform.position);

                Debug.Log("[SnappableObject] Distance: " + mySnap.gameObject.name + " (" + mySnap.direction + ") -> " + targetSnap.gameObject.name + " (" + targetSnap.direction + ") = " + distance.ToString("F3") + "m");

                if (distance <= snapDistance && targetSnap.CanAccept(gameObject))
                {
                    SnapPoint finalMySnap = mySnap;

                    // Si même direction, utiliser MON SnapPoint opposé
                    if (mySnap.direction == targetSnap.direction)
                    {
                        Debug.Log("[SnappableObject] Même direction détectée (" + mySnap.direction + "), recherche de MON SnapPoint opposé...");
                        SnapDirection oppositeDir = GetOppositeDirection(mySnap.direction);
                        SnapPoint myOppositeSnap = FindOppositeSnapPointOnMyObject(mySnap, oppositeDir);

                        if (myOppositeSnap != null)
                        {
                            finalMySnap = myOppositeSnap;
                            Debug.Log("[SnappableObject] Utilisation de mon SnapPoint opposé: " + finalMySnap.gameObject.name + " pour se fixer sur " + targetSnap.gameObject.name);
                        }
                        else
                        {
                            Debug.LogWarning("[SnappableObject] Mon SnapPoint opposé non trouvé, skip");
                            continue;
                        }
                    }

                    if (distance < bestPair.distance)
                    {
                        bestPair.distance = distance;
                        bestPair.mySnapPoint = finalMySnap;
                        bestPair.targetSnapPoint = targetSnap;
                    }
                }
            }
        }

        if (bestPair.targetSnapPoint != null)
        {
            Debug.Log("[SnappableObject] Meilleure paire: " + bestPair.mySnapPoint.gameObject.name + " (" + bestPair.mySnapPoint.direction + ") -> " + bestPair.targetSnapPoint.gameObject.name + " (" + bestPair.targetSnapPoint.direction + ") à " + bestPair.distance.ToString("F3") + "m");
        }

        return bestPair;
    }

    private bool NeedsPerpendicularRotation(SnapDirection myDirection, SnapDirection targetDirection)
    {
        // Cas 1: Front/Back sur Top/Bottom
        if ((myDirection == SnapDirection.Front || myDirection == SnapDirection.Back) &&
            (targetDirection == SnapDirection.Top || targetDirection == SnapDirection.Bottom))
        {
            return true;
        }

        // Cas 2: Top/Bottom sur Front/Back
        if ((myDirection == SnapDirection.Top || myDirection == SnapDirection.Bottom) &&
            (targetDirection == SnapDirection.Front || targetDirection == SnapDirection.Back))
        {
            return true;
        }

        // Cas 3: Left/Right sur Top/Bottom
        if ((myDirection == SnapDirection.Left || myDirection == SnapDirection.Right) &&
            (targetDirection == SnapDirection.Top || targetDirection == SnapDirection.Bottom))
        {
            return true;
        }

        // Cas 4: Top/Bottom sur Left/Right
        if ((myDirection == SnapDirection.Top || myDirection == SnapDirection.Bottom) &&
            (targetDirection == SnapDirection.Left || targetDirection == SnapDirection.Right))
        {
            return true;
        }

        // Cas 5: Front/Back sur Left/Right
        if ((myDirection == SnapDirection.Front || myDirection == SnapDirection.Back) &&
            (targetDirection == SnapDirection.Left || targetDirection == SnapDirection.Right))
        {
            return true;
        }

        // Cas 6: Left/Right sur Front/Back
        if ((myDirection == SnapDirection.Left || myDirection == SnapDirection.Right) &&
            (targetDirection == SnapDirection.Front || targetDirection == SnapDirection.Back))
        {
            return true;
        }

        return false;
    }

    private Quaternion CalculatePerpendicularRotation(SnapDirection myDirection, SnapDirection targetDirection, SnapPoint targetSnapPoint)
    {
        // Front/Back → Top/Bottom : rotation autour de Right (X)
        if ((myDirection == SnapDirection.Front || myDirection == SnapDirection.Back) &&
            (targetDirection == SnapDirection.Top || targetDirection == SnapDirection.Bottom))
        {
            if (targetDirection == SnapDirection.Top)
            {
                if (myDirection == SnapDirection.Front)
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.right);
                }
                else // Back
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.right);
                }
            }
            else // Bottom
            {
                if (myDirection == SnapDirection.Front)
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.right);
                }
                else // Back
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.right);
                }
            }
        }

        // Top/Bottom → Front/Back : rotation autour de Right (X)
        if ((myDirection == SnapDirection.Top || myDirection == SnapDirection.Bottom) &&
            (targetDirection == SnapDirection.Front || targetDirection == SnapDirection.Back))
        {
            if (myDirection == SnapDirection.Bottom)
            {
                return Quaternion.AngleAxis(90f, targetSnapPoint.transform.right);
            }
            else // Top
            {
                return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.right);
            }
        }

        // Left/Right → Top/Bottom : rotation autour de Forward (Z)
        if ((myDirection == SnapDirection.Left || myDirection == SnapDirection.Right) &&
            (targetDirection == SnapDirection.Top || targetDirection == SnapDirection.Bottom))
        {
            if (targetDirection == SnapDirection.Top)
            {
                if (myDirection == SnapDirection.Right)
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.forward);
                }
                else // Left
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.forward);
                }
            }
            else // Bottom
            {
                if (myDirection == SnapDirection.Right)
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.forward);
                }
                else // Left
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.forward);
                }
            }
        }

        // Top/Bottom → Left/Right : rotation autour de Forward (Z)
        if ((myDirection == SnapDirection.Top || myDirection == SnapDirection.Bottom) &&
            (targetDirection == SnapDirection.Left || targetDirection == SnapDirection.Right))
        {
            if (targetDirection == SnapDirection.Right)
            {
                if (myDirection == SnapDirection.Bottom)
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.forward);
                }
                else // Top
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.forward);
                }
            }
            else // Left
            {
                if (myDirection == SnapDirection.Bottom)
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.forward);
                }
                else // Top
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.forward);
                }
            }
        }

        // Front/Back → Left/Right : rotation autour de Up (Y)
        if ((myDirection == SnapDirection.Front || myDirection == SnapDirection.Back) &&
            (targetDirection == SnapDirection.Left || targetDirection == SnapDirection.Right))
        {
            if (targetDirection == SnapDirection.Right)
            {
                if (myDirection == SnapDirection.Front)
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.up);
                }
                else // Back
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.up);
                }
            }
            else // Left
            {
                if (myDirection == SnapDirection.Front)
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.up);
                }
                else // Back
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.up);
                }
            }
        }

        // Left/Right → Front/Back : rotation autour de Up (Y)
        if ((myDirection == SnapDirection.Left || myDirection == SnapDirection.Right) &&
            (targetDirection == SnapDirection.Front || targetDirection == SnapDirection.Back))
        {
            if (targetDirection == SnapDirection.Front)
            {
                if (myDirection == SnapDirection.Right)
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.up);
                }
                else // Left
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.up);
                }
            }
            else // Back
            {
                if (myDirection == SnapDirection.Right)
                {
                    return Quaternion.AngleAxis(90f, targetSnapPoint.transform.up);
                }
                else // Left
                {
                    return Quaternion.AngleAxis(-90f, targetSnapPoint.transform.up);
                }
            }
        }

        return Quaternion.identity;
    }

    private IEnumerator SnapToPoint(SnapPoint targetSnapPoint, SnapPoint mySnapPoint)
    {
        Debug.Log("[SnappableObject] Début du snap: aligner " + mySnapPoint.gameObject.name + " avec " + targetSnapPoint.gameObject.name);
        Debug.Log("[SnappableObject] Directions: Mon SnapPoint = " + mySnapPoint.direction + ", SnapPoint cible = " + targetSnapPoint.direction);

        isSnapping = true;
        currentSnapPoint = targetSnapPoint;
        this.mySnapPoint = mySnapPoint;
        currentSnapPoint.Occupy(gameObject);

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // ÉTAPE 1: Calculer la rotation de base
        Quaternion rotationDelta;

        // CAS 1: Directions perpendiculaires (toutes les combinaisons)
        if (NeedsPerpendicularRotation(mySnapPoint.direction, targetSnapPoint.direction))
        {
            Debug.Log("[SnappableObject] Rotation perpendiculaire détectée");

            rotationDelta = targetSnapPoint.transform.rotation * Quaternion.Inverse(mySnapPoint.transform.rotation);
            Quaternion perpendicularRotation = CalculatePerpendicularRotation(mySnapPoint.direction, targetSnapPoint.direction, targetSnapPoint);
            rotationDelta = perpendicularRotation * rotationDelta;

            Debug.Log("[SnappableObject] Rotation perpendiculaire: " + perpendicularRotation.eulerAngles);
        }
        // CAS 2: Directions opposées (Top <-> Bottom, Front <-> Back, etc.) - Alignement normal
        else
        {
            Debug.Log("[SnappableObject] Alignement normal (directions opposées ou compatibles)");
            rotationDelta = targetSnapPoint.transform.rotation * Quaternion.Inverse(mySnapPoint.transform.rotation);
        }

        Quaternion targetRotation = rotationDelta * transform.rotation;

        Debug.Log("[SnappableObject] Rotation calculée - Delta: " + rotationDelta.eulerAngles);

        // ÉTAPE 2: Appliquer temporairement la rotation pour calculer le nouvel offset de position
        Quaternion originalRotation = transform.rotation;
        transform.rotation = targetRotation;

        // ÉTAPE 3: Calculer l'offset de position APRÈS rotation
        Vector3 offsetPosition = mySnapPoint.transform.position - transform.position;
        Vector3 targetPosition = targetSnapPoint.transform.position - offsetPosition;

        // Restaurer la rotation originale pour l'animation
        transform.rotation = originalRotation;

        Debug.Log("[SnappableObject] Position calculée - Offset: " + offsetPosition + ", Cible: " + targetPosition);

        if (snapSpeed <= 0)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            Debug.Log("[SnappableObject] Snap instantané terminé");
        }
        else
        {
            float startTime = Time.time;
            while (Vector3.Distance(transform.position, targetPosition) > 0.001f || Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * snapSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * snapSpeed);
                yield return null;
            }

            transform.position = targetPosition;
            transform.rotation = targetRotation;
            Debug.Log("[SnappableObject] Snap animé terminé en " + (Time.time - startTime).ToString("F2") + "s");
        }

        // Vérification finale
        float finalDistance = Vector3.Distance(mySnapPoint.transform.position, targetSnapPoint.transform.position);
        float finalAngle = Quaternion.Angle(mySnapPoint.transform.rotation, targetSnapPoint.transform.rotation);
        Debug.Log("[SnappableObject] Distance finale: " + finalDistance.ToString("F6") + "m, Angle: " + finalAngle.ToString("F2") + "°");

        isSnapping = false;
    }

    private void OnDrawGizmos()
    {
        if (showDebugSphere)
        {
            // Dessiner une sphère autour de chaque SnapPoint enfant
            SnapPoint[] mySnapPoints = GetComponentsInChildren<SnapPoint>();
            foreach (SnapPoint snap in mySnapPoints)
            {
                Gizmos.color = debugColor;
                Gizmos.DrawWireSphere(snap.transform.position, snapDistance);
            }
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }

        if (currentSnapPoint != null)
        {
            currentSnapPoint.Release();
        }
    }
}