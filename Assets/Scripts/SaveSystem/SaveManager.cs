using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; private set; }

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;

    [SerializeField] private string fileName = "doantotnghiep.json";
    [SerializeField] private bool useEncryption = true;

    private void Awake()
    {
        /*if (instance != null)
        {
            Destroy(gameObject);
            return;
        }*/
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // MAKE SURE ALL START METHODS ARE CALLED BEFORE LOADGAME
    private IEnumerator Start()
    {
        // Persistent data path different for each platform,
        // %userprofile%\AppData\LocalLow\<companyname>\<productname> on Windows
        Debug.Log(Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);  
        allSaveables = FindAllSaveables();

        yield return new WaitForSeconds(0.01f);
        LoadGame();
    }

    private void LoadGame()
    {
        gameData = dataHandler.LoadData();

        if (gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            gameData = new GameData();
            return;
        }

        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData); // use ref to modify the same instance
        }

        dataHandler.Save(gameData);
    }

    public GameData GetGameData()
    {
        return gameData;
    }


    [ContextMenu("### Delete Save Data ###")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        dataHandler.DeleteData();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveable> FindAllSaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }
}
