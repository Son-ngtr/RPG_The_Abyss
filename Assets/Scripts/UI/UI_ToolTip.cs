using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Vector2 offset = new Vector2(300, 20);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (show == false)
        {
            rectTransform.position = new Vector2(9999, 9999);
            return;
        }

        UpdatePosition(targetRect);
    }

    public void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0f;

        Vector2 targetPosition = targetRect.position;

        targetPosition.x = targetPosition.x > screenCenterX
            ? targetPosition.x - offset.x
            : targetPosition.x + offset.x;

        float verticalHalf = rectTransform.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        if (topY > screenTop)
        {
            targetPosition.y = screenTop - verticalHalf - offset.y;
        }
        else if (bottomY < screenBottom)
        {
            targetPosition.y = screenBottom + verticalHalf + offset.y;
        }

        rectTransform.position = targetPosition;
    }
}
