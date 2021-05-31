using System.Collections;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    public class LootScript : MonoBehaviour
    {
        public Transform ObjectTransform;
        private float Delay = 0;
        private float PastTime = 0;
        private float When = 1.0f;
        private Vector3 Offset;
        private float XRandomDistance = 3f;
        private float YRandomDistance = 3f;
        [SerializeField] private float TimeToDestroy = 1.1f;
        [SerializeField]
        public int Amount;
        [SerializeField] private string ItemName;
        
        public Transform PrefabExperiencePopup;

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
                other.gameObject.GetComponent<PlayerLevelController>().GetExperience(Amount);
                // LootPopup.Create(transform.position, Amount, PrefabExperiencePopup);
                StartCoroutine(DestroyObject());
            }
        }
        private IEnumerator DestroyObject()
        {
            yield return new WaitForSeconds(TimeToDestroy);
            Destroy(this.gameObject);
        }
    }
}
