using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Objects
{
    public class ProjectileSpriteScript : MonoBehaviour
    {
        public void DestroyParent()
        {
            GetComponentInParent<ProjectileScript>().DestroySelf();
        }
    }
}
