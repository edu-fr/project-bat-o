using UnityEngine;

namespace Game
{
    public class PositionRendererSorter : MonoBehaviour
    {
        #region Variables
        private Renderer MyRenderer;
        [SerializeField]
        private float SortingOrderBase = 5000;
        [SerializeField]
        private float Offset = 0;
        [SerializeField]
        private bool RunOnlyOnce = false;
        #endregion


        #region Unity Callbacks
        // Start is called before the first frame update
        private void Awake()
        {
            MyRenderer = gameObject.GetComponent<Renderer>();
        }

        private void LateUpdate()
        {
            MyRenderer.sortingOrder = (int)(SortingOrderBase - transform.position.y - Offset);
            if(RunOnlyOnce)
            {
                Destroy(this);
            }
        }

        #endregion

        #region Auxiliar Methods

        #endregion
    }
}
