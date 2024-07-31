using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookLeftHolder : MonoBehaviour
{
    [Header("Frame")]
    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] private Sprite hulaHulaFrame;
    [SerializeField] private Sprite donganTobuFrame;
    [SerializeField] private Sprite boruFrame;

    [Header("Checkmark")]
    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] private Sprite checkedSprite;
    [SerializeField] private Sprite uncheckedSprite;


    [Header("Character Field Reference")]
    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] private Image profilePic;
    [SerializeField] private Image frame;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Mission Field Reference")]
    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] private TextMeshProUGUI missionTitle;
    [SerializeField] private RectTransform missionContainers;
    [SerializeField] private GameObject bookMissionPrefab;

    public void Set(NPCData data) {
        profilePic.sprite = data.hiImage;
        nameText.text = data.npcName;
        descriptionText.text = data.description;

        NPCTungku tungku = data.tungku;
        switch (tungku) {
            case NPCTungku.HulaHula:
                frame.sprite = hulaHulaFrame;
                break;
            case NPCTungku.DonganTubu:
                frame.sprite = donganTobuFrame;
                break;
            case NPCTungku.Boru:
                frame.sprite = boruFrame;
                break;
        }

        List<Mission> missions = MissionManager.Instance.FindMissionForNPC(data.npcName);

        foreach (Transform child in missionContainers) {
            child.gameObject.SetActive(false);
        }

        if (missions.Count == 0) {
            missionTitle.text = "Tidak ada Misi";
            return;
        }

        missionTitle.text = "Misi";

        for (int i = 0; i < missions.Count; i++) {
            GameObject bookMission = i < missionContainers.childCount ? 
                missionContainers.GetChild(i).gameObject : 
                Instantiate(bookMissionPrefab, missionContainers);

            bookMission.GetComponent<BookMission>().Set(
                missions[i],
                missions[i].isCompleted ? checkedSprite : uncheckedSprite
            );
            bookMission.SetActive(true);
        }
    }
}
