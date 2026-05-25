using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Localization;

public enum ModeSlot { LoadGame, GameBaru }

public class SaveSlotManager : MonoBehaviour
{
    public static SaveSlotManager instance;

    [Header("Kamus Judul & Teks Dinamis")]
    public LocalizedString judulLanjutkanGame;
    public LocalizedString judulPilihSlotBaru;
    public LocalizedString judulPilihDataGame;
    public LocalizedString teksSlotKosong;

    [Header("Referensi Panel UI Utama")]
    public GameObject panelMainParent;
    public GameObject panelDataGame; 
    public GameObject panelPilihSlot; 
    public GameObject popUpKonfirmasiHapusSlot;
    public GameObject popUpKonfirmasiTimpaSlot; 
    public TextMeshProUGUI txtJudulPanelMain; 

    [Header("Tombol Slot 1")]
    public Button btnSlot1;
    public TextMeshProUGUI txtSlot1;
    public GameObject btnHapusSlot1;

    [Header("Tombol Slot 2")]
    public Button btnSlot2;
    public TextMeshProUGUI txtSlot2;
    public GameObject btnHapusSlot2;

    [Header("Tombol Slot 3")]
    public Button btnSlot3;
    public TextMeshProUGUI txtSlot3;
    public GameObject btnHapusSlot3;

    private ModeSlot modeSekarang;
    private SaveSlotData dataSlot1;
    private SaveSlotData dataSlot2;
    private SaveSlotData dataSlot3;
    
    private int slotYangAkanDihapus = 0;
    private int slotYangAkanDitimpa = 0;
    private string folderAkunAktif;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (popUpKonfirmasiHapusSlot != null) 
        {
            popUpKonfirmasiHapusSlot.SetActive(false);
        }

