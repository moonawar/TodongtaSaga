using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    public static string SAVE_FOLDER;
    public static SaveManager Instance;
    public GameSave CurrentGameSave;
    private void Awake() {
        SAVE_FOLDER = Application.persistentDataPath + "/Saves";

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (!Directory.Exists(SAVE_FOLDER)) {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public void SaveGame() {
        GameSave gameSave = new();
        Transform player = GameObject.FindWithTag("Player").transform;
        gameSave.PlayerPosition = player.position;
        gameSave.CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        gameSave.SceneObjects = SceneData.Instance.sceneObjects;
        gameSave.AllMissions = MissionManager.Instance.allMissions;
        gameSave.AvailableMissions = MissionManager.Instance.availableMissions;
        gameSave.NPCsData = NPCManager.Instance.GetNPCBookDatas();
        gameSave.Achievements = AchievementManager.Instance.achievements;
        gameSave.UlosProgress = UlosManager.Instance.Progress;
        gameSave.UlosInitialized = UlosManager.Instance.UlosInitialized;

        string json = JsonUtility.ToJson(gameSave);
        File.WriteAllText(SAVE_FOLDER + "/save.json", json);

        CurrentGameSave = gameSave;
        Debug.Log("Game Saved");
    }

    public void LoadGame() {
        SceneLoader.Instance.DeleteCore();
        if (File.Exists(SAVE_FOLDER + "/save.json")) {
            string json = File.ReadAllText(SAVE_FOLDER + "/save.json");
            GameSave gameSave = JsonUtility.FromJson<GameSave>(json);
            CurrentGameSave = gameSave;

            SceneLoader.Instance.LoadScene(gameSave.CurrentScene, () => {
                Transform player = GameObject.FindWithTag("Player").transform;
                GameObject followPlayer = Camera.main.transform.GetChild(0).gameObject;
                followPlayer.SetActive(false);
                player.position = gameSave.PlayerPosition;
                Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
                followPlayer.SetActive(true);
                SceneData.Instance.sceneObjects = gameSave.SceneObjects;
                MissionManager.Instance.allMissions = gameSave.AllMissions;
                MissionManager.Instance.availableMissions = gameSave.AvailableMissions;
                MissionManager.Instance.IsMissionLoaded = true;
                NPCManager.Instance.LoadNPCBookDatas(gameSave.NPCsData);
                AchievementManager.Instance.achievements = gameSave.Achievements;
                UlosManager.Instance.Progress = gameSave.UlosProgress;
                UlosManager.Instance.UlosInitialized = gameSave.UlosInitialized;
            });
            Debug.Log("Game Loaded");
        } else {
            SceneLoader.Instance.LoadScene("Gameplay", () => {
                MissionManager.Instance.IsMissionLoaded = true;
            });
        }
    }

    public void DeleteSave() {
        if (File.Exists(SAVE_FOLDER + "/save.json")) {
            File.Delete(SAVE_FOLDER + "/save.json");
            Debug.Log("Save Deleted");
            if (UITextPopup.Instance != null) {
                UITextPopup.Instance.Show("Save Telah Dihapus");
            }
        } else {
            Debug.Log("No Save Found. Skipping Delete.");
            if (UITextPopup.Instance != null) {
                UITextPopup.Instance.Show("Tidak Ada Save Yang Ditemukan");
            }
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveManager saveMgr = (SaveManager)target;

        if (GUILayout.Button("Save Game"))
        {
            saveMgr.SaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            saveMgr.LoadGame();
        }

        if (GUILayout.Button("Delete Save"))
        {
            saveMgr.DeleteSave();
        }
    }
}
#endif