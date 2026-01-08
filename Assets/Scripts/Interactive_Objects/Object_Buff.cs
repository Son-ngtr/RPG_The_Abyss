using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Object_Buff : MonoBehaviour
{
    private Player_Stats statsToModify;

    [Header("Buff Settings")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private float buffDuration = 4f;

    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = 0.25f;
    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffSet = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffSet, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        statsToModify = collision.GetComponent<Player_Stats>();

        if (statsToModify.CanApplyBuff(buffName))
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }

    }
}
