using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// used info from https://www.youtube.com/watch?v=wJhMaLkeEuY for initial skeleton

public class DataManager : MonoBehaviour
{

    public Transform playerTransform;


    // save game function
    public void SaveGame()
    {
        // new instance of class
        DataStorage dataStorage = new DataStorage();
        
        // new float array
        dataStorage.position = new float[] { playerTransform.position.x, playerTransform.position.y };

        // datastorage to json string
        string json = JsonUtility.ToJson(dataStorage);

        // builds a file path for json string
        string path = Application.persistentDataPath + "/saveData.json";

        // writes json string into the file
        System.IO.File.WriteAllText(path, json);
    }


    // load game function 
    public void LoadGame()
    {
        // same file path
        string path = Application.persistentDataPath + "/saveData.json";

        // check if it exists
        if (File.Exists(path))
        {
            // read file and convert back to game data
            string json = System.IO.File.ReadAllText(path);
            DataStorage loadedData = JsonUtility.FromJson<DataStorage>(json);

            // update the player position
            playerTransform.position = new Vector2(loadedData.position[0], loadedData.position[1]);
            Vector3 loadedPosition = new Vector2(loadedData.position[0], loadedData.position[1]);
            playerTransform.position = loadedPosition;

        }
        else
        {
            Debug.LogWarning("File not found");
        }
    }

}
