using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private HealthBar healthBar = null;

    [SerializeField] private int currentHP = 100;
    [SerializeField] private int maxHP = 100;
    #endregion

    private void Start()
    {
        if(healthBar != null)
            healthBar.setMaxHealth(maxHP);
    }

    // Update is called once per frame
    private void Update()
    {
        // Update HealthBar
        if(healthBar != null)
            healthBar.setHealth(currentHP);

        // Avoid overkill
        if(currentHP <= 0)
        {
            currentHP = 0;
        }
     
        // Avoid overheal
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void HurtObject(float damage)
    {
        currentHP -= (int)damage;
    }

    public int getCurrentHP()
    {
        return currentHP;
    }
}
