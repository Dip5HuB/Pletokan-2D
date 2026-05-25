using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class LanguageSettingsUI : MonoBehaviour
{
    [Header("UI Bahasa Components")]
    public TextMeshProUGUI txtIndikatorBahasa; 
    
    public GameObject btnPrev;                     
    public GameObject btnNext;                     

    [Header("Daftar Font Sesuai Bahasa")]
    public TMP_FontAsset[] fontPerBahasa;

    private int indeksBahasa = 0;
    private bool isSistemSiap = false;

    void Start()
    {
        // Hubungkan event klik ke tombol
        if (btnPrev != null) btnPrev.GetComponent<Button>().onClick.AddListener(GantiBahasaSebelumnya);
        if (btnNext != null) btnNext.GetComponent<Button>().onClick.AddListener(GantiBahasaSelanjutnya);
        
        indeksBahasa = PlayerPrefs.GetInt("PilihanBahasaSistem", 0);
        isSistemSiap = true;

        // Terapkan semuanya saat game pertama kali dimulai
        ProsesPerubahanBahasa(true); 
    }

    void GantiBahasaSelanjutnya()
    {
        int totalBahasa = LocalizationSettings.AvailableLocales.Locales.Count;
        if (indeksBahasa < totalBahasa - 1)
        {
            indeksBahasa++;
            ProsesPerubahanBahasa(false);
        }
    }

    void GantiBahasaSebelumnya()
    {
        if (indeksBahasa > 0)
        {
            indeksBahasa--;
            ProsesPerubahanBahasa(false);
        }
    }

    void ProsesPerubahanBahasa(bool isAwalMulai)
    {
        if (!isSistemSiap) return;

        TerapkanFontSecaraGlobal(indeksBahasa);

        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (indeksBahasa >= 0 && indeksBahasa < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[indeksBahasa];
            if (!isAwalMulai) 
            {
                PlayerPrefs.SetInt("PilihanBahasaSistem", indeksBahasa);
                PlayerPrefs.Save();
            }
        }

        UpdateVisualNavigasi();
    }

    void TerapkanFontSecaraGlobal(int indeks)
    {
        if (fontPerBahasa == null || indeks < 0 || indeks >= fontPerBahasa.Length) return;

        TMP_FontAsset fontTerpilih = fontPerBahasa[indeks];

        if (fontTerpilih != null)
        {
            // Ambil semua teks dan ganti font-nya seketika
            TextMeshProUGUI[] semuaTeksDiScene = FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI teks in semuaTeksDiScene)
            {
                teks.font = fontTerpilih;
            }
        }
    }

    void UpdateVisualNavigasi()
    {
        int totalBahasa = LocalizationSettings.AvailableLocales.Locales.Count;

        // Hilangkan tombol Prev jika di indeks awal, hilangkan Next jika di akhir
        if (btnPrev != null) btnPrev.SetActive(indeksBahasa > 0);
        if (btnNext != null) btnNext.SetActive(indeksBahasa < totalBahasa - 1);

        // Setel tulisan indikator
        if (txtIndikatorBahasa != null)
        {
            if (indeksBahasa == 0) txtIndikatorBahasa.text = "B. Indonesia";
            else if (indeksBahasa == 1) txtIndikatorBahasa.text = "日本語";
        }
    }
}