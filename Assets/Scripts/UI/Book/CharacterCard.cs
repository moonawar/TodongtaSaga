using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour {
    [SerializeField] private Image profilePic;
    [SerializeField] private TextMeshProUGUI nameText;
    public NPCData selfData {get; private set;}

    public void Set(NPCData data) {
        selfData = data;

        profilePic.sprite = data.loImage;
        nameText.text = data.npcName;
    }
}