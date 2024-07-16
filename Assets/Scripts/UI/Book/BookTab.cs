using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookTab : MonoBehaviour {
    private static List<BookTab> tabs = new List<BookTab>();

    [SerializeField] private Sprite active;
    [SerializeField] private Sprite inactive;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        tabs.Add(this);
    }

    public void MakeActive() {
        image.sprite = active;

        foreach (BookTab tab in tabs) {
            if (tab != this) {
                tab.MakeInactive();
            }
        }
    }

    public void MakeInactive() {
        image.sprite = inactive;
    }
}