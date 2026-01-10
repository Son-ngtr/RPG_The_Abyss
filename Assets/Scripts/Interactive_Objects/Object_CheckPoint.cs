using UnityEngine;

public class Object_CheckPoint : MonoBehaviour, ISaveable
{
    private Object_CheckPoint[] allCheckPoints;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        allCheckPoints = FindObjectsByType<Object_CheckPoint>(FindObjectsSortMode.None);
    }


    public void ActiveCheckPoint(bool activate)
    {
        animator.SetBool("isActive", activate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var point in allCheckPoints)
        {
            point.ActiveCheckPoint(false);
        }

        // Save position to save manager
        SaveManager.instance.GetGameData().savedCheckPoint = transform.position;
        ActiveCheckPoint(true);
    }

    public void LoadData(GameData data)
    {
        bool active = data.savedCheckPoint == transform.position;

        ActiveCheckPoint(active);
        Player.instance.TeleportPlayer(transform.position);
    }

    public void SaveData(ref GameData data)
    {
    }
}
