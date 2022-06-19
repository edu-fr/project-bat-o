using UnityEngine;

namespace Game
{
    public class RoadblockController : MonoBehaviour
    {
        private BoxCollider2D BoxCollider2D;
        private LevelManager LevelManager;
        public bool IsBlocking { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            IsBlocking = true;
            BoxCollider2D = GetComponent<BoxCollider2D>();
            LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!IsBlocking)
                {    
                    LevelManager.GoToNextLevel();
                }
            }
        }
    }
}
