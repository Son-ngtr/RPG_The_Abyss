using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance { get; private set; }

    private Vector3 lastPlayerPosition;

    public string lastScenePlayedName;

    private bool dataLoadCompleted = false;

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

    // public void SetLastPlayerPosition(Vector3 position) => lastPlayerPosition = position;


    public void ContinuePlay()
    {
        if (string.IsNullOrEmpty(lastScenePlayedName)) lastScenePlayedName = "Level_0";

        if (SaveManager.instance.GetGameData() == null) lastScenePlayedName = "Level_0";

        ChangeScene(lastScenePlayedName, RespawnType.NonSpecific);
    }


    public void RestartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        ChangeScene(currentSceneName, RespawnType.NonSpecific);
    }

    public void ChangeScene(string sceneName, RespawnType respawnType)
    {
        SaveManager.instance.SaveGame();
        Time.timeScale = 1f; // Cause when open pause menu, time scale = 0
        StartCoroutine(ChangeSceneCo(sceneName, respawnType));
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        // Fade Effect
        UI_FadeScreen fadeScreenUI = FindFadeScreenUI();

        fadeScreenUI.DoFadeOut(1f);
        yield return fadeScreenUI.fadeEffectCo;

        SceneManager.LoadScene(sceneName);

        dataLoadCompleted = false; // data loaded becomes true when you load game from savemanager
        yield return null; // Wait one frame for scene to load, if not the fadein may happen before scene is loaded
        while (dataLoadCompleted == false)
        {
            yield return null;
        }

        fadeScreenUI = FindFadeScreenUI();
        fadeScreenUI.DoFadeIn(1f);

        Player player = Player.instance;
        if (player == null)
        {
            yield break;
        }

        Vector3 position = GetNewPlayerPosition(respawnType);

        if (position != Vector3.zero)
        {
            Player.instance.TeleportPlayer(position);
        }
    }

    private UI_FadeScreen FindFadeScreenUI()
    {
        if (UI.instance == null)
        {
            return FindFirstObjectByType<UI_FadeScreen>();
        }

        return UI.instance.fadeScreenUI;
    }

    // Determine new player position based on respawn type, if None, use last death position or closest checkpoint/enter waypoint
    private Vector3 GetNewPlayerPosition(RespawnType respawnType)
    {
        if (respawnType == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instance;

            Vector3 position = portal.GetPosition();

            portal.SetTrigger(false);
            portal.DisableIfNeed();

            return position;
        }

        if (respawnType == RespawnType.NonSpecific)
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
                return selectedPositions.OrderBy(position => Vector3.Distance(position, lastPlayerPosition)).First();
            }
            else
            {
                // No unlocked checkpoints or enter waypoints found, return last death position
                return lastPlayerPosition;
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

    public void LoadData(GameData data)
    {
        lastScenePlayedName = data.lastScenePlayedName;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayedName)) // First time playing the game Or no saved scene
        {
            lastScenePlayedName = "Level_0";
        }

        dataLoadCompleted = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "MainMenu")
        {
            return;
        }

        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayedName = currentSceneName;

        dataLoadCompleted = false;
    }
}
