using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerSuvival : MonoBehaviour
{
    public Transform[] titikSpawn;

    public GameObject[] enemyNormal;
    public GameObject[] enemyElit;
    public GameObject enemyBoss;

    public int maxMusuhDiArena;
    public int intervalSpawn;

    private List<string> daftarElitSudahMuncul = new List<string>();    
    private bool bossSudahMuncul = false;

    void Start()
    {
        StartCoroutine(SpawnMusuhSecaraBergelombang());
    }

    IEnumerator SpawnMusuhSecaraBergelombang()
    {
        while (true)
        {
            if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan)
            {
                yield return null;
                continue;
            }

            GameObject[] musuhDiArena = GameObject.FindGameObjectsWithTag("Enemy");
            if (musuhDiArena.Length < maxMusuhDiArena)
            {
                LogikaPengaturanWave(GameTimer.instance.waktuSisa);
            }

            yield return new WaitForSeconds(intervalSpawn);
        }
    }

    void LogikaPengaturanWave(float waktuSisa)
    {
        Transform titik = titikSpawn[Random.Range(0, titikSpawn.Length)];
        if (waktuSisa <= 60f)
        {
            if (!bossSudahMuncul)
            {
                SpawnBoss(titik);
            }
            else
            {
                SpawnNormal(titik); // Aturan 4: Normal tidak terbatas
            }
        }
        else if (waktuSisa <= 120f)
        {
            if (!SpawnElit(titik))
            {
                SpawnNormal(titik); // Jika semua Elit sudah habis muncul, balik ke Normal
            }
        }
        else
        {
            SpawnNormal(titik);
        }
    }

    void SpawnNormal(Transform titik)
    {
        if (enemyNormal.Length == 0) return;
        GameObject pilihan = enemyNormal[Random.Range(0, enemyNormal.Length)];
        Instantiate(pilihan, titik.position, Quaternion.identity);
    }

    bool SpawnElit(Transform titik)
    {
        foreach (GameObject elit in enemyElit)
        {
            // Jika nama karakter ini belum ada di daftar "Sudah Muncul"
            if (!daftarElitSudahMuncul.Contains(elit.name))
            {
                Instantiate(elit, titik.position, Quaternion.identity);
                daftarElitSudahMuncul.Add(elit.name); // Kunci agar tidak muncul lagi
                Debug.Log("Elit Unik Muncul: " + elit.name);
                return true;
            }
        }
        return false; // Semua jenis elit sudah pernah muncul
    }

    void SpawnBoss(Transform titik)
    {
        if (enemyBoss != null)
        {
            Instantiate(enemyBoss, titik.position, Quaternion.identity);
            bossSudahMuncul = true;
            Debug.Log("Boss Utama Telah Masuk Arena!");
        }
    }
}
