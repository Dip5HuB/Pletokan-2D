using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Pengaturan spawn")]
    public GameObject pisangPrefab;
    public float minSpawnWaktu;
    public float maxSpawnWaktu;


    public BoxCollider2D areaSpawn; // Area di mana pisang akan muncul

    void Start()
    {
        StartCoroutine(SpawnItemPenyembuh());
    }

    void Update()
    {
    }

    IEnumerator SpawnItemPenyembuh()
    {
        while (true)
        {
            float waktuTunggu = Random.Range(minSpawnWaktu, maxSpawnWaktu);
            yield return new WaitForSeconds(waktuTunggu);

            // Hanya spawn jika game sedang berjalan
            if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan)
            {
                continue;
            }

            SpawnPisang();
        }
    }

    void SpawnPisang()
    {
        Bounds batasArea = areaSpawn.bounds;

        float spawnX = Random.Range(batasArea.min.x, batasArea.max.x);
        float spawnY = Random.Range(batasArea.min.y, batasArea.max.y);
        Vector2 posisiSpawn = new Vector2(spawnX, spawnY);

        Instantiate(pisangPrefab, posisiSpawn, Quaternion.identity);
    }
}
