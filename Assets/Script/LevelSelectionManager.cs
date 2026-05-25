using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class LevelSelectionManager : MonoBehaviour
{
    public static LevelSelectionManager instance;

    [Header("Kamus Teks Dinamis Pemilihan Level")]
    public LocalizedString judulPilihBabak;
    public LocalizedString judulPilihLevel;
    public LocalizedString teksTotalSkor;
    public LocalizedString notifBelumTersedia;
    
    [Header("Referensi Sistem Menu Babak & Level")]
    public GameObject panelBabak;
    public GameObject subPanelPilihBabak;
    public GameObject subPanelPilihLevel;
    public TextMeshProUGUI txtJudulPanelBabak;
    
    [Header("Navigasi & Bypass")]
    public TextMeshProUGUI textNotifLevelSistem;

    [Header("UI Tampilan Level Dinamis")]
    public TextMeshProUGUI textSkorKumulatif;
    public Button[] tombolLevel;
    public GameObject[] containerBintangLevel;
    public Sprite spriteBintangEmas;
    public Sprite spriteBintangHitam;

    private int babakAktifTerpilih = 1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (panelBabak != null) 
        {
            panelBabak.SetActive(false);
        }

        if (textNotifLevelSistem != null) 
        {
            textNotifLevelSistem.text = "";
        }

        // --- BYPASS MENU SAAT KEMBALI DARI GAME ---
        if (GameDataManager.instance != null && GameDataManager.instance.kembaliDariGame)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PutarMusik(AudioManager.instance.musikMenuUtama);
            }
            
            if (AccountUIManager.instance != null)
            {
                if (AccountUIManager.instance.panelAkun != null) AccountUIManager.instance.panelAkun.SetActive(false);
                if (AccountUIManager.instance.uiHome != null) AccountUIManager.instance.uiHome.SetActive(true);
            }

            // Menyuruh SaveSlotManager menyembunyikan panelnya
            if (SaveSlotManager.instance != null)
            {
                SaveSlotManager.instance.SembunyikanPanelSlot();
            }

            BukaMenuBabakAwal();
            PilihBabakKeLevel(GameDataManager.instance.babakTerakhir); 

            GameDataManager.instance.kembaliDariGame = false;
        }
    }

    public void BukaMenuBabakAwal()
    {
        if (panelBabak != null) panelBabak.SetActive(true); 
        if (subPanelPilihBabak != null) subPanelPilihBabak.SetActive(true); 
        if (subPanelPilihLevel != null) subPanelPilihLevel.SetActive(false); 

        if (txtJudulPanelBabak != null && judulPilihBabak != null) 
        {
            txtJudulPanelBabak.text = judulPilihBabak.GetLocalizedString(); 
        }
    }

    public void PilihBabakKeLevel(int nomorBabak)
    {
        babakAktifTerpilih = nomorBabak;
        
        subPanelPilihBabak.SetActive(false);
        subPanelPilihLevel.SetActive(true);

        if (txtJudulPanelBabak != null && judulPilihLevel != null) 
        {
            txtJudulPanelBabak.text = judulPilihLevel.GetLocalizedString(); 
        }
        
        RefreshTampilanOpsiLevel();
    }

    void RefreshTampilanOpsiLevel()
    {
        if (GameDataManager.instance == null || GameDataManager.instance.slotAktif == null) 
        {
            return;
        }
        
        SaveSlotData slot = GameDataManager.instance.slotAktif;
        System.Collections.Generic.List<LevelData> listLevel = (babakAktifTerpilih == 1) ? slot.dataLevelBabak1 : slot.dataLevelBabak2;
        int totalSkorBabakIni = 0;

        for (int i = 0; i < tombolLevel.Length; i++)
        {
            if (i >= listLevel.Count) break;
            
            LevelData dataLvl = listLevel[i];
            totalSkorBabakIni += dataLvl.skorTertinggi;
            tombolLevel[i].interactable = dataLvl.isTerbuka;

            if (i < containerBintangLevel.Length && containerBintangLevel[i] != null)
            {
                Transform parentStars = containerBintangLevel[i].transform;
                Transform objStars1 = parentStars.Find("Stars 1");
                Transform objStars2 = parentStars.Find("Stars 2");
                Transform objStars3 = parentStars.Find("Stars 3");

                if (objStars1 != null && objStars2 != null && objStars3 != null)
                {
                    objStars1.gameObject.SetActive(true);
                    objStars2.gameObject.SetActive(true);
                    objStars3.gameObject.SetActive(true);

                    UnityEngine.UI.Image imgStars1 = objStars1.GetComponent<UnityEngine.UI.Image>();
                    UnityEngine.UI.Image imgStars2 = objStars2.GetComponent<UnityEngine.UI.Image>();
                    UnityEngine.UI.Image imgStars3 = objStars3.GetComponent<UnityEngine.UI.Image>();

                    if (imgStars1 != null) imgStars1.sprite = spriteBintangHitam;
                    if (imgStars2 != null) imgStars2.sprite = spriteBintangHitam;
                    if (imgStars3 != null) imgStars3.sprite = spriteBintangHitam;

                    int jumlah = dataLvl.jumlahBintang;
                    if (jumlah >= 1 && imgStars1 != null) imgStars1.sprite = spriteBintangEmas; 
                    if (jumlah >= 2 && imgStars2 != null) imgStars2.sprite = spriteBintangEmas; 
                    if (jumlah >= 3 && imgStars3 != null) imgStars3.sprite = spriteBintangEmas; 
                }
            }
        }
        
        if (textSkorKumulatif != null && teksTotalSkor != null) 
        { 
            textSkorKumulatif.text = teksTotalSkor.GetLocalizedString(totalSkorBabakIni); 
        }
    }

    public void KembaliKePilihSlotDariBabak() 
    { 
        if (panelBabak != null) 
        {
            panelBabak.SetActive(false); 
        }
        
        if (SaveSlotManager.instance != null)
        {
            SaveSlotManager.instance.PersiapkanSiklusSlot(); 
        }
    }
    
    public void KembaliKePilihBabakDariLevel() 
    { 
        subPanelPilihLevel.SetActive(false); 
        subPanelPilihBabak.SetActive(true); 
        
        if (txtJudulPanelBabak != null && judulPilihBabak != null) 
        {
            txtJudulPanelBabak.text = judulPilihBabak.GetLocalizedString(); 
        }
    }
    
    public void PindahKeSceneMedanLaga(int nomorLevel)
    {
        string namaSceneTujuan = "Babak" + babakAktifTerpilih + "Level" + nomorLevel;
        
        if (Application.CanStreamedLevelBeLoaded(namaSceneTujuan))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(namaSceneTujuan);
        }
        else
        {
            if (notifBelumTersedia != null)
            {
                MunculkanNotif(notifBelumTersedia.GetLocalizedString());
            }
            Debug.LogWarning("Scene '" + namaSceneTujuan + "' belum masuk Build Settings!");
        }
    }

    public void MunculkanNotif(string pesan)
    {
        if (textNotifLevelSistem != null)
        {
            textNotifLevelSistem.text = pesan;
            CancelInvoke("HapusNotif"); 
            Invoke("HapusNotif", 1.5f); 
        }
    }

    void HapusNotif()
    {
        if (textNotifLevelSistem != null)
        {
            textNotifLevelSistem.text = "";
        }
    }
}