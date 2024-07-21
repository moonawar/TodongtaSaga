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
        foreach (var data in datas) {
            GameObject card = Instantiate(cardPrefab, charactersContainer.transform);
            card.GetComponent<CharacterCard>().Set(data);
            card.GetComponent<Button>().onClick.AddListener(() => leftHolder.Set(data));
        }

        leftHolder.Set(datas[0]);
    }

    private void OnDisable() {
        foreach (Transform child in charactersContainer.transform) {
            Destroy(child.gameObject);
        }
    }
}
