using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance { get {
        if (instance == null) {
            instance = FindObjectOfType<GameStateManager>();
        }
        return instance;
    }}
    private static GameStateManager instance;

    public GameState CurrentState { get; private set; } = GameState.Explore;

    [Header("General UI")]
    [SerializeField] private RectTransform dialogueUI;
    [SerializeField] private RectTransform cutsceneUI;
    [SerializeField] private RectTransform gameplayUI;
    [SerializeField] private RectTransform pauseUI;
    [SerializeField] private RectTransform demoEndUI;


    [Header("Game Specific UI")]
    [SerializeField] private RectTransform bookUI;
    [SerializeField] private RectTransform missionUI;
    [SerializeField] private RectTransform inventoryUI;
    
    [Header("Visual Effects")]
    [SerializeField] private Volume postProcessVolume;
    private DepthOfField dof;

    public Action<GameState> OnGameStateChanged = delegate { };

    [SerializeField] private bool debugOn;
    private RectTransform playerUI;

    private GameState previousState;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        if (debugOn) {
            OnGameStateChanged += state => Debug.Log($"Game State Changed to {state}");
        }
    }

    private void Start() {
        if (postProcessVolume != null) {
            postProcessVolume.profile.TryGet(out dof);
        }
    }

    private void SetState(GameState state) {
        CurrentState = state;
        OnGameStateChanged?.Invoke(state);
    }

    private void DisableGameplayUI() {
        gameplayUI.gameObject.SetActive(false);
        if (playerUI == null) {
            playerUI = GameObject.FindWithTag("PlayerCanvas").transform as RectTransform;
        }

        playerUI.gameObject.SetActive(false);
    }
    
    private void EnableGameplayUI() {
        gameplayUI.gameObject.SetActive(true);

        if (playerUI == null) {
            playerUI = GameObject.FindWithTag("PlayerCanvas").transform as RectTransform;
        }
        playerUI.gameObject.SetActive(true);
    }

    public void ToDialogue() {
        dialogueUI.gameObject.SetActive(true);
        DisableGameplayUI();

        SetState(GameState.Dialogue);
    }

    public void ToCutscene() {
        cutsceneUI.gameObject.SetActive(true);
        DisableGameplayUI();

        SetState(GameState.Cutscene);
    }

    public void ToExplore(bool immediate = false) {
        dialogueUI.gameObject.SetActive(false);
        cutsceneUI.gameObject.SetActive(false);

        EnableGameplayUI();

        SetState(GameState.Explore);
    }

    public void Pause() {
        pauseUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        previousState = CurrentState;
        SetState(GameState.Paused);
    }

    public void Unpause() {
        pauseUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SetState(previousState);
    }

    public void ToDemoEnd() {
        demoEndUI.gameObject.SetActive(true);
        Time.timeScale = 0f;
        previousState = CurrentState;
        SetState(GameState.DemoEnd);
    }

    public void ContinueDemo() {
        demoEndUI.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SetState(previousState);

        MissionAnnouncer.Instance.AnnounceMission("Demo Berakhir", "Selamat, kamu telah menyelesaikan demo ini. Terima kasih telah bermain!");
    }

    public void MainMenu() {
        Time.timeScale = 1f;
        SceneLoader.Instance.ToMainMenu();
    }

    public void DeleteSave() {
        SaveManager.Instance.DeleteSave();
        SaveManager.Instance.LoadGame();
    }

    /**
        All the methods below must be called from Explore state
    */
    public void OpenBook() {
        dof.active = true;

        bookUI.gameObject.SetActive(true);
        DisableGameplayUI();

        SetState(GameState.OpeningUI);
    }

    public void CloseBook() {
        dof.active = false;

        bookUI.gameObject.SetActive(false);
        EnableGameplayUI();

        SetState(GameState.Explore);
    }

    public void OpenMission() {
        dof.active = true;

        missionUI.gameObject.SetActive(true);
        DisableGameplayUI();

        SetState(GameState.OpeningUI);
    }

    public void CloseMission() {
        dof.active = false;

        missionUI.gameObject.SetActive(false);
        EnableGameplayUI();

        SetState(GameState.Explore);
    }

    public void OpenInventory() {
        dof.active = true;

        inventoryUI.gameObject.SetActive(true);
        DisableGameplayUI();

        SetState(GameState.OpeningUI);
    }

    public void CloseInventory() {
        dof.active = false;

        inventoryUI.gameObject.SetActive(false);
        EnableGameplayUI();

        SetState(GameState.Explore);
    }

    public void ToMinigameTutorial() {
        gameplayUI.gameObject.SetActive(false);
        dialogueUI.gameObject.SetActive(false);
        cutsceneUI.gameObject.SetActive(false);

        SetState(GameState.MinigameTutorial);
    }

    public void ToMinigamePlaying() {
        SetState(GameState.MinigamePlaying);
    }

    public void ToMinigameEnd() {
        SetState(GameState.MinigameEnd);
    }
}