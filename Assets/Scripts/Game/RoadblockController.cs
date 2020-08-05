using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadblockController : MonoBehaviour
{
    private BoxCollider2D BoxCollider2D;
    private GameManager GameManager;
    private bool IsBlocking { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        GameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!IsBlocking)
            {    
                GameManager.GoToNextLevel();
            }
        }
    }
}
