using Resources.Scripts.Enemy;
using UnityEngine;

public class ProjectileSpriteScript : MonoBehaviour
{
    public void DestroyParent()
    {
        GetComponentInParent<ProjectileScript>().DestroySelf();
    }
}
