using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encryptData;
    private string codeword = "nguyenxuanson";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encryptData = encryptData;
    }

    public void Save(GameData gameData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); // Ensure directory exists // Create directory if it doesn't exist

            string dataToSave = JsonUtility.ToJson(gameData, true); // Convert GameData to JSON format

            if (encryptData)
            {
                dataToSave = EncryptDecrypt(dataToSave);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) // Create or overwrite the file
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToSave); // Write JSON string to file
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data to file: " + fullPath + "\n" + e.Message);
        }
    }

    public GameData LoadData()
    {
        GameData loadData = null;

        // Check if the file exists before attempting to read
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = ""; // Variable to hold the file content

                using (FileStream stream = new FileStream(fullPath, FileMode.Open)) // Open the file for reading
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); // Read the entire file content
                    }
                }

                if (encryptData)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad); // Convert JSON string back to GameData object
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data from file: " + fullPath + "\n" + e.Message);
            }
        }

        return loadData;
    }

    public void DeleteData()
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath); // Delete the file if it exists
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting data file: " + fullPath + "\n" + e.Message);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeword[i % codeword.Length]); // XOR operation for simple encryption/decryption
        }
        return modifiedData;
    }
}
