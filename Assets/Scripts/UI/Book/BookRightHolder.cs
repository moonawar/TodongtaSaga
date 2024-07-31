using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookRightHolder : MonoBehaviour
{
    [SerializeField] private GameObject charactersContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private BookLeftHolder leftHolder;

    private void OnEnable() {
        List<NPCData> datas = NPCManager.Instance.GetNPCBookDatas();
        for (int i = 0; i < datas.Count; i++) {
            GameObject cardObj = i < charactersContainer.transform.childCount ? 
                charactersContainer.transform.GetChild(i).gameObject : 
                Instantiate(cardPrefab, charactersContainer.transform);

            CharacterCard card = cardObj.GetComponent<CharacterCard>();
            card.Set(datas[i]);
            NPCData data = datas[i];
            cardObj.GetComponent<Button>().onClick.AddListener(() => {
                leftHolder.Set(data);
                card.BorderOn();
            });

            if (i == 0) {
                card.BorderOn();
                leftHolder.Set(data);
            }
        }
    }

    private void OnDisable() {
        
    }
}