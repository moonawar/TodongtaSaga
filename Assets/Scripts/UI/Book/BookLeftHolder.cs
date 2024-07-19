using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookLeftHolder : MonoBehaviour
{
    [Header("Frame")]
    [SerializeField] private Sprite hulaHulaFrame;
    [SerializeField] private Sprite donganTobuFrame;
    [SerializeField] private Sprite boruFrame;

    [Header("Field Reference")]
    [SerializeField] private Image profilePic;
    [SerializeField] private Image frame;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RectTransform missionPanel;

    public void Set(NPCData data) {
        profilePic.sprite = data.hiImage;
        nameText.text = data.npcName;
        descriptionText.text = data.description;

        NPCTungku tungku = data.tungku;
        switch (tungku) {
            case NPCTungku.HulaHula:
                frame.sprite = hulaHulaFrame;
                break;
            case NPCTungku.DonganTobu:
                frame.sprite = donganTobuFrame;
                break;
            case NPCTungku.Boru:
                frame.sprite = boruFrame;
                break;
        }
    }
}
