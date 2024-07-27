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

    public GameState CurrentState { get; private set; } = GameState.Gameplay;

    [Header("General UI")]
    [SerializeField] private RectTransform dialogueUI;
    [SerializeField] private RectTransform cutsceneUI;
    [SerializeField] private RectTransform gameplayUI;
    [SerializeField] private RectTransform pauseUI;


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
            OnGameStateChanged += state => {
                Debug.Log($"Game State Changed to {state}");
            };
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
            GameObject playerCanvas = GameObject.FindWithTag("PlayerCanvas");
            if (playerCanvas != null) {
                playerUI = playerCanvas.transform as RectTransform;
            }
        }

        if (playerUI != null) {
            playerUI.gameObject.SetActive(false);
        }
    }
    
    private void EnableGameplayUI() {
        gameplayUI.gameObject.SetActive(true);

        if (playerUI == null) {
            GameObject playerCanvas = GameObject.FindWithTag("PlayerCanvas");
            if (playerCanvas != null) {
                playerUI = playerCanvas.transform as RectTransform;
            }
        }

        if (playerUI != null) {
            playerUI.gameObject.SetActive(true);
        }
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

    public void ToGameplay() {
        dialogueUI.gameObject.SetActive(false);
        cutsceneUI.gameObject.SetActive(false);

        EnableGameplayUI();

        SetState(GameState.Gameplay);
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

        SetState(GameState.Gameplay);
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

        SetState(GameState.Gameplay);
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

        SetState(GameState.Gameplay);
    }

    public void ToOpenUI() {
        gameplayUI.gameObject.SetActive(false);
        dialogueUI.gameObject.SetActive(false);
        cutsceneUI.gameObject.SetActive(false);

        SetState(GameState.OpeningUI);
    }

    public void ToOpenUIOverride() {
        SetState(GameState.OpeningUI);
    }

    public void ToGameplayOverride() {
        SetState(GameState.Gameplay);
    }
}