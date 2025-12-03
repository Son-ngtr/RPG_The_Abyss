using UnityEngine;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationPoint;
    [SerializeField] private RectTransform connectionLength;
    [SerializeField] private RectTransform childNodeConnectionPoint;

    public void DirectConnection(NodeConnectionType direction, float length)
    {
        bool shouldBeActive = direction != NodeConnectionType.None;

        float finalLength = shouldBeActive ? length : 0;
        float angle = GetDirectionAngle(direction);

        rotationPoint.localRotation = Quaternion.Euler(0, 0, angle);
        connectionLength.sizeDelta = new Vector2(finalLength, rotationPoint.sizeDelta.y);
    }

    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
            rect.parent as RectTransform, 
            childNodeConnectionPoint.position, 
            null, 
            out var localPosition
            );

        return localPosition;
    }

    private float GetDirectionAngle(NodeConnectionType direction)
    {
        return direction switch
        {
            NodeConnectionType.UpLeft => 135f,
            NodeConnectionType.Up => 90f,
            NodeConnectionType.UpRight => 45f,
            NodeConnectionType.Left => 180f,
            NodeConnectionType.Right => 0f,
            NodeConnectionType.DownLeft => -135f,
            NodeConnectionType.Down => -90f,
            NodeConnectionType.DownRight => -45f,
            _ => 0f,
        };
    }
}

public enum NodeConnectionType
{
    None, 
    UpLeft,
    Up,
    UpRight,
    Left,
    Right,
    DownLeft,
    Down,
    DownRight
}
