using UnityEngine;
using Random = UnityEngine.Random;

namespace Resources.Scripts.Objects
{
    public class LootScript : MonoBehaviour
    {
        public Transform ObjectTransform;
        private float Delay = 0;
        private float PastTime = 0;
        [SerializeField] [Range(0, 2)]
        private float When = 0.35f;
        private Vector3 Offset;
        [SerializeField] [Range(1, 3)] private float XRandomDistance = 1.4f;
        [SerializeField] [Range(1, 3)]private float YRandomDistance = 1.4f;
        [SerializeField] public int Amount;
        [SerializeField] private string ItemName;
        private static GameObject _prefabLoot;
        private bool CanBeCollected = false;

        public static GameObject Create(Vector3 position, int amount) // Exp loot creation
        {
            _prefabLoot = UnityEngine.Resources.Load("Project/Runtime/Prefabs/Objects/LootDrop") as GameObject;
            GameObject lootDropParent = GameObject.FindGameObjectWithTag("Loot Drop Parent");
            if (!lootDropParent) lootDropParent = new GameObject("Loot Drop Parent");
            lootDropParent.tag = "Loot Drop Parent";
            GameObject lootObject = Instantiate(_prefabLoot, position, Quaternion.identity, lootDropParent.transform);
            LootScript lootComponent = lootObject.GetComponent<LootScript>();
            lootComponent.Amount = amount;
            return lootObject;
        }
        
        public static GameObject Create(Vector3 position, string itemName) // Item loot creation
        {
            _prefabLoot = UnityEngine.Resources.Load("Project/Runtime/Prefabs/Objects/LootDrop") as GameObject;
            GameObject lootDropParent = GameObject.FindGameObjectWithTag("Loot Drop Parent");
            if (!lootDropParent) lootDropParent = new GameObject("Loot Drop Parent");
            lootDropParent.tag = "Loot Drop Parent";
            GameObject lootObject = Instantiate(_prefabLoot, position, Quaternion.identity, lootDropParent.transform);
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

            if (!CanBeCollected)
            {
                if(Delay > When) CanBeCollected = true;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (!CanBeCollected) return;

            if (!other.CompareTag("Player") || other.isTrigger) return;
            var playerLevelController = other.gameObject.GetComponent<PlayerLevelController>();
            var playerCollectorController = other.gameObject.GetComponent<PlayerCollectorController>();
            playerCollectorController.PlayerCollect(Amount);
            Destroy(gameObject);
        }
    }
}
