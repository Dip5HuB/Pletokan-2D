using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMisiSlot : MonoBehaviour
{
    [Header("Komponen UI")]
    public TextMeshProUGUI teksDeskripsi; // Tulisan "Kalahkan semua musuh"
    public TextMeshProUGUI teksProgresAngka; // Tulisan "1/10"
    public Image ikonProgresLingkaran; // Gambar lingkaran kosong

    [Header("Pengaturan Warna")]
    public Color warnaGagal = Color.red;
    public Color warnaBerhasil = Color.green;
    public Color warnaTeksAman = Color.black;

    private MisiManager.ProgresMisiAktif misiTerkait;

    // Fungsi ini dipanggil MisiManager saat awal babak untuk menyetting baris ini
    public void SetupSlot(MisiManager.ProgresMisiAktif misiBaru)
    {
        misiTerkait = misiBaru;

        if (misiTerkait.dataMisi.deskripsiLocalized != null)
        {
            teksDeskripsi.text = misiTerkait.dataMisi.deskripsiLocalized.GetLocalizedString();
        }
        else
        {
            teksDeskripsi.text = "Missing Description Key!";
        }

        // Atur tampilan berdasarkan kategori misi
        if (misiTerkait.dataMisi.kategoriMisi == KategoriMisi.IndikatorUI)
        {
            // Tampilkan lingkaran, sembunyikan angka
            ikonProgresLingkaran.gameObject.SetActive(true);
            teksProgresAngka.gameObject.SetActive(false);
        }
        else
        {
            // Tampilkan angka, sembunyikan lingkaran
            ikonProgresLingkaran.gameObject.SetActive(false);
            teksProgresAngka.gameObject.SetActive(true);
        }

        UpdateTampilan();
    }

    // Fungsi ini akan dipanggil setiap kali ada musuh mati / progres bertambah
    public void UpdateTampilan()
    {
        if (misiTerkait == null) return;

        if (misiTerkait.dataMisi.deskripsiLocalized != null && !misiTerkait.dataMisi.deskripsiLocalized.IsEmpty)
        {
            teksDeskripsi.text = misiTerkait.dataMisi.deskripsiLocalized.GetLocalizedString();
        }

        // Tipe 2: Indikator UI (Lingkaran)
        if (misiTerkait.dataMisi.kategoriMisi == KategoriMisi.IndikatorUI)
        {
            // ---> Pengecekan Khusus Batas Darah <---
            if (misiTerkait.dataMisi.tipeMisi == TipeMisi.BatasDarah) 
            {
                // Cek persentase darah yang dikirim oleh PlayerController
                if (misiTerkait.currentHealthPercentage < misiTerkait.dataMisi.targetNilai) 
                {
                    ikonProgresLingkaran.color = warnaGagal; // Merah saat di bawah batas (Sekarat)
                } 
                else 
                {
                    ikonProgresLingkaran.color = warnaBerhasil; // Hijau kembali saat aman (Darah banyak)
                }
                
                // Return agar tidak menjalankan logika warna bawaan di bawahnya
                return; 
            }
            // ------------------------------------------------------------------

            if (misiTerkait.dataMisi.uiMulaiDariHijau) 
            {
                // Mulai hijau, kalau gagal jadi merah (Misi Bertahan)
                if (misiTerkait.isGagal) ikonProgresLingkaran.color = warnaGagal;
                else ikonProgresLingkaran.color = warnaBerhasil;
            } 
            else 
            {
                // Mulai merah, kalau berhasil jadi hijau (Misi Tunggu Waktu)
                if (misiTerkait.isBerhasil) ikonProgresLingkaran.color = warnaBerhasil;
                else ikonProgresLingkaran.color = warnaGagal;
            }
        }
        // Tipe 1 & 3: Angka (Teks)
        else
        {
            teksProgresAngka.text = Mathf.FloorToInt(misiTerkait.nilaiSaatIni) + " / " + Mathf.FloorToInt(misiTerkait.dataMisi.targetNilai);

            // Tipe 1: Minimal angka (Awal merah, lulus jadi hitam)
            if (misiTerkait.dataMisi.kategoriMisi == KategoriMisi.MinimalAngka)
            {
                if (misiTerkait.isBerhasil) teksProgresAngka.color = warnaTeksAman;
                else teksProgresAngka.color = warnaGagal;
            }
            // Tipe 3: Maksimal angka (Awal hitam, lewat batas jadi merah)
            else if (misiTerkait.dataMisi.kategoriMisi == KategoriMisi.MaksimalAngka)
            {
                if (misiTerkait.isGagal) teksProgresAngka.color = warnaGagal;
                else teksProgresAngka.color = warnaTeksAman;
            }
        }
    }
}