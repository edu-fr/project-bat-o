using UnityEngine;

namespace Enemy
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
        public Material PreparingAttackMaterial;
        public Renderer Renderer;

        private void Start()
        {
            EnemyStateMachine = GetComponent<EnemyStateMachine>();
            // Current sprite material
            DefaultMaterial = Renderer.material;
            CurrentMaterial = DefaultMaterial;
        }

        public void SetToDefaultMaterial()
        {
            Renderer.material = DefaultMaterial;
        }

        public void SetMaterial(Material material)
        {
            Renderer.material = material;
        }
    }
}
