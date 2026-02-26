using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// used info from https://www.youtube.com/watch?v=wJhMaLkeEuY for initial skeleton
// used Chat GPT for assistance and debugging help after I identified what needed to be saved

public class DataManager : MonoBehaviour
{

    public Transform playerTransform;

    // save game function
    public void SaveGame()
    {
        // new instance of class
        DataStorage data = new DataStorage();


        ///

        // World data
        data.gameState = GameManager.Instance.state;
        data.worldGenSeed = GameManager.Instance.worldSeed;


        // Sloop-O-War data
        data.boatPosition = new float[]
        {
            GameManager.Instance.boatPosition.x,
            GameManager.Instance.boatPosition.y,
            GameManager.Instance.boatPosition.z
        };

        data.boatVelocity = new float[] {
        GameManager.Instance.boatVelocity.x,
        GameManager.Instance.boatVelocity.y
        };

        data.hasBoatState = GameManager.Instance.hasBoatState;


        // Island data
        data.currentIslandID = GameManager.Instance.currentIslandID;
        data.currentIsalndMorality = GameManager.Instance.currentIslandMorality;



        // Player data
        data.playerPosition = new float[]
        {
            playerTransform.position.x,
            playerTransform.position.y,
            playerTransform.position.z
        };

        ///


        // datastorage to json string
        string json = JsonUtility.ToJson(data);

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


            ///

            // World data loaded back in
            GameManager.Instance.worldSeed = loadedData.worldGenSeed;
            GameManager.Instance.UpdateGameState(loadedData.gameState); // Update game state with save after seed generated


            // Sloop-O-War data loaded back in
            GameManager.Instance.boatPosition = new Vector3
            (
            loadedData.boatPosition[0],
            loadedData.boatPosition[1],
            loadedData.boatPosition[2]
            );

            GameManager.Instance.boatVelocity = new Vector2(
            loadedData.boatVelocity[0],
            loadedData.boatVelocity[1]
            );

            GameManager.Instance.hasBoatState = loadedData.hasBoatState;


            // Island data loaded back in
            GameManager.Instance.currentIslandID = loadedData.currentIslandID;
            GameManager.Instance.currentIslandMorality = loadedData.currentIsalndMorality;



            // Player data loaded back in
            Vector3 loadedPosition = new Vector3 (
            loadedData.playerPosition[0],
            loadedData.playerPosition[1],
            loadedData.playerPosition[2]
            );
            playerTransform.position = loadedPosition;

            ///

        }
        else
        {
            Debug.LogWarning("File not found");
        }
    }

}
