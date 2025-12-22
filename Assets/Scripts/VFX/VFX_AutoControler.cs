using System.Collections;
using UnityEngine;

public class VFX_AutoControler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;

    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;

    [Header("Fade effect")]
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float maxRotation = 360f;

    [Header("Random Position")]
    [SerializeField] private float xMinOffset = -0.3f;
    [SerializeField] private float xMaxOffset = 0.3f;
    [Space]
    [SerializeField] private float yMinOffset = -0.3f;
    [SerializeField] private float yMaxOffset = 0.3f;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (canFade)
        {
            StartCoroutine(FadeCo());
        }

        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;

        while (targetColor.a > 0)
        {
            targetColor.a = targetColor.a - fadeSpeed * Time.deltaTime;
            spriteRenderer.color = targetColor;
            
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    private void ApplyRandomOffset()
    {
        if (randomOffset == false)
        {
            return;
        }

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position = transform.position + new Vector3(xOffset, yOffset);
    }

    private void ApplyRandomRotation()
    {
        if (randomRotation == false)
        {
            return;
        }

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}
