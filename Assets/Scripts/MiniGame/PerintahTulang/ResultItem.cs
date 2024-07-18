using UnityEngine;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    [CreateAssetMenu(fileName = "NewResultItem", menuName = "Minigame/Tulang/Result", order = 0)]
    public class ResultItem : ScriptableObject {
        public PerintahItem[] recipe;
        public string resultName;
        public Sprite resultSprite;
    }
}