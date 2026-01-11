using UnityEngine;

public class Object_CheckPoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointID;
    [SerializeField] private Transform respawnPoint;

    public bool isActive { get; private set; }
    private Animator animator;
    private AudioSource audioSource;

    private void OnValidate()
    {
//#if UNITYEDITOR
        if (string.IsNullOrEmpty(checkpointID))
        {
            checkpointID = System.Guid.NewGuid().ToString();
        }
//#endif
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public string GetCheckPointID() => checkpointID;

    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;


    public void ActiveCheckPoint(bool activate)
    {
        isActive = activate;
        animator.SetBool("isActive", activate);

        if (isActive && audioSource.isPlaying == false)
        {
            audioSource.Play();
        }

        if (isActive == false)
        {
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActiveCheckPoint(true);
    }

    public void LoadData(GameData data)
    {
        bool active = data.unlockedCheckPoints.ContainsKey(checkpointID) && data.unlockedCheckPoints[checkpointID];

        ActiveCheckPoint(active);
    }

    public void SaveData(ref GameData data)
    {
        if (isActive == false)
        {
            return;
        }

        if (data.unlockedCheckPoints.ContainsKey(checkpointID) == false)
        {
            data.unlockedCheckPoints.Add(checkpointID, true);
        }
    }
}
