using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts.Objects
{
    public class LootScript : MonoBehaviour
    {
        public Transform ObjectTransform;
        private float Delay = 0;
        private float PastTime = 0;
        private float When = 1.0f;
        private Vector3 Offset;
        private float XRandomDistance = 1f;
        private float YRandomDistance = 1f;
        [SerializeField] public int Amount;
        [SerializeField] private string ItemName;
        public Transform PrefabExperiencePopup;
        private static GameObject PrefabLoot;
        
        public static GameObject Create(int amount) // Exp loot creation
        {
            PrefabLoot = UnityEngine.Resources.Load("Prefabs/Objects/LootDrop") as GameObject;
            GameObject lootObject = Instantiate( PrefabLoot);
            LootScript lootComponent = lootObject.GetComponent<LootScript>();
            lootComponent.Amount = amount;
            return lootObject;
        }
        
        public static GameObject Create(string itemName) // Item loot creation
        {
            PrefabLoot = UnityEngine.Resources.Load("Prefabs/Objects/LootDrop") as GameObject;
            GameObject lootObject = Instantiate(PrefabLoot);
            LootScript lootComponent = lootObject.GetComponent<LootScript>();
            lootComponent.ItemName = itemName;
            return lootObject;
        }
        
        private void Awake()
        {
            Offset = new Vector3(Random.Range(-XRandomDistance, XRandomDistance), 
                Random.Range(-YRandomDistance, YRandomDistance), Offset.z);
        }

        // Update is called once per frame
        void Update()
        {
            if (When >= Delay)
            {
                PastTime = Time.deltaTime;
                ObjectTransform.position += Offset * Time.deltaTime;
                Delay += PastTime;
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            { 
                var playerLevelController = other.gameObject.GetComponent<PlayerLevelController>();
                playerLevelController.GainExperience(Amount);
                LootPopup.Create(playerLevelController.transform.position, Amount, PrefabExperiencePopup);
                Destroy(gameObject);
            }
        }
    }
}
