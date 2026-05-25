using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public bool isTerbuka;
    public int jumlahBintang; // Nilai 0 sampai 3 bintang
    public int skorTertinggi;

    public LevelData()
    {
        isTerbuka = false;
        jumlahBintang = 0;
        skorTertinggi = 0;
    }
}

[Serializable]
public class SaveSlotData
{
    public string namaSlot;
    public string tanggalDibuat;
    public bool isKosong;
    public int totalSkorGlobal;

    // Menampung data level untuk Babak 1 dan Babak 2
    public List<LevelData> dataLevelBabak1 = new List<LevelData>();
    public List<LevelData> dataLevelBabak2 = new List<LevelData>();

    public SaveSlotData(string namaSlotBambu)
    {
        namaSlot = namaSlotBambu;
        tanggalDibuat = "-";
        isKosong = true;
        totalSkorGlobal = 0;

        // Generate otomatis 3 cetakan level kosong untuk Babak 1 & 2
        for (int i = 0; i < 3; i++)
        {
            LevelData lvl1 = new LevelData();
            LevelData lvl2 = new LevelData();
            
            // Level 1-1 (Indeks 0 Babak 1) otomatis terbuka di awal game baru
            if (i == 0) lvl1.isTerbuka = true; 

            dataLevelBabak1.Add(lvl1);
            dataLevelBabak2.Add(lvl2);
        }
    }
}