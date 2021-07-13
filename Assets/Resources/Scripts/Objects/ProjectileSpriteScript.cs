using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpriteScript : MonoBehaviour
{
    public void DestroyParent()
    {
        Destroy(this.transform.parent);
    }
}
