using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class RoadblockController : MonoBehaviour
    {
        private BoxCollider2D BoxCollider2D;
        private LevelBuilder LevelBuilder;
        public bool IsBlocking;

        // Start is called before the first frame update
        void Start()
        {
            IsBlocking = true;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!IsBlocking)
                {    
                    LevelBuilder.CreateNewAreaObjects();
                }
            }
        }
    }
}
