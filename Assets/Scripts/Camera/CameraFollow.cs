using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables:
    private Transform target;
    private float smoothSpeed = 7f;

    [SerializeField]
    private float leftLimit = 0;
    [SerializeField]
    private float rightLimit = 0;
    [SerializeField]
    private float bottomLimit = 0;
    [SerializeField]
    private float upperLimit = 0;

    #endregion

    #region Unity Callbacks
    void Start()
    {
        setPlayerAsTarget();
    }

    void LateUpdate()
    {
        if(target)
        {
            Vector3 desiredPosition = new Vector3(
                Mathf.Clamp(target.transform.position.x, leftLimit, rightLimit),
                Mathf.Clamp(target.transform.position.y, bottomLimit, upperLimit),
                -10);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;

        }
    }
    #endregion

    #region Auxiliar Methods
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public Transform GetTarget()
    {
        return this.target; 
    }

    public void setPlayerAsTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    #endregion
}
