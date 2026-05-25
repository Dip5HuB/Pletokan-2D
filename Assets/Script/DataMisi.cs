using UnityEngine;
using UnityEngine.Localization; 

public enum TipeMisi
{
    EliminasiTotal, EliminasiElit, EliminasiNormal, EliminasiBoss, 
    TanpaLuka, TanpaTembak, KumpulkanSkor, KumpulkanBenda, 
    GunakanPenyembuh, BatasPenyembuh, TanpaPenyembuh, 
    GunakanPeluruEmas, BatasDarah, BatasWaktu
}

public enum KategoriMisi { MinimalAngka, IndikatorUI, MaksimalAngka }

[CreateAssetMenu(fileName = "DataMisi", menuName = "GamePletokan/DataMisi")]
public class DataMisi : ScriptableObject
{
    [Header("Informasi UI Lokalisasi")]

    public LocalizedString deskripsiLocalized;

    [Header("Aturan Sistem")]
    public TipeMisi tipeMisi;
    public KategoriMisi kategoriMisi;
    
    [Tooltip("Centang jika UI awalnya Hijau lalu Merah jika gagal (Misal: Batas Darah). Kosongkan jika awalnya Merah lalu Hijau jika berhasil (Misal: Tanpa Tembak 5 Detik).")]
    public bool uiMulaiDariHijau;

    [Tooltip("Bisa berupa jumlah bunuh, jumlah skor, atau jumlah detik (untuk waktu)")]
    public float targetNilai;
}