using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float imageWidthOffset = 10; 

    private float imageFullWidth;
    private float halfWidth;

    public void Move(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageLeftEdge = (background.position.x - halfWidth) + imageWidthOffset;
        float imageRightEdge = (background.position.x + halfWidth) - imageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth;
        }
        else if (imageLeftEdge > cameraRightEdge)
        {
            background.position -= Vector3.right * imageFullWidth;
        }
    }

    public void CalculateImageWidth()
    {
        SpriteRenderer spriteRenderer = background.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            imageFullWidth = spriteRenderer.bounds.size.x;
            halfWidth = imageFullWidth / 2f;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on background object.");
        }
    }
}
