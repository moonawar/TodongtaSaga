using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Collections;
using NaughtyAttributes;
using Yarn.Unity;

[RequireComponent(typeof(CutsceneManager))]
[RequireComponent(typeof(DialogueManager))]
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public List<Mission> allMissions;
    public List<Mission> availableMissions = new();

    private CutsceneManager cutsceneManager;
    private DialogueManager dialogueManager;

    public bool IsMissionInProgress { get; private set; } = false;
    public bool IsMissionLoaded { get; set; } = false;

    [Header("Trigger Prefabs")]
    [SerializeField] private GameObject locationTrigger;

    [Header("Visual Effects")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private CanvasGroup cutsceneCanvasGroup;

    private Mission currentMission;
    public Mission CurrentMission => currentMission;
    private int currentTaskIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        cutsceneManager = GetComponent<CutsceneManager>();
        dialogueManager = GetComponent<DialogueManager>();
    }

    private void Start() {
        StartCoroutine(DelayedStart());
    }
    
    private IEnumerator DelayedStart() {
        yield return new WaitUntil(() => IsMissionLoaded);
        if (availableMissions.Count == 0)
        {
            MissionAnnouncer.Instance.AnnounceMission("Demo Berakhir", "Selamat, kamu telah menyelesaikan demo ini. Gunakan tombol reset game di dev tools. Thanks for playing!");
        }

        AudioManager.Instance.PlayBGMOverwrite("Game");
        foreach (var mission in availableMissions)
        {
            HandleNewMission(mission);
        }
    }

    public void StartMission(Mission mission)
    {
        // Code to start the mission
        AudioManager.Instance.StopBGMCrossfade();
        Debug.Log("Mission Started: " + mission.missionName);
        currentMission = mission;
        currentTaskIndex = 0;
        IsMissionInProgress = true;
        ExecuteMissionAction(mission.onMissionStart);
        ExecuteNextTask(mission, 0);
    }

    public void CompleteMission(Mission mission)
    {
        AudioManager.Instance.PlayBGMOverwrite("Game");
        Debug.Log("Mission Completed: " + mission.missionName);

        availableMissions.Remove(mission);

        ExecuteMissionAction(mission.onMissionComplete);

        mission.OnMissionCompleteAction?.Invoke();
        mission.OnMissionCompleteAction = null;
        mission.isCompleted = true;

        foreach (MissionRelationshipImpact impact in mission.relationshipImpacts)
        {
            NPCManager.Instance.UpdateRelationship(impact.npcName, impact.impactAmount);
        }

        foreach (Mission followUpMission in mission.followUpMissions)
        {
            if (availableMissions.Contains(followUpMission)) continue; // Skip if the mission is already there
            availableMissions.Add(followUpMission);
            HandleNewMission(followUpMission);
        }

        string missionsString = string.Join(", ", availableMissions.ConvertAll(m => m.missionName).ToArray());

        Debug.Log($"Missions Available: [{missionsString}]");

        SaveManager.Instance.SaveGame();
        GameStateManager.Instance.ToGameplay();

        currentMission = null;
        IsMissionInProgress = false;

        if (availableMissions.Count == 0)
        {
            MissionAnnouncer.Instance.AnnounceMission("Demo Berakhir", "Selamat, kamu telah menyelesaikan demo ini. Terima kasih telah bermain!");
        }
    }

    [YarnCommand("failMission")]
    public void FailMission() {
        if (currentMission == null) return;
        Debug.Log("Mission Failed: " + currentMission.missionName);

        GameStateManager.Instance.ToGameplay();

        currentMission = null;
        IsMissionInProgress = false;
        currentTaskIndex = 0;

        dialogueManager.CleanDialogue();
        cutsceneManager.CleanCutscene();
    }

    public void HandleNewMission(Mission mission) {
        Debug.Log("Handling new mission: " + mission.missionName);

        if (mission == null) return;
        // Create the trigger for the new mission
        MissionTriggerType triggerType = mission.trigger.type;
        switch (triggerType)
        {
            case MissionTriggerType.NPC:
                // Spawn NPC interaction point
                Debug.Log("Assigning mission to NPC: " + mission.trigger.npcName);

                NPCManager.Instance.AssignMissionToNPC(mission, mission.trigger.npcName);
                NPC npc = NPCManager.Instance.FindNPCWithName(mission.trigger.npcName);

                if (mission.trigger.showWaypoint)
                {
                    Debug.Log("Setting destination to NPC: " + mission.trigger.npcName);

                    WaypointManager.Instance.SetDestination(npc.transform.position);
                    mission.OnMissionCompleteAction += () => WaypointManager.Instance.FinishDestination();
                }

                break;
            case MissionTriggerType.Object:
                // Spawn object interaction point
                // Object obj = ObjectManager.Instance.GetObject(mission.trigger.objectName);
                // obj.SpawnInteractionPoint();
                break;
            case MissionTriggerType.Location:
                // Spawn location interaction point
                GameObject trigger = Instantiate(locationTrigger, mission.trigger.location, Quaternion.identity);
                trigger.GetComponent<LocationTrigger>().Mission = mission;
                trigger.GetComponent<LocationTrigger>().Radius = mission.trigger.radius;
                mission.OnMissionCompleteAction += () => { if (trigger != null) Destroy(trigger); };

                if (mission.trigger.showWaypoint)
                {
                    Debug.Log("Setting destination to mission location: " + mission.trigger.location);

                    WaypointManager.Instance.SetDestination(mission.trigger.location);
                    mission.OnMissionCompleteAction += () => WaypointManager.Instance.FinishDestination();
                }

                break;
            case MissionTriggerType.Auto:
                // Automatically start the mission
                StartMission(mission);
                break;
        }

        // Announce if the mission should be announced
        if (mission.announce)
        {
            MissionAnnouncer.Instance.AnnounceMission("Misi Baru", mission.description);
        }

        ExecuteMissionAction(mission.onMissionHandled);
    }

    [Button("Skip Task")]
    public void SkipCurrentTask() {
        // Skip current task
        dialogueManager.CleanDialogue();
        cutsceneManager.CleanCutscene();
        GameStateManager.Instance.ToGameplay();
        ExecuteNextTask(currentMission, currentTaskIndex + 1);
    }

    [Button("Skip Mission")]
    public void SkipCurrentMission() {
        // Skip entire mission
        if (currentMission == null) {
            currentMission = availableMissions[0];
        }

        dialogueManager.CleanDialogue();
        cutsceneManager.CleanCutscene();
        CompleteMission(currentMission);
    }

    private void ExecuteMissionAction(List<MissionAction> actions) {
        foreach (var action in actions)
        {
            MissionAction.Type actionType = action.type;
            switch (actionType)
            {
                case MissionAction.Type.CreateInteractableObject:
                    InteractableObjectManager.Instance.CreateInteractableObject(action.position, ExecuteNextTask);
                    break;
                case MissionAction.Type.PositionPlayer:
                    Transform player = GameObject.FindWithTag("Player").transform;
                    GameObject followPlayer = Camera.main.transform.GetChild(0).gameObject;
                    followPlayer.SetActive(false);
                    player.position = action.position;
                    
                    Vector2 playerOrientation;
                    switch (action.orientation)
                    {
                        case PosOrientationType.Up:
                            playerOrientation = Vector2.up;
                            break;
                        case PosOrientationType.Down:
                            playerOrientation = Vector2.down;
                            break;
                        case PosOrientationType.Left:
                            playerOrientation = Vector2.left;
                            break;
                        case PosOrientationType.Right:
                            playerOrientation = Vector2.right;
                            break;
                        default:
                            playerOrientation = Vector2.up;
                            break;
                    }

                    Animator playerAnimator = player.GetComponent<Animator>();
                    playerAnimator.SetFloat("Horizontal", playerOrientation.x);
                    playerAnimator.SetFloat("Vertical", playerOrientation.y);

                    Camera.main.transform.position = new Vector3(action.position.x, action.position.y, Camera.main.transform.position.z);
                    followPlayer.SetActive(true);
                    break;
                case MissionAction.Type.ExecuteNextTask:
                    ExecuteNextTask();
                    break;
                case MissionAction.Type.PositionNPC:
                    if (action.assignNextTask)
                        NPCManager.Instance.AssignNextTaskToNPC(action.npcName, action.position);
                    else
                        NPCManager.Instance.PositionNPC(action.npcName, action.position);
                    break;
                case MissionAction.Type.SetDissapearNPC:
                    NPCManager.Instance.SetDissapearNPC(action.npcName, action.dissapearImmediately);
                    break;
                case MissionAction.Type.ToExplore:
                    GameStateManager.Instance.ToGameplay();
                    break;
                case MissionAction.Type.GetAchievement:
                    AchievementManager.Instance.UnlockAchievement(action.achievementId);
                    break;
                case MissionAction.Type.UlosAction:
                    UlosManager.Instance.ExecuteUlosAction(action.ulosAction);
                    break;
            }
        }
    }

    public void ExecuteNextTask() {
        ExecuteNextTask(currentMission);
    }

    public void ExecuteNextTask(Mission mission) {
        ExecuteNextTask(mission, currentTaskIndex + 1);
    }

    private void ExecuteNextTask(Mission mission, int taskIndex)
    {
        if (!availableMissions.Contains(mission))
        {
            Debug.Log($"Mission {mission.name} not found in the list of missions. Skipping task.");
        }

        // Call OnTaskComplete actions
        if (taskIndex > 0)
        {
            var previousTask = mission.tasks[taskIndex - 1];
            ExecuteMissionAction(previousTask.onTaskComplete);
        }

        currentTaskIndex = taskIndex;
        if (taskIndex >= mission.tasks.Count)
        {
            CompleteMission(mission);
            return;
        }

        Debug.Log($"Executing Task {taskIndex} of type {mission.tasks[taskIndex].type} for mission {mission.missionName}");

        var task = mission.tasks[taskIndex];
        switch (task.type)
        {
            case MissionTaskType.Dialogue:
                // Trigger NPC dialogue (integrated with Yarn Spinner)
                GameStateManager.Instance.ToDialogue();
                dialogueManager.StartDialogue(task.yarnTitle, () =>
                {
                    Debug.Log($"Dialogue Finished for mission {mission.missionName}");

                    GameStateManager.Instance.ToGameplay();
                    ExecuteNextTask(mission, taskIndex + 1);
                });
                break;
            case MissionTaskType.Cutscene:
                // Trigger cutscene
                cutsceneCanvasGroup.alpha = 0;
                FadeToBlack(() => {
                    cutsceneCanvasGroup.alpha = 1;
                    cutsceneManager.StartCutscene(task.cutscene, () =>
                    {
                        Debug.Log($"Cutscene Finished for mission {mission.missionName}");

                        GameStateManager.Instance.ToGameplay();
                        ExecuteNextTask(mission, taskIndex + 1);
                        FadeFromBlack();
                    });
                });

                break;
            case MissionTaskType.Travel:
                // Travel to location
                if (task.showWaypoint)
                {
                    WaypointManager.Instance.SetDestination(task.travelLocation);
                }
                break;
            case MissionTaskType.SwitchScene:
                SceneLoader.Instance.LoadScene(task.sceneName, ExecuteNextTask);
                break;
        }
    }


    public List<Mission> FindMissionForNPC(string npcName)
    {
        return allMissions.FindAll((Mission mission) => { return mission.isKeyMission && mission.keyMissionFor == npcName; });
    }

    #if UNITY_EDITOR
    [Button]
    public void RefreshMissionsFromAssets() {
        allMissions = new List<Mission>();
        string[] guids = AssetDatabase.FindAssets("t:Mission");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Mission mission = AssetDatabase.LoadAssetAtPath<Mission>(path);
            allMissions.Add(mission);
        }
    }
    #endif

    private void FadeToBlack(Action onComplete = null)
    {
        blackOverlay.color = new Color(0, 0, 0, 0);
        blackOverlay.DOFade(1f, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }
    private void FadeFromBlack(Action onComplete = null)
    {
        blackOverlay.color = new Color(0, 0, 0, 1);
        blackOverlay.DOFade(0f, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }
}