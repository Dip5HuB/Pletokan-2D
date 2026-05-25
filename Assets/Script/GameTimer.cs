using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer instance;

    [Header("Pengaturan Waktu")]
    public float waktuBermain = 180f; 
    public float waktuSisa;
    public bool isGameBerjalan = false;

    [Header("Referensi UI")]
    public TextMeshProUGUI teksTimer;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        waktuSisa = waktuBermain;
        isGameBerjalan = true;
        Time.timeScale = 1f;
        UpdateUITimer();
    }

    void Update()
    {
        if (isGameBerjalan && waktuSisa > 0)
        {
            waktuSisa -= Time.deltaTime;
            
            if (waktuSisa <= 0)
            {
                waktuSisa = 0;
                WaktuHabis();
            }
            
            UpdateUITimer();
        }
    }

    void UpdateUITimer()
    {
        if (teksTimer != null)
        {
            int menit = Mathf.FloorToInt(waktuSisa / 60);
            int detik = Mathf.FloorToInt(waktuSisa % 60);
            teksTimer.text = string.Format("{0:00}:{1:00}", menit, detik);
        }
    }

    public void WaktuHabis() 
    {
        if (!isGameBerjalan)
        {
            return; 
        }
        
        isGameBerjalan = false;
        
        // Lempar tugas evaluasi akhir ke GameplayManager
        if (GameplayManager.instance != null)
        {
            GameplayManager.instance.GameSelesai();
        }
    }

    public void HentikanWaktu()
    {
        isGameBerjalan = false;
    }
}