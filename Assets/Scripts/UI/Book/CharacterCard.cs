using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour {
    [SerializeField] private Image profilePic;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image border;

    private static List<CharacterCard> cards = new List<CharacterCard>();

    private void Awake() {
        cards.Add(this);
    }

    public void Set(NPCData data) {
        profilePic.sprite = data.loImage;
        nameText.text = data.npcName;
    }

    public void BorderOn() {
        foreach (CharacterCard card in cards) {
            card.BorderOff();
        }

        border.gameObject.SetActive(true);
    }

    public void BorderOff() {
        border.gameObject.SetActive(false);
    }
}