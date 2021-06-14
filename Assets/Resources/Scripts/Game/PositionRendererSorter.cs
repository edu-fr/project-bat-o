using System;
using UnityEngine;

namespace Game
{
    public class PositionRendererSorter : MonoBehaviour
    {
        private Renderer MyRenderer;
        [SerializeField]
        private float SortingOrderBase = 5000;
        [SerializeField]
        private float Offset = 0;
        [SerializeField]
        private bool RunOnlyOnce = false;
        
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
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube( new Vector3(transform.position.x,transform.position.y - Offset, 0), new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}
