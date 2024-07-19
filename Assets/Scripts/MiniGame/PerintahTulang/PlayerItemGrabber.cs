using System.Collections.Generic;
using UnityEngine;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public class PlayerItemGrabber : MonoBehaviour {
        public List<PerintahItem> GrabbedItems {get; private set;} = new();
        [SerializeField] private RectTransform grabbedItemsContainer;
        [SerializeField] private GameObject grabbedItemPrefab;
        [SerializeField] private int maxCapacity = 7;

        public void GrabItem(PerintahItem item)
        {
            if (GrabbedItems.Count >= maxCapacity) return;

            GrabbedItems.Add(item);

            var itemObj = Instantiate(grabbedItemPrefab, grabbedItemsContainer);
            itemObj.GetComponent<GrabbedItem>().Init(this);
            itemObj.GetComponent<GrabbedItem>().Set(item.itemSprite, item.itemName);
        }

        public void RemoveItem(PerintahItem item)
        {
            GrabbedItems.Remove(item);
        }

        public void RemoveItem(int index)
        {
            GrabbedItems.RemoveAt(index);
        }
    }
}