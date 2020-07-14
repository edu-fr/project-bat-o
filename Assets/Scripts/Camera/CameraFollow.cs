using UnityEngine;

namespace Camera
{
    public class CameraFollow : MonoBehaviour
    {
        #region Variables:
        private Transform Target;
        private float SmoothSpeed = 7f;

        [SerializeField]
        private float LeftLimit = 0;
        [SerializeField]
        private float RightLimit = 0;
        [SerializeField]
        private float BottomLimit = 0;
        [SerializeField]
        private float UpperLimit = 0;

        #endregion

        #region Unity Callbacks
        void Start()
        {
            SetPlayerAsTarget();
        }

        void LateUpdate()
        {
            if(Target)
            {
                Vector3 desiredPosition = new Vector3(
                    Mathf.Clamp(Target.transform.position.x, LeftLimit, RightLimit),
                    Mathf.Clamp(Target.transform.position.y, BottomLimit, UpperLimit),
                    -10);
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);

                transform.position = smoothedPosition;

            }
        }
        #endregion

        #region Auxiliar Methods
        public void SetTarget(Transform target)
        {
            this.Target = target;
        }

        public Transform GetTarget()
        {
            return this.Target; 
        }

        public void SetPlayerAsTarget()
        {
            Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        #endregion
    }
}
