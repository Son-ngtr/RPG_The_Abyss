using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    private Entity entity;

    [Header("On Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVFXDuration = 0.15f;
    private Material originalMaterial;
    private Coroutine onDamageVFXCoroutine;

    [Header("On Dealing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;
    private Coroutine statusVfxCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusVfx(float duration, ElementType element)
    {
        switch(element)
        {
            case ElementType.Fire:
                StartCoroutine(PlayStatusVfxCo(duration, burnVfx));
                break;
            case ElementType.Ice:
                StartCoroutine(PlayStatusVfxCo(duration, chillVfx));
                break;
            case ElementType.Lightning:
                StartCoroutine(PlayStatusVfxCo(duration, shockVfx));
                break;
            default:
                break;
        }
    }

    public void StopAllVfx()
    {
        StopAllCoroutines();

        spriteRenderer.color = Color.white;
        spriteRenderer.material = originalMaterial;
    }

    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.2f;
        float timeHasPassed = 0f;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 7.8f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            spriteRenderer.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);
            timeHasPassed += tickInterval;
        }

        spriteRenderer.color = Color.white;
    }

    public void CreateOnHitVfx(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        if (isCrit == false)
        {
            vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element);
        }

        if (entity.facingDirection == -1 && isCrit)
        {
            vfx.transform.Rotate(0, 180, 0);
        }
    }

    public Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Ice:
                return chillVfx;
            case ElementType.Lightning:
                return shockVfx;
            default:
                return Color.white;
        }
    }

    public void PlayOnDamageVFX()
    {
        if (onDamageVFXCoroutine != null)
        {
            StopCoroutine(onDamageVFXCoroutine);
        }
        onDamageVFXCoroutine = StartCoroutine(OnDamageVfxCoroutine());
    }

    private IEnumerator OnDamageVfxCoroutine()
    {
        spriteRenderer.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVFXDuration);
        spriteRenderer.material = originalMaterial;
    }
}
