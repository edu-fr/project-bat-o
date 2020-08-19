using UnityEngine;

namespace Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        #region Variables

        private Mesh Mesh;
        private Vector3 Origin;
        private float StartingAngle;
        private float FieldOfViewValue = 120f;
        private float ViewDistance =5f;
        private int RayCount = 50;
        [SerializeField] private LayerMask LayerMask = 0;
        private EnemyBehavior MyEnemyBehavior;

        #endregion

        #region UnityCallbacks

        private void Awake()
        {
            Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = Mesh;
        }
        
        private void LateUpdate()
        {
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

                RaycastHit2D raycastHit2D = Physics2D.Raycast(Origin, UtilitiesClass.GetVectorFromAngle(angle), ViewDistance, LayerMask);
                if (raycastHit2D.collider == null)
                {
                    // No hit
                    vertex = Origin + UtilitiesClass.GetVectorFromAngle(angle) * ViewDistance;
                }
                else
                {
                    // Hit object
                    if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                    {
                        MyEnemyBehavior.SetTargetPlayer(raycastHit2D.collider.gameObject);
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
            this.MyEnemyBehavior = enemyBehavior;
        }
        #endregion
    }
}
