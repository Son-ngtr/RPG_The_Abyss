using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToSceneName;

    [Space]
    [SerializeField] private RespawnType waypointType;

    [SerializeField] private RespawnType connnectedWaypointType; // if Enter, set to Exit and vice versa

    [SerializeField] private Transform respawnPoint;

    [SerializeField] private bool canBeTriggered = true;


    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + waypointType.ToString() + " - " + transferToSceneName;

        if (waypointType == RespawnType.Enter)
        {
            connnectedWaypointType = RespawnType.Exit;
        }

        if (waypointType == RespawnType.Exit)
        {
            connnectedWaypointType = RespawnType.Enter;
        }
    }

    public void SetCanBeTrigger(bool canBeTriggered)
    {
        this.canBeTriggered = canBeTriggered;
    }

    public RespawnType GetWaypointType()
    {
        return waypointType;
    }

    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;
        if (respawnPoint != null)
        {
            return respawnPoint.position;
        }
        else
        {
            return transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false) return;

        // Save manager - save game
        SaveManager.instance.SaveGame();


        // Game manager - transfer scene
        GameManager.instance.ChangeScene(transferToSceneName, connnectedWaypointType);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }

}
