using UnityEngine;

namespace Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        #region Variables

        private Mesh Mesh;
        private Vector3 Origin;
        public float StartingAngle;
        public float FieldOfViewValue;
        public float ViewDistance;
        public int RayCount = 50;
        public EnemyBehavior EnemyBehavior;
        public LayerMask LayerMask;

        #endregion

        #region UnityCallbacks

        private void Awake()
        {
            Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = Mesh;
        }
        
        private void LateUpdate()
        {
            SetOrigin(Vector3.zero);
            float angle = StartingAngle;
            float angleIncrease = FieldOfViewValue / RayCount;

            Vector3[] vertices = new Vector3[RayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[RayCount * 3];

            vertices[0] = Origin;

            int vertexIndex = 1;
            int triangleIndex = 0;

            for (int i = 0; i <= RayCount; i++)
            {
                Vector3 vertex;
                Debug.Log(ViewDistance);
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, UtilitiesClass.GetVectorFromAngle(angle), ViewDistance, LayerMask);
                if (raycastHit2D.collider == null)
                {
                    // No hit
                    vertex = UtilitiesClass.GetVectorFromAngle(angle) * ViewDistance;
                    vertex.z = transform.position.z; 
                }
                else
                {
                    // Hit object
                    if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                    {
                        EnemyBehavior.SetTargetPlayer(raycastHit2D.collider.gameObject);
                    }
                    vertex = (Vector3) raycastHit2D.point - transform.position;
                    vertex.z = transform.position.z;
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


            Mesh.vertices = vertices;
            Mesh.uv = uv;
            Mesh.triangles = triangles;
        }
        #endregion

        #region Auxiliar methods
    

        public void SetOrigin(Vector3 origin)
        {
            this.Origin = origin;
        }

        public void SetAimDirection(Vector3 aimDirection)
        {
            StartingAngle = UtilitiesClass.GetAngleFromVectorFloat(aimDirection) + FieldOfViewValue / 2f;
        }

        public void SetFieldOfView(float fieldOfView)
        {
            this.FieldOfViewValue = fieldOfView;
        }

        public void SetViewDistance(float viewDistance)
        {
            this.ViewDistance = viewDistance;
        }

        public void SetMyEnemyBehavior(EnemyBehavior enemyBehavior)
        {
            this.EnemyBehavior = enemyBehavior;
        }
        #endregion
    }
}
