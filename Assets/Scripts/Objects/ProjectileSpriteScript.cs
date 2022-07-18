using UnityEngine;

namespace Objects
{
    public class ProjectileSpriteScript : MonoBehaviour
    {
        public void DestroyParent()
        {
            GetComponentInParent<ProjectileScript>().DestroySelf();
        }
    }
}
