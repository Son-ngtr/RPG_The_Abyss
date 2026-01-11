using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private Vector3 lastDeathPosition;

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

    public void SetLastDeathPosition(Vector3 position) => lastDeathPosition = position;

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();

        string currentSceneName = SceneManager.GetActiveScene().name;
        ChangeScene(currentSceneName, RespawnType.None);
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

        Vector3 position = GetNewPlayerPosition(respawnType);

        if (position != Vector3.zero)
        {
            Player.instance.TeleportPlayer(position);
        }
    }

    // Determine new player position based on respawn type, if None, use last death position or closest checkpoint/enter waypoint
    private Vector3 GetNewPlayerPosition(RespawnType respawnType)
    {
        if (respawnType == RespawnType.None)
        {
            var data = SaveManager.instance.GetGameData();
            var checkpoints = FindObjectsByType<Object_CheckPoint>(FindObjectsSortMode.None);
            var unlockedCheckPoints = checkpoints
                .Where(c => data.unlockedCheckPoints.TryGetValue(c.GetCheckPointID(), out bool unlocked) && unlocked)
                .Select(c => c.GetPosition())
                .ToList();

            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                .Select(wp => wp.GetPositionAndSetTriggerFalse())
                .ToList();

            var selectedPositions = unlockedCheckPoints.Concat(enterWaypoints).ToList(); // Combine both lists into one

            if (selectedPositions.Count > 0)
            {
                // Find the closest position to the last death position
                return selectedPositions.OrderBy(position => Vector3.Distance(position, lastDeathPosition)).First();
            }
            else
            {
                // No unlocked checkpoints or enter waypoints found, return last death position
                return lastDeathPosition;
            }
        }

        return GetWaypointPosition(respawnType);
    }

    private Vector3 GetWaypointPosition(RespawnType respawnType)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var waypoint in waypoints)
        {
            if (waypoint.GetWaypointType() == respawnType)
            {
                return waypoint.GetPositionAndSetTriggerFalse();
            }
        }

        return Vector3.zero;
    }
}
