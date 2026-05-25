using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeluruDamage : MonoBehaviour
{
    public int damage = 15; // Besarnya damage setiap satu peluru

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            Musuh musuhYangDitembak = hitInfo.GetComponent<Musuh>();
            if (musuhYangDitembak != null)
            {
                musuhYangDitembak.TakeDamage(damage); // Memanggil fungsi TakeDamage pada musuh yang terkena peluru
                Debug.Log("Musuh terkena peluru! Damage: " + damage);
            }
            Destroy(gameObject); // Hancurkan peluru setelah menabrak musuh
        }   

    }
}