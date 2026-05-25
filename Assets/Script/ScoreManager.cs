using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton instance

    [Header("UI Skor")]
    public TextMeshProUGUI skorText; // Komponen UI untuk menampilkan skor

    public int poinSkor = 0; // Variabel untuk menyimpan skor saat ini

    void Awake()
    {
        // Implementasi Singleton
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Jangan hancurkan objek ini saat ganti scene
        }
        else
        {
            Destroy(gameObject); // Hancurkan objek duplikat jika sudah ada instance
        }
    }

    void Start()
    {
        UpdateSkorUI(); // Tampilkan skor awal di UI
    }

    public void TambahSkor(int jumlah)
    {
        poinSkor += jumlah; // Tambahkan jumlah ke skor saat ini
        UpdateSkorUI(); // Perbarui tampilan skor di UI

        if (MisiManager.instance !=null){
            MisiManager.instance.LaporSkor(jumlah);
        }
    }

    void UpdateSkorUI()
    {
        if (skorText != null)
        {
            skorText.text = poinSkor.ToString(); // Perbarui teks skor
        }
    }
}