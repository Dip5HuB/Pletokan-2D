using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MisiManager : MonoBehaviour {
    
    public static MisiManager instance;
    public List<DataMisi> semuaMisiTersedia;

    public class ProgresMisiAktif {
        public DataMisi dataMisi;
        public float nilaiSaatIni;
        public float currentHealthPercentage;
        public bool isBerhasil;
        public bool isGagal;
    }

    public List<ProgresMisiAktif> daftarMisiAktif = new List<ProgresMisiAktif>();

    [Header("Referensi UI Misi")]
    public UIMisiSlot[] slotUIMisi;
    public int jumlahMisi = 3;

    void Awake() 
    { 
        if (instance == null) instance = this; 
    }

    void Start() 
    { 
        PilihMisiAcak(); 

        LocalizationSettings.SelectedLocaleChanged += SaatBahasaBerubah;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= SaatBahasaBerubah;
    }

    void SaatBahasaBerubah(Locale locale)
    {
        RefreshSemuaUI();
    }

    void Update()
    {
        // Berhenti berjalan jika game tidak aktif
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;

        foreach (var progres in daftarMisiAktif)
        {
            if (progres.isBerhasil || progres.isGagal) continue;

            if (progres.dataMisi.kategoriMisi == KategoriMisi.IndikatorUI)
            {
                if (progres.dataMisi.tipeMisi == TipeMisi.BatasDarah) continue;
                
                // Misi Bertahan (Target 0) tidak menggunakan timer, hanya menunggu gagal/waktu habis
                if (progres.dataMisi.targetNilai > 0)
                {
                    progres.nilaiSaatIni += Time.deltaTime;
                    if (progres.nilaiSaatIni >= progres.dataMisi.targetNilai)
                    {
                        progres.isBerhasil = true;
                    }
                }
                RefreshSemuaUI();
            }
        }
    }

    void PilihMisiAcak()
    {
        daftarMisiAktif.Clear();
        List<DataMisi> keranjangAcak = new List<DataMisi>(semuaMisiTersedia);

        for (int i = 0; i < jumlahMisi; i++)
        {
            if (keranjangAcak.Count == 0) break;

            int indeksAcak = Random.Range(0, keranjangAcak.Count);
            ProgresMisiAktif progresBaru = new ProgresMisiAktif();
            progresBaru.dataMisi = keranjangAcak[indeksAcak];
            progresBaru.nilaiSaatIni = 0;
            progresBaru.currentHealthPercentage = 100f;
            progresBaru.isBerhasil = false;
            progresBaru.isGagal = false;

            daftarMisiAktif.Add(progresBaru);
            keranjangAcak.RemoveAt(indeksAcak);
        }
        foreach (var misi in daftarMisiAktif)
        {
            for (int i = 0; i < daftarMisiAktif.Count; i++)
            {
                if (i < slotUIMisi.Length) slotUIMisi[i].SetupSlot(daftarMisiAktif[i]);
            }
        }
    }

    public void LaporKondisiDarahRealTime(float persenDarah)
    {
        foreach (var progres in daftarMisiAktif)
        {
            if (progres.dataMisi.tipeMisi == TipeMisi.BatasDarah)
            {
                progres.currentHealthPercentage = persenDarah;
            }
        }
        RefreshSemuaUI();
    }

    public void LaporTindakan(TipeMisi tipeAksi, float nilaiTambahan = 1f)
    {
        foreach(var progres in daftarMisiAktif){
            // Jika misi sudah selesai (berhasil/gagal), skip pengecekan untuk misi itu
            // if (progres.isBerhasil || progres.isGagal) continue;

            if (progres.isGagal) continue;

            if (progres.dataMisi.tipeMisi == tipeAksi)
            {
                progres.nilaiSaatIni += nilaiTambahan;
                
                if (progres.dataMisi.kategoriMisi == KategoriMisi.MinimalAngka) {
                    if (progres.nilaiSaatIni >= progres.dataMisi.targetNilai) progres.isBerhasil = true;
                }
                else if (progres.dataMisi.kategoriMisi == KategoriMisi.MaksimalAngka) {
                    if (progres.nilaiSaatIni > progres.dataMisi.targetNilai) progres.isGagal = true; 
                }
            }
        }   
        RefreshSemuaUI();
    }

    public void GagalkanMisi(TipeMisi tipYangBatal)
    {
        foreach(var progres in daftarMisiAktif){
            if (progres.isBerhasil || progres.isGagal) continue;

            if (progres.dataMisi.tipeMisi == tipYangBatal) 
            {
                if (progres.dataMisi.uiMulaiDariHijau)
                {
                    progres.isGagal = true; // Gagal permanen, indikator jadi merah selamanya
                    Debug.Log("Misi " + tipYangBatal + " Gagal Total!");
                }
                else
                {
                    progres.nilaiSaatIni = 0; // Reset progres, indikator kembali ke merah awal
                    Debug.Log("Misi " + tipYangBatal + " Progres Reset!");
                }
            }
        
        }
        RefreshSemuaUI();
    }

    public void EvaluasiMisiAkhir(float persenDarahPlayer, float sisaWaktu)
    {
        foreach (var progres in daftarMisiAktif)
        {
            if (progres.isBerhasil || progres.isGagal) continue;

            // 1. Maksimal Angka: Jika belum gagal sampai akhir, berarti berhasil
            if (progres.dataMisi.kategoriMisi == KategoriMisi.MaksimalAngka)
            {
                progres.isBerhasil = true;
            }
            // 2. Indikator UI: Jika tipe bertahan selamanya (Target 0) dan belum gagal, berarti berhasil!
            else if (progres.dataMisi.kategoriMisi == KategoriMisi.IndikatorUI)
            {
                progres.isBerhasil = true;
            }
            // 3. Batas Darah (Mengecek sisa darah di akhir)
            else if (progres.dataMisi.tipeMisi == TipeMisi.BatasDarah && persenDarahPlayer >= progres.dataMisi.targetNilai)
            {
                progres.isBerhasil = true;
            }
            // 4. Batas Waktu
            else if (progres.dataMisi.tipeMisi == TipeMisi.BatasWaktu && sisaWaktu >= progres.dataMisi.targetNilai)
            {
                progres.isBerhasil = true;
            }
        }
        RefreshSemuaUI();
    }

    // Fungsi untuk menghitung berapa misi yang sukses
    public int HitungMisiSelesai()
    {
    int jumlahBerhasil = 0;
    foreach (var misi in daftarMisiAktif)
    {
        // Pastikan variabel 'isBerhasil' adalah yang kita cek
        if (misi.isBerhasil) 
        {
            jumlahBerhasil++;
        }
    }
    Debug.Log("Total misi yang sukses: " + jumlahBerhasil);
    return jumlahBerhasil;
    }

    public void LaporMusuhMati(bool isElit, bool isBoss)
    {
        LaporTindakan(TipeMisi.EliminasiTotal);
        if (isElit) LaporTindakan(TipeMisi.EliminasiElit);
        if (isBoss) LaporTindakan(TipeMisi.EliminasiBoss);
        if (!isElit && !isBoss) LaporTindakan(TipeMisi.EliminasiNormal);
    }

    public void LaporSkor (int point) { LaporTindakan (TipeMisi.KumpulkanSkor, point); }
    public void RefreshSemuaUI() { foreach (var slot in slotUIMisi) if (slot != null) slot.UpdateTampilan(); }
}