using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeluruDamageMusuh : MonoBehaviour
{
    public int damage = 15; // Besarnya damage setiap satu peluru

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            PlayerController playerYangDitembak = hitInfo.GetComponent<PlayerController>();
        if (playerYangDitembak != null)
        {
            playerYangDitembak.TakeDamage(damage); // Memanggil fungsi TakeDamage pada player yang terkena peluru
            Debug.Log("Player terkena peluru! Damage: " + damage);
        }

        Destroy(gameObject); // Hancurkan peluru setelah menabrak musuh
        }   
    }
}