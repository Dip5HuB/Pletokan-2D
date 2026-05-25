using UnityEngine;

public class AwanBergerak : MonoBehaviour
{
    public float kecepatan = 0.2f; // Kecepatan pergerakan awan

    public float batasKiri = -12f; // Batas kiri layar
    public float batasKanan = 12f; // Batas kanan layar

    void Update()
    {
        // Menggerakkan awan ke kanan
        transform.Translate(Vector3.left * kecepatan * Time.deltaTime);

        // Jika awan keluar dari layar, kembalikan ke posisi awal
        if (transform.position.x < batasKiri)
        {
            transform.position = new Vector3(batasKanan, transform.position.y, transform.position.z);
        }
    }
}