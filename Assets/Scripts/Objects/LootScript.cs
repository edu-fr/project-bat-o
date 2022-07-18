using System;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Objects
{
    public class LootScript : MonoBehaviour
    {
        public Transform objectTransform;
        private float _delay = 0;
        private float _pastTime = 0;
        [SerializeField] [Range(0, 2)]
        private float when = 0.35f;
        private Vector3 _offset;
        [SerializeField] [Range(1, 3)] private float xRandomDistance = 1.4f;
        [SerializeField] [Range(1, 3)]private float yRandomDistance = 1.4f;
        [SerializeField] public int amount;
        [SerializeField] private string itemName;
        [SerializeField] private float speed;
        [SerializeField] private float magnetDistance;
        
        private static GameObject _prefabLoot;
        private bool _canBeCollected = false;
        private Transform _playerTransform;
        
        private void Awake()
        {
            _offset = new Vector3(Random.Range(-xRandomDistance, xRandomDistance), 
                Random.Range(-yRandomDistance, yRandomDistance), _offset.z);
        }

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (when >= _delay)
            {
                _pastTime = Time.deltaTime;
                objectTransform.position += _offset * Time.deltaTime;
                _delay += _pastTime;
            }

            if (!_canBeCollected)
            {
                if(_delay > when) _canBeCollected = true;
            }
            else
            {
                FlyToPlayer();
            }
        }

        private void FlyToPlayer()
        {
            var thisTransform = this.transform;
            var playerDistance = Vector2.Distance(thisTransform.position, _playerTransform.position);
            if (playerDistance > magnetDistance) return;
            
            if (playerDistance > 0.1f)
            {
                var playerDirection = (_playerTransform.position - thisTransform.position).normalized;
                thisTransform.position += playerDirection * speed;
            }
        }
        
        
        public void OnTriggerStay2D(Collider2D other)
        {
            if (!_canBeCollected) return;

            if (!other.CompareTag("Player") || other.isTrigger) return;
            var playerLevelController = other.gameObject.GetComponent<PlayerLevelController>();
            var playerCollectorController = other.gameObject.GetComponent<PlayerCollectorController>();
            playerCollectorController.PlayerCollect(amount);
            Destroy(gameObject);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, magnetDistance);
        }
    }
}
