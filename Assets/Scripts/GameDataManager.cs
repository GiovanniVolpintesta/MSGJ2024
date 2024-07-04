using data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField]
    private string DialogStructureResourcePath;

    [SerializeField]
    private CharactersData CharactersData;

    [SerializeField]
    private GameData gameData;

    [SerializeField]
    private int dialoguesPerDay = 3;
    [SerializeField]
    private string dayStatId = "day";
    [SerializeField]
    private string[] sendersCounted = { };

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

            TextAsset txtAsset = (TextAsset)Resources.Load(DialogStructureResourcePath, typeof(TextAsset));
            GameData.createFromJSON(txtAsset.text);
            gameData = GameData.Instance;
            gameData.Initialize(CharactersData, dialoguesPerDay, dayStatId, sendersCounted);
        }
    }

}
