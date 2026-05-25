using UnityEngine;

public class ItemPenyembuh : MonoBehaviour
{
    [Header("Pengaturan Item")]
    public int jumlahHeal = 10; // Berapa banyak darah yang ditambah
    public float waktuHilang = 10f; // Pisang akan hilang sendiri jika tidak diambil dalam 10 detik

    void Start()
    {
        // Hancurkan objek ini otomatis setelah waktu tertentu agar arena tidak penuh pisang
        Destroy(gameObject, waktuHilang);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang menyentuh pisang adalah Player
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.currentHealth += jumlahHeal;
                if (player.currentHealth > player.maxHealth)
                    player.currentHealth = player.maxHealth;
                
                player.UpdateHealthBarUI();
                Debug.Log("Nyam! Player makan pisang, nambah darah: " + jumlahHeal);
            }

            Destroy(gameObject);
        }
    }
}