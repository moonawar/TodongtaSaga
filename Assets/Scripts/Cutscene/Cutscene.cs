using UnityEngine;

[System.Serializable]
public class CutsceneImage {
    public string name;
    public Sprite image;
}

[CreateAssetMenu(fileName = "NewCutscene", menuName = "RPG/Cutscene")]
public class Cutscene : ScriptableObject {
    public string yarnTitle;
    public CutsceneImage[] images;
}