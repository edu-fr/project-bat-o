using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyMaterialManager : MonoBehaviour
    {
        private EnemyStateMachine EnemyStateMachine;
        
        public Material DefaultMaterial;
        public Material CurrentMaterial;
        public Material FlashMaterial;
        public Material BurnedMaterial;
        public Material FrozenMaterial;
        public Material ParalyzedMaterial;
        public Material TargetedMaterial;
        public Renderer Renderer;

        void Start()
        {
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            // Current sprite material
            DefaultMaterial = Renderer.material;
            CurrentMaterial = DefaultMaterial;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMaterial();
        }
        
        private void UpdateMaterial()
        {
            if (EnemyStateMachine.IsDying)
            {
                CurrentMaterial = DefaultMaterial;
            }
            else if (EnemyStateMachine.IsTargeted)
            {
                CurrentMaterial = TargetedMaterial;
            } 
            else if (EnemyStateMachine.IsOnFire)
            {
                CurrentMaterial = BurnedMaterial;
            }
            else if (EnemyStateMachine.IsFrozen)
            {
                CurrentMaterial = FrozenMaterial;
            }
            else if (EnemyStateMachine.IsParalyzed)
            {
                CurrentMaterial = ParalyzedMaterial;
            }
            else
            {
                CurrentMaterial = DefaultMaterial;
            }
            Renderer.material = CurrentMaterial;
        }
    }
}
