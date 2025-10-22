using UnityEngine;

public enum SnapDirection
{
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back,
    Center
}

public class SnapPoint : MonoBehaviour
{
    [Header("Configuration")]
    public SnapDirection direction;
    public bool isOccupied = false;
    public GameObject occupiedBy;

    [Header("Filtrage")]
    [Tooltip("Tags acceptés pour ce point de snap. Vide = tous acceptés")]
    public string[] acceptedTags;

    [Header("Visualisation")]
    public Color gizmoColorFree = Color.green;
    public Color gizmoColorOccupied = Color.red;
    public float gizmoSize = 0.05f;

    public bool CanAccept(GameObject obj)
    {
        if (isOccupied) return false;

        if (acceptedTags == null || acceptedTags.Length == 0) return true;

        foreach (string tag in acceptedTags)
        {
            if (obj.CompareTag(tag)) return true;
        }

        return false;
    }

    public void Occupy(GameObject obj)
    {
        isOccupied = true;
        occupiedBy = obj;
    }

    public void Release()
    {
        isOccupied = false;
        occupiedBy = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? gizmoColorOccupied : gizmoColorFree;
        Gizmos.DrawSphere(transform.position, gizmoSize);

        Gizmos.color = Color.yellow;
        Vector3 directionVector = GetDirectionVector();
        Gizmos.DrawLine(transform.position, transform.position + directionVector * 0.1f);
    }

    private Vector3 GetDirectionVector()
    {
        switch (direction)
        {
            case SnapDirection.Top: return Vector3.up;
            case SnapDirection.Bottom: return Vector3.down;
            case SnapDirection.Left: return Vector3.left;
            case SnapDirection.Right: return Vector3.right;
            case SnapDirection.Front: return Vector3.forward;
            case SnapDirection.Back: return Vector3.back;
            default: return Vector3.zero;
        }
    }
}