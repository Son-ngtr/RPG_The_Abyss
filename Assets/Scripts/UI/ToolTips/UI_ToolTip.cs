using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(300f, 20f);
    [SerializeField] private float edgeMargin = 8f; // khoảng cách tối thiểu với mép màn hình

    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera uiCamera;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas != null ? canvas.worldCamera : null;
    }

    // Hiển thị/ẩn tooltip; khi hiển thị sẽ cập nhật vị trí theo target
    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (!show)
        {
            // Ẩn sạch sẽ, tránh đẩy ra (9999,9999)
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);
        UpdatePosition(targetRect);
    }

    // Cập nhật vị trí tooltip tương đối với target, có tính scale theo CanvasScaler
    public void UpdatePosition(RectTransform targetRect)
    {
        if (targetRect == null) return;

        // Tính vị trí target ở screen space
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetRect.position);

        // Lấy scale từ Canvas (CanvasScaler). Nếu không có Canvas, dùng 1f.
        float scale = canvas != null ? canvas.scaleFactor : 1f;
        Vector2 scaledOffset = offset * scale;

        // Quyết định đặt tooltip bên trái hoặc bên phải target
        float screenCenterX = Screen.width * 0.5f;
        screenPos.x = screenPos.x > screenCenterX
            ? screenPos.x - scaledOffset.x
            : screenPos.x + scaledOffset.x;

        // Kích thước tooltip theo screen (nhân scale)
        float halfWidth = (rectTransform.rect.width * scale) * 0.5f;
        float halfHeight = (rectTransform.rect.height * scale) * 0.5f;

        // Clamp ngang
        float leftX = screenPos.x - halfWidth;
        float rightX = screenPos.x + halfWidth;
        if (rightX > Screen.width)
        {
            screenPos.x = Screen.width - halfWidth - edgeMargin;
        }
        else if (leftX < 0f)
        {
            screenPos.x = halfWidth + edgeMargin;
        }

        // Clamp dọc: khi vượt top/bottom, tôn trọng offset.y để tạo khoảng cách
        float topY = screenPos.y + halfHeight;
        float bottomY = screenPos.y - halfHeight;
        if (topY > Screen.height)
        {
            screenPos.y = Screen.height - halfHeight - scaledOffset.y;
        }
        else if (bottomY < 0f)
        {
            screenPos.y = halfHeight + scaledOffset.y;
        }

        // Chuyển về world space trên cùng plane của tooltip
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, uiCamera, out var worldPos))
        {
            rectTransform.position = worldPos;
        }
    }

    protected string GetColoredText(string colorHex, string text)
    {
        return $"<color={colorHex}>{text}</color>";
    }
}