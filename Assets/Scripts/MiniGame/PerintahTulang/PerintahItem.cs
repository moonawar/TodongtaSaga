using UnityEngine;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    [CreateAssetMenu(fileName = "NewPerintahItem", menuName = "Minigame/Tulang/Item", order = 0)]
    public class PerintahItem : ScriptableObject {
        public string itemName;
        public Sprite itemSprite;
        public Sprite hintSprite;
    }
}