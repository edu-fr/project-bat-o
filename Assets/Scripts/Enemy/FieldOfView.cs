using System;
using UnityEngine;

namespace Enemy
{
    public class FieldOfView : MonoBehaviour
    {
        private Mesh Mesh;
        private Vector3 Origin;
        public float StartingAngle;
        public float FieldOfViewValue;
        public float ViewDistance;
        public int RayCount = 50;

        public GameObject Player;
        public bool PlayerIsOnFieldOfView;
        public EnemyMovementHandler EnemyMovementHandler { get; private set; }
        public LayerMask LayerMask;
        private float LastX;        
        private float LastY;        
        
        private void Awake()
        {
            Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = Mesh;
            EnemyMovementHandler = GetComponentInParent<EnemyMovementHandler>();
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

            var hitThisTime = false;
            for (int i = 0; i <= RayCount; i++)
            {
                Vector3 vertex;
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, UtilitiesClass.GetVectorFromAngle(angle), ViewDistance, LayerMask);
                if (raycastHit2D.collider == null)
                {
                    // No hit
                    vertex = UtilitiesClass.GetVectorFromAngle(angle) * ViewDistance;
                    vertex.z = transform.position.z;
                    if (i == RayCount && !hitThisTime)
                        PlayerIsOnFieldOfView = false;
                }
                else
                {
                    // Hit object
                    if (raycastHit2D.collider.gameObject.CompareTag("Player"))
                    {
                        hitThisTime = true;
                        Player = raycastHit2D.collider.gameObject;
                        PlayerIsOnFieldOfView = true;
                        EnemyMovementHandler.SetTargetPlayer(Player);
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

        public void SetOrigin(Vector3 origin)
        {
            this.Origin = origin;
        }

        public void SetAimDirection(EnemyAnimationController.FaceDirection currentFaceDirection)
        {
            StartingAngle = currentFaceDirection switch
            {
                EnemyAnimationController.FaceDirection.BackRight => 45 + FieldOfViewValue / 2f,
                EnemyAnimationController.FaceDirection.BackLeft => 135 + FieldOfViewValue / 2f,
                EnemyAnimationController.FaceDirection.FrontRight => -45 + FieldOfViewValue / 2f,
                EnemyAnimationController.FaceDirection.FrontLeft => -135 + FieldOfViewValue / 2f,
                _ => throw new ArgumentOutOfRangeException(nameof(currentFaceDirection), currentFaceDirection, null)
            };
        }

        public void SetFieldOfView(float fieldOfView)
        {
            this.FieldOfViewValue = fieldOfView;
        }

        public void SetViewDistance(float viewDistance)
        {
            this.ViewDistance = viewDistance;
        }

        public void SetMyMovementHandler(EnemyMovementHandler enemyMovementHandler)
        {
            this.EnemyMovementHandler = enemyMovementHandler;
        }
    }
}
