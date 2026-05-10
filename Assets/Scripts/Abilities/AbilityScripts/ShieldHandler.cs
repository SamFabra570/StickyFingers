using System;
using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    [SerializeField] private float stunTime;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();

            if (enemy != null)
            {
                enemy.Stun(stunTime);
            }
        }
    }
}
