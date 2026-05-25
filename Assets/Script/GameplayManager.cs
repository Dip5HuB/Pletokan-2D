using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    [Header("Identitas Level Ini")]
    public int nomorBabakIni = 1;
    public int nomorLevelIni = 1; 

    [Header("Referensi UI Game Akhir")]
    public GameObject panelSelesai; 
    public GameObject panelSetting;
    public TextMeshProUGUI teksSkorAkhir;
    
    [Header("Sistem Bintang")]
    public Image[] slotBintangUI;
    public Sprite bintangEmas;    
    public Sprite bintangHitam;   

    [Header("Referensi Player")]
    public PlayerController playerController;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    void Start()
    {
        if (panelSelesai != null)
        {
            panelSelesai.SetActive(false);
        }
        
        if (panelSetting != null)
        {
            panelSetting.SetActive(false);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PutarMusik(AudioManager.instance.musikGameplay);
        }
    }

    public void GameSelesai()
    {
        if (GameTimer.instance != null)
        {
            GameTimer.instance.HentikanWaktu();
        }

        int jumlahMisiBerhasil = 0;
        int skorAkhir = 0;

        // 1. Evaluasi Misi & Skor
        if (MisiManager.instance != null && playerController != null)
        {
            float persenDarah = (float)playerController.currentHealth / playerController.maxHealth * 100f;
            float waktuSisa = GameTimer.instance != null ? GameTimer.instance.waktuSisa : 0f;

            MisiManager.instance.EvaluasiMisiAkhir(persenDarah, waktuSisa);
            jumlahMisiBerhasil = MisiManager.instance.HitungMisiSelesai();
        }

        if (ScoreManager.instance != null)
        {
            skorAkhir = ScoreManager.instance.poinSkor;
        }

        // 2. Update UI Visual
        TampilkanBintang(jumlahMisiBerhasil);
        
        if (teksSkorAkhir != null)
        {
            teksSkorAkhir.text = skorAkhir.ToString();
        }
        
        if (panelSelesai != null)
        {
            panelSelesai.SetActive(true);
        }

        // 3. Audio Menang/Kalah
        if (AudioManager.instance != null)
        {
            AudioManager.instance.GantiMusikAkhir(jumlahMisiBerhasil > 0);
        }

        // 4. Konek Data ke Pusat Abadi
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.PerbaruiProgressLevel(nomorBabakIni, nomorLevelIni, jumlahMisiBerhasil, skorAkhir);
        }

        Time.timeScale = 0f; 
    }

    void TampilkanBintang(int jumlahMisiSelesai)
    {
        for (int i = 0; i < slotBintangUI.Length; i++)
        {
            if (slotBintangUI[i] != null)
            {
                slotBintangUI[i].gameObject.SetActive(true); 
                
                if (i < jumlahMisiSelesai)
                {
                    slotBintangUI[i].sprite = bintangEmas;
                }
                else
                {
                    slotBintangUI[i].sprite = bintangHitam;
                }
            }
        }
    }

    // --- FUNGSI TOMBOL UI ---

    public void KembaliKeMainMenu()
    {
        if (GameDataManager.instance != null)
        {
            // Tinggalkan jejak memori untuk dibaca oleh Scene Menu
            GameDataManager.instance.kembaliDariGame = true;
            GameDataManager.instance.babakTerakhir = nomorBabakIni; 
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); 
    }

    public void RestartGame()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ResetMusikKeAmbient();
        }
        
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void BukaPanelSetting()
    {
        if (panelSelesai.activeSelf)
        {
            return; 
        }
        
        if (panelSetting != null)
        {
            panelSetting.SetActive(true);
        }
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSuaraBerjalan(false);
        }
        
        Time.timeScale = 0f;
    }

    public void TutupPanelSetting()
    {
        if (panelSelesai.activeSelf)
        {
            return;
        }
        
        if (panelSetting != null)
        {
            panelSetting.SetActive(false);
        }
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSuaraBerjalan(true);
        }
        
        Time.timeScale = 1f;
    }
}