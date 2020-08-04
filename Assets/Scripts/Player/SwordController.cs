using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private PlayerAttackManager PlayerAttackManager;
    private BoxCollider2D BoxCollider2D;
    
    // Start is called before the first frame update
    void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        PlayerAttackManager = GetComponentInParent<PlayerAttackManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Colidiu com " + other.gameObject.name);
            PlayerAttackManager.VerifyAttackCollision(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Parou de colidir com " + other.gameObject.name);
        }
    }
}
