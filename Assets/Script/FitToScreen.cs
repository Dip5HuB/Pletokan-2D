using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Memastikan bahwa objek memiliki komponen SpriteRenderer
public class FitToScreen : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on " + gameObject.name);
            return;
        }

        transform.localScale = new Vector3(1, 1, 1); // Reset scale to default

        float spriteWidth = spriteRenderer.bounds.size.x; // Mendapatkan lebar sprite
        float spriteHeight = spriteRenderer.bounds.size.y; // Mendapatkan tinggi sprite

        float screenHeight = Camera.main.orthographicSize * 2f; // Menghitung tinggi layar
        float screenWidth = screenHeight * Camera.main.aspect; // Menghitung lebar layar

        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        transform.localScale = new Vector3(scaleX, scaleY, 1);// Mengatur skala objek agar sesuai dengan ukuran layar
    }
}