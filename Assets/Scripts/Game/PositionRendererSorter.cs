using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    #region Variables
    private Renderer myRenderer;
    [SerializeField]
    private float sortingOrderBase = 5000;
    [SerializeField]
    private float offset = 0;
    [SerializeField]
    private bool runOnlyOnce = false;
    #endregion


    #region Unity Callbacks
    // Start is called before the first frame update
    private void Awake()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
        if(runOnlyOnce)
        {
            Destroy(this);
        }
    }

    #endregion

    #region Auxiliar Methods

    #endregion
}
