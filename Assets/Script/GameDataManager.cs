using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [Header("Data Slot Aktif Terkini")]
    public SaveSlotData slotAktif;
    public int nomorSlotTerpilih;
    public string folderPathAkun;

    // ---> MEMORI TRANSISE SCENE <---
    [HideInInspector] public bool kembaliDariGame = false;
    [HideInInspector] public int babakTerakhir = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Menjaga data tetap hidup saat pindah ke scene game pertempuran
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fungsi dipanggil saat player memenangkan level di scene game untuk menyimpan progress baru
    public void PerbaruiProgressLevel(int babak, int level, int bintangDapat, int skorDapat)
    {
        if (slotAktif == null) return;

        // Ambil list babak yang sesuai
        System.Collections.Generic.List<LevelData> targetBabak = (babak == 1) ? slotAktif.dataLevelBabak1 : slotAktif.dataLevelBabak2;
        int indeksLevel = level - 1;

        if (indeksLevel >= 0 && indeksLevel < targetBabak.Count)
        {
            // 1. Simpan rekor bintang tertinggi
            if (bintangDapat > targetBabak[indeksLevel].jumlahBintang)
                targetBabak[indeksLevel].jumlahBintang = bintangDapat;

            // 2. Simpan rekor skor tertinggi level tersebut
            if (skorDapat > targetBabak[indeksLevel].skorTertinggi)
                targetBabak[indeksLevel].skorTertinggi = skorDapat;

            // 3. Buka Level Selanjutnya (Mekanika Auto-Unlock Level)
            int indeksSelanjutnya = level;
            if (indeksSelanjutnya < targetBabak.Count)
            {
                targetBabak[indeksSelanjutnya].isTerbuka = true;
            }
            
            // Hitung ulang total akumulasi skor keseluruhan game
            HitungTotalSkorGlobal();

            SimpanDataKeDisk();
        }
    }

    void HitungTotalSkorGlobal()
    {
        int total = 0;
        foreach (var lvl in slotAktif.dataLevelBabak1) total += lvl.skorTertinggi;
        foreach (var lvl in slotAktif.dataLevelBabak2) total += lvl.skorTertinggi;
        slotAktif.totalSkorGlobal = total;
    }

    public void SimpanDataKeDisk()
    {
        try {
            string fullPath = Path.Combine(folderPathAkun, "slot" + nomorSlotTerpilih + ".dat");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(fullPath);
            bf.Serialize(file, slotAktif);
            file.Close();
            Debug.Log("Progress Slot " + nomorSlotTerpilih + " Berhasil Diperbarui.");
        } catch (System.Exception e) {
            Debug.LogError("Gagal menulis enkripsi biner: " + e.Message);
        }
    }

    public void ResetSesiPusat()
    {
        slotAktif = null;
        nomorSlotTerpilih = 0;
        folderPathAkun = "";
        kembaliDariGame = false;
        babakTerakhir = 1;
        Debug.Log("[Pusat Data] RAM Progress berhasil di-reset bersih!");
    }
}