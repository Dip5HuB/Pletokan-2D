using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusuhSpawner : MonoBehaviour
{
    [Header("Daftar Musuh")]
    public GameObject[] musuhPrefabs; // Array untuk menyimpan berbagai jenis musuh

    [Header("Titik Kemunculan")]
    public Transform[] titikSpawn; // Titik kiri dan kanan di luar layar

    [Header("Pengaturan Waktu")]
    public float waktuAntarSpawn = 4f; // Musuh muncul setiap 4 detik
    public int maksimalMusuh = 8;      // Batas maksimal musuh di layar agar tidak ngelag

    void Start()
    {
        // Memulai proses pencetakan musuh secara berulang
        StartCoroutine(MulaiSpawnOtomatis());
    }

    IEnumerator MulaiSpawnOtomatis()
    {
        // Looping tanpa henti selama game berjalan
        while (true)
        {
            // Mengecek berapa banyak musuh yang masih hidup di arena
            int jumlahMusuhDiArena = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // Hanya memunculkan musuh jika jumlahnya belum mencapai batas maksimal
            if (jumlahMusuhDiArena < maksimalMusuh)
            {
                SpawnSatuMusuh();
            }

            // Tunggu beberapa detik sebelum memunculkan musuh berikutnya
            yield return new WaitForSeconds(waktuAntarSpawn);
        }
    }

    void SpawnSatuMusuh()
    {
        // Mencegah error jika kolom belum diisi
        if (musuhPrefabs.Length == 0 || titikSpawn.Length == 0) return;

        // 1. Pilih jenis musuh secara acak dari daftar Prefab
        int indexMusuhRandom = Random.Range(0, musuhPrefabs.Length);
        GameObject musuhTerpilih = musuhPrefabs[indexMusuhRandom];

        // 2. Pilih titik kemunculan secara acak (Kiri atau Kanan)
        int indexTitikRandom = Random.Range(0, titikSpawn.Length);
        Transform titikTerpilih = titikSpawn[indexTitikRandom];

        // 3. Cetak musuh tersebut ke arena
        Instantiate(musuhTerpilih, titikTerpilih.position, Quaternion.identity);
    }
}