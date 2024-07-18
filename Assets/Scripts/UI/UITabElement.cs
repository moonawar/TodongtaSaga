using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabElement : MonoBehaviour {
    private static List<UITabElement> tabs = new List<UITabElement>();

    [SerializeField] private Sprite active;
    [SerializeField] private Sprite inactive;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        tabs.Add(this);
    }

    public void MakeActive() {
        image.sprite = active;

        foreach (UITabElement tab in tabs) {
            if (tab != this) {
                tab.MakeInactive();
            }
        }
    }

    public void MakeInactive() {
        image.sprite = inactive;
    }
}