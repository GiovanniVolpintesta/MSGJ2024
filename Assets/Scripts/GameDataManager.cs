using data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField]
    private string DialogStructureFilePath;

    [SerializeField]
    private GameData gameData;

    // Start is called before the first frame update
    void Awake()
    {
        GameDataManager[] objs = Object.FindObjectsByType<GameDataManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (GameDataManager o in objs)
        {
            if (!gameObjects.Contains(o.gameObject))
            {
                gameObjects.Add(o.gameObject);
            }
        }

        if (gameObjects.Count > 1)
        {
            Debug.LogWarning("A GameDataManager is already present. This will be destroyed.");
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);

            Dialogue.ResetDefaultIdGenerator();

            using (StreamReader reader = new StreamReader(DialogStructureFilePath))
            {
                gameData = GameData.createFromJSON(reader.ReadToEnd());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
