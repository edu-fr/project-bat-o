using System.Collections;
using Resources.Project.Runtime.Scripts.UI;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Player
{
    public class PlayerCollectorController : MonoBehaviour
    {
        [SerializeField] private bool IsCollecting;
        [SerializeField] private int AccumulatedExp;
        [SerializeField] private ArrayList CollectedItems;
        [SerializeField] private float CollectingInterval;
        [SerializeField] private float CollectingCurrentTime;
        [SerializeField] private float TimeBetweenPopUps;
    
        public Transform PrefabLootPopup;

        void Awake()
        {
            if (CollectingInterval == 0) CollectingInterval = 3f;
            if (TimeBetweenPopUps == 0) TimeBetweenPopUps = 0.4f;
            CollectedItems = new ArrayList();
            ResetVariables();
        }

        void Update()
        {
            if (IsCollecting)
            {
                CollectingCurrentTime += Time.deltaTime;
                if (CollectingCurrentTime > CollectingInterval)
                {
                    // Finished
                    CreateCollectedStuffPopUps();
                    ResetVariables();
                }
            }
        }

        public void PlayerCollect(int amount)
        {
            IncrementAccumulatedExp(amount);
            CollectingCurrentTime = 0;
        }

        public void PlayerCollect(string itemName)
        {
            AddItemToCollectedList(itemName);
        }
    
        private void IncrementAccumulatedExp(int amount)
        {
            if (!IsCollecting)
            {
                IsCollecting = true;
                AccumulatedExp = 0;
            }
            AccumulatedExp += amount;
            GetComponentInParent<PlayerLevelController>().GainExperience(amount);
        }

        private void AddItemToCollectedList(string itemName)
        {
            CollectedItems.Add(itemName);
        }
    
        private void CreateCollectedStuffPopUps()
        {
            var position = transform.position;
            LootPopup.Create(position, AccumulatedExp, PrefabLootPopup);
            ArrayList itemList = new ArrayList(CollectedItems);
            StartCoroutine(CreateItemPopUps(position, itemList));
        }

        private IEnumerator CreateItemPopUps(Vector3 position, ArrayList itemList)
        {
            yield return new WaitForSeconds(TimeBetweenPopUps);
            foreach (var item in itemList)
            {
                LootPopup.Create(position, item.ToString(), PrefabLootPopup);
                yield return new WaitForSeconds(TimeBetweenPopUps);
            }
        }

        private void ResetVariables()
        {
            IsCollecting = false;
            CollectingCurrentTime = 0;
            AccumulatedExp = 0;
            CollectedItems.Clear();
        }
    }
}