        if (popUpKonfirmasiTimpaSlot != null) 
        {
            popUpKonfirmasiTimpaSlot.SetActive(false);
        }
    }

    public void BukaMenuLanjutkanGame() 
    { 
        modeSekarang = ModeSlot.LoadGame; 
        
        if (txtJudulPanelMain != null && judulLanjutkanGame != null) 
        {
            txtJudulPanelMain.text = judulLanjutkanGame.GetLocalizedString(); 
        }
        
        PersiapkanSiklusSlot(); 
    }
    
    public void BukaMenuGameBaru() 
    { 
        modeSekarang = ModeSlot.GameBaru; 
        
        if (txtJudulPanelMain != null && judulPilihSlotBaru != null) 
        {
            txtJudulPanelMain.text = judulPilihSlotBaru.GetLocalizedString(); 
        }
        
        PersiapkanSiklusSlot(); 
    }
    
    public void KembaliKeMenuPilihDataGame() 
    { 
        panelPilihSlot.SetActive(false); 
        panelDataGame.SetActive(true); 

        if (txtJudulPanelMain != null && judulPilihDataGame != null) 
        {
            txtJudulPanelMain.text = judulPilihDataGame.GetLocalizedString(); 
        }
    }

    public void SembunyikanPanelSlot()
    {
        panelPilihSlot.SetActive(false);
        panelDataGame.SetActive(false);

        if (panelMainParent != null) 
        {
            panelMainParent.SetActive(false);
        }
    }

    public void PersiapkanSiklusSlot()
    {
        if (panelMainParent != null)
        {
            panelMainParent.SetActive(true);
        }

        string namaAkun = "Guest";
        if (AccountManager.instance != null)
        {
            namaAkun = AccountManager.instance.NamaAkunAktif;
        }
        
        folderAkunAktif = Path.Combine(Application.persistentDataPath, namaAkun);
        
        if (!Directory.Exists(folderAkunAktif)) 
        {
            Directory.CreateDirectory(folderAkunAktif);
        }
        
        dataSlot1 = MuatDataSlotFisik(1); 
        dataSlot2 = MuatDataSlotFisik(2); 
        dataSlot3 = MuatDataSlotFisik(3);
        
        RefreshVisualTombolSlot(1, dataSlot1, txtSlot1, btnHapusSlot1); 
        RefreshVisualTombolSlot(2, dataSlot2, txtSlot2, btnHapusSlot2); 
        RefreshVisualTombolSlot(3, dataSlot3, txtSlot3, btnHapusSlot3);
        
        // Atur aktivasi sub-panel anak
        if (panelDataGame != null) panelDataGame.SetActive(false); 
        if (panelPilihSlot != null) panelPilihSlot.SetActive(true);
    }

    void RefreshVisualTombolSlot(int nomorSlot, SaveSlotData data, TextMeshProUGUI txtLabel, GameObject objBtnHapus)
    {
        if (data.isKosong) 
        { 
            if (txtLabel != null && teksSlotKosong != null)
            {
                txtLabel.text = teksSlotKosong.GetLocalizedString();
            }
            if (objBtnHapus != null) objBtnHapus.SetActive(false); 
        }
        else 
        { 
            // Untuk teks "Save1 24/05/26" biarkan karena berupa data String Teknis/Angka
            txtLabel.text = data.namaSlot + " " + data.tanggalDibuat; 
            if (objBtnHapus != null) objBtnHapus.SetActive(true); 
        }
    }

    public void KlikTombolPlayUtamaHome()
    {
        if (panelMainParent != null)
        {
            panelMainParent.SetActive(true);
        }

        if (panelDataGame != null) panelDataGame.SetActive(true);   
        if (panelPilihSlot != null) panelPilihSlot.SetActive(false); 

        if (txtJudulPanelMain != null && judulPilihDataGame != null) 
        {
            txtJudulPanelMain.text = judulPilihDataGame.GetLocalizedString(); 
        }
    }

    public void MatikanSeluruhPanelMain()
    {
        if (panelMainParent != null)
        {
            panelMainParent.SetActive(false); // Mematikan parent otomatis mematikan semua anak di bawahnya
        }
    }

    public void KlikTombolSlot(int nomorSlot)
    {
        SaveSlotData slotTerpilih = (nomorSlot == 1) ? dataSlot1 : (nomorSlot == 2) ? dataSlot2 : dataSlot3;
        
        if (modeSekarang == ModeSlot.GameBaru) 
        { 
            if (!slotTerpilih.isKosong) 
            { 
                PemicuPopUpTimpa(nomorSlot); 
            } 
            else 
            { 
                CreateNewSaveGame(nomorSlot); 
            } 
        }
        else 
        { 
            if (slotTerpilih.isKosong) return; 
            
            LoadExistingSaveGame(slotTerpilih, nomorSlot); 
        }
    }

    void PemicuPopUpTimpa(int nomorSlot) 
    { 
        slotYangAkanDitimpa = nomorSlot; 
        if (popUpKonfirmasiTimpaSlot != null) popUpKonfirmasiTimpaSlot.SetActive(true); 
    }
    
    public void KonfirmasiTimpaYa() 
    { 
        if (slotYangAkanDitimpa == 0) return; 
        CreateNewSaveGame(slotYangAkanDitimpa); 
        if (popUpKonfirmasiTimpaSlot != null) popUpKonfirmasiTimpaSlot.SetActive(false); 
        slotYangAkanDitimpa = 0; 
    }
    
    public void KonfirmasiTimpaTidak() 
    { 
        slotYangAkanDitimpa = 0; 
        if (popUpKonfirmasiTimpaSlot != null) popUpKonfirmasiTimpaSlot.SetActive(false); 
    }
    
    void CreateNewSaveGame(int nomorSlot) 
    { 
        SaveSlotData newData = new SaveSlotData("Save" + nomorSlot); 
        newData.isKosong = false; 
        newData.tanggalDibuat = DateTime.Now.ToString("dd/MM/yy"); 
        
        SimpanDataSlotFisik(nomorSlot, newData); 
        TransferDataKePusatAbadi(newData, nomorSlot); 
        
        SembunyikanPanelSlot();
        LevelSelectionManager.instance.BukaMenuBabakAwal(); // Lempar tugas ke manager sebelah
    }
    
    void LoadExistingSaveGame(SaveSlotData dataYangDimuat, int nomorSlot) 
    { 
        TransferDataKePusatAbadi(dataYangDimuat, nomorSlot);  
        
        SembunyikanPanelSlot();
        LevelSelectionManager.instance.BukaMenuBabakAwal(); // Lempar tugas ke manager sebelah
    }
    
    void TransferDataKePusatAbadi(SaveSlotData data, int nomorSlot) 
    { 
        if (GameDataManager.instance != null) 
        { 
            GameDataManager.instance.slotAktif = data; 
            GameDataManager.instance.nomorSlotTerpilih = nomorSlot; 
            GameDataManager.instance.folderPathAkun = folderAkunAktif; 
        } 
    }

    public void PemicuTombolX(int nomorSlot) 
    { 
        slotYangAkanDihapus = nomorSlot; 
        if (popUpKonfirmasiHapusSlot != null) popUpKonfirmasiHapusSlot.SetActive(true); 
    }
    
    public void KonfirmasiHapusYa() 
    { 
        if (slotYangAkanDihapus == 0) return; 
        string pathFile = Path.Combine(folderAkunAktif, "slot" + slotYangAkanDihapus + ".dat"); 
        
        try 
        {
            if (File.Exists(pathFile)) File.Delete(pathFile);
        }
        catch (Exception e) 
        {
            Debug.LogError("Gagal menghapus file slot secara fisik: " + e.Message);
        }

        if (popUpKonfirmasiHapusSlot != null) popUpKonfirmasiHapusSlot.SetActive(false); 
        PersiapkanSiklusSlot(); 
        slotYangAkanDihapus = 0; 
    }
    
    public void KonfirmasiHapusTidak() 
    { 
        slotYangAkanDihapus = 0; 
        if (popUpKonfirmasiHapusSlot != null) popUpKonfirmasiHapusSlot.SetActive(false); 
    }
    
    void SimpanDataSlotFisik(int nomorSlot, SaveSlotData data) 
    { 
        try 
        { 
            string pathFile = Path.Combine(folderAkunAktif, "slot" + nomorSlot + ".dat"); 
            BinaryFormatter bf = new BinaryFormatter(); 
            FileStream file = File.Create(pathFile); 
            bf.Serialize(file, data); 
            file.Close(); 
        } 
        catch (Exception e) 
        { 
            Debug.LogError("Error menulis berkas biner slot: " + e.Message); 
        } 
    }
    
    SaveSlotData MuatDataSlotFisik(int nomorSlot) 
    { 
        string pathFile = Path.Combine(folderAkunAktif, "slot" + nomorSlot + ".dat"); 
        if (File.Exists(pathFile)) 
        { 
            try 
            { 
                BinaryFormatter bf = new BinaryFormatter(); 
                FileStream file = File.Open(pathFile, FileMode.Open); 
                SaveSlotData data = (SaveSlotData)bf.Deserialize(file); 
                file.Close(); 
                return data; 
            } 
            catch 
            { 
                return new SaveSlotData("Save" + nomorSlot); 
            } 
        } 
        return new SaveSlotData("Save" + nomorSlot); 
    }
}