using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(CutsceneManager))]
[RequireComponent(typeof(DialogueManager))]
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public List<Mission> missions = new List<Mission>();

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
        foreach (var mission in missions)
        {
            HandleNewMission(mission);
        }
    }

    public void StartMission(Mission mission)
    {
        // Code to start the mission
        Debug.Log("Mission Started: " + mission.missionName);
        currentMission = mission;
        currentTaskIndex = 0;
        IsMissionInProgress = true;
        ExecuteNextTask(mission, 0);
    }

    // public List<Mission> GetAvailableMissions(NPC npc)
    // {
        // List<Mission> availableMissions = new List<Mission>();
        // foreach (var mission in missions)
        // {
        //     if (!mission.isCompleted && mission.assignedNPC == npc)
        //     {
        //         availableMissions.Add(mission);
        //     }
        // }
        // return availableMissions;
    // }

    public void CompleteMission(Mission mission)
    {
        missions.Remove(mission);

        mission.OnMissionCompleteAction?.Invoke();
        mission.OnMissionCompleteAction = null;

        foreach (MissionRelationshipImpact impact in mission.relationshipImpacts)
        {
            NPCManager.Instance.UpdateRelationship(impact.npcName, impact.impactAmount);
        }

        foreach (Mission followUpMission in mission.followUpMissions)
        {
            missions.Add(followUpMission);
            HandleNewMission(followUpMission);
        }

        currentMission = null;
        IsMissionInProgress = false;
        SaveManager.Instance.SaveGame();
        GameStateManager.Instance.ToExplore();

        if (missions.Count == 0)
        {
            MissionAnnouncer.Instance.AnnounceMission("Demo Berakhir", "Selamat, kamu telah menyelesaikan demo ini. Terima kasih telah bermain!");
        }
    }

    public void HandleNewMission(Mission mission) {
        if (mission == null) return;
        // Create the trigger for the new mission
        MissionTriggerType triggerType = mission.trigger.type;
        switch (triggerType)
        {
            case MissionTriggerType.NPC:
                // Spawn NPC interaction point
                NPCManager.Instance.AssignMissionToNPC(mission, mission.trigger.npcName);
                NPC npc = NPCManager.Instance.FindNPCWithName(mission.trigger.npcName);

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
    }

    public void SkipCurrentTask() {
        // Skip current task
        dialogueManager.OnSkipped();
        cutsceneManager.OnSkipped();
        GameStateManager.Instance.ToExplore();
        ExecuteNextTask(currentMission, currentTaskIndex + 1);
    }

    public void SkipCurrentMission() {
        // Skip entire mission
        if (currentMission == null) {
            currentMission = missions[0];
        }

        dialogueManager.OnSkipped();
        cutsceneManager.OnSkipped();
        CompleteMission(currentMission);
    }

    public void ExecuteNextTask() {
        ExecuteNextTask(currentMission);
    }

    public void ExecuteNextTask(Mission mission) {
        ExecuteNextTask(mission, currentTaskIndex + 1);
    }

    private void ExecuteNextTask(Mission mission, int taskIndex)
    {
        // Call OnTaskComplete actions
        if (taskIndex > 0)
        {
            var previousTask = mission.tasks[taskIndex - 1];
            foreach (var action in previousTask.onTaskComplete)
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
                        NPCManager.Instance.AssignNextTaskToNPC(action.npcName, action.position);
                        break;
                    case MissionAction.Type.SetDissapearNPC:
                        NPCManager.Instance.SetDissapearNPC(action.npcName, action.dissapearImmediately);
                        break;
                    case MissionAction.Type.ToExplore:
                        GameStateManager.Instance.ToExplore();
                        break;
                }
            }
        }

        currentTaskIndex = taskIndex;
        if (taskIndex >= mission.tasks.Count)
        {
            CompleteMission(mission);
            return;
        }

        var task = mission.tasks[taskIndex];
        switch (task.type)
        {
            case MissionTaskType.Dialogue:
                // Trigger NPC dialogue (integrated with Yarn Spinner)
                GameStateManager.Instance.ToDialogue();
                dialogueManager.StartDialogue(task.yarnTitle, () =>
                {
                    GameStateManager.Instance.ToExplore();
                    ExecuteNextTask(mission, taskIndex + 1);
                });
                break;
            case MissionTaskType.Cutscene:
                // Trigger cutscene
                cutsceneCanvasGroup.alpha = 0;
                GameStateManager.Instance.ToCutscene();
                FadeToBlack(() => {
                    cutsceneCanvasGroup.alpha = 1;
                    cutsceneManager.StartCutscene(task.cutscene, () =>
                    {
                        GameStateManager.Instance.ToExplore();
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
                // Player.Instance.MoveToLocation(task.travelLocation, () =>
                // {
                //     ExecuteNextTask(mission, taskIndex + 1);
                // });
                break;
            case MissionTaskType.SwitchScene:
                SceneLoader.Instance.LoadScene(task.sceneName, ExecuteNextTask);
                break;
        }
    }

    public void ResetMission(Mission mission) {
        missions.Remove(mission);
        missions.Add(mission);
        HandleNewMission(mission);
    }

    public void ResetCurrentMission() {
        ResetMission(currentMission);
    }

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

    // [YarnCommand("startMission")]
    // public void StartMissionCommand(string missionName)
    // {
    //     var mission = missions.Find(m => m.missionName == missionName);
    //     if (mission != null)
    //     {
    //         StartMission(mission);
    //     }
    // }

    // [YarnCommand("executeNextTask")]
    // public void ExecuteNextTaskCommand(string missionName, int taskIndex)
    // {
    //     var mission = missions.Find(m => m.missionName == missionName);
    //     if (mission != null)
    //     {
    //         ExecuteNextTask(mission, taskIndex);
    //     }
    // }


    // API For Cross-Scene Communication
    public static void CreateInteractableObject(Vector2 location, UnityEvent onInteraction)
    {
        InteractableObjectManager.Instance.CreateInteractableObject(location, onInteraction);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MissionManager))]
public class MissionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MissionManager missionManager = (MissionManager)target;
        if (GUILayout.Button("Skip Task"))
        {
            missionManager.SkipCurrentTask();
        }
        if (GUILayout.Button("Skip Mission"))
        {
            missionManager.SkipCurrentMission();
        }
    }
}
#endif