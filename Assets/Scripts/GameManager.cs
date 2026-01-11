using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // There is already an instance of GameManager, destroy new one
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string sceneName, RespawnType respawnType)
    {
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        // Fade Effect

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(0.2f);

        Vector3 position = GetWaypointPosition(respawnType);
        if (position != Vector3.zero)
        {
            Player.instance.TeleportPlayer(position);
        }
    }

    private Vector3 GetWaypointPosition(RespawnType respawnType)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var waypoint in waypoints)
        {
            if (waypoint.GetWaypointType() == respawnType)
            {
                waypoint.SetCanBeTrigger(false);
                return waypoint.GetPosition();
            }
        }

        return Vector3.zero;
    }
}
