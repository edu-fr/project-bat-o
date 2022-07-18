using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    public class Dropper : MonoBehaviour
    {
        [Serializable]
        public class DroppableItem
        {
            private Transform _itemPrefab;
            private float _dropProbability;
        }

        [Range(0, 100f)] [SerializeField] private float expAmount;
        [SerializeField] private List<DroppableItem> droppableItems;
        [SerializeField] private Transform expPrefab;
        private GameObject _lootDropParent;

        public void Start()
        {
            _lootDropParent = GameObject.FindGameObjectWithTag("Loot Drop Parent");
            if (!_lootDropParent)
            {
                _lootDropParent = new GameObject("Loot Drop Parent")
                {
                    tag = "Loot Drop Parent"
                };
            }
        }

        public void DropExp()
        {
            for (var i = 0; i < expAmount; i++)
            {
                Instantiate(expPrefab, transform.position, Quaternion.identity, _lootDropParent.transform);
            }
        }

        public void DropItem()
        {
            throw new NotImplementedException();
        }
    }
}
