using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;

public class FieldOfView : MonoBehaviour
{
    #region Variables

    private Mesh mesh;
    private Vector3 origin;
    private float startingAngle;
    private float fieldOfView = 90f;
    private float viewDistance =5f;
    private int rayCount = 50;
    [SerializeField] private LayerMask layerMask = 0;
    private EnemyBehavior myEnemyBehavior;

    #endregion

    #region UnityCallbacks
    
    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        float angle = startingAngle;
        float angleIncrease = fieldOfView / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, Utilities.GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                // No hit
                vertex = origin + Utilities.GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // Hit object
                if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                {
                    myEnemyBehavior.setTargetPlayer(raycastHit2D.collider.gameObject);
                }
                vertex = raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    #endregion

    #region Auxiliar methods
    

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = Utilities.GetAngleFromVectorFloat(aimDirection) + fieldOfView / 2f;
    }

    public void setFieldOfView(float fieldOfView)
    {
        this.fieldOfView = fieldOfView;
    }

    public void setViewDistance(float viewDistance)
    {
        this.viewDistance = viewDistance;
    }

    public void setMyEnemyBehavior(EnemyBehavior enemyBehavior)
    {
        this.myEnemyBehavior = enemyBehavior;
    }
    #endregion
}
