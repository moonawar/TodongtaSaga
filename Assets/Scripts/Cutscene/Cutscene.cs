using UnityEngine;

[CreateAssetMenu(fileName = "NewCutscene", menuName = "RPG/Cutscene")]
public class Cutscene : ScriptableObject {
    public string yarnTitle;
    public Sprite[] images;
}