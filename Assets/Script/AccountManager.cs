using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Localization;

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;

    private GlobalAccountRegistry registry = new GlobalAccountRegistry();
    private string savePath;
    private UserAccount activeUser;

    [Header("Kamus Notifikasi UI")]
    public LocalizedString notifInputKosong;
    public LocalizedString notifAkunTerdaftar;
    public LocalizedString notifDiskPenuh;
    public LocalizedString notifBuatAkunSukses;
    public LocalizedString notifLoginSukses;
    public LocalizedString notifLoginGagal;
    public LocalizedString notifLogoutSukses;
    public LocalizedString notifHapusAkunSukses;

    public string NamaAkunAktif
    {
        get { return activeUser != null ? activeUser.username : "Guest"; }
    }

    void Awake()
    {
        instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "accounts.dat");
        LoadRegistry();
    }

    void Start()
    {
        if (registry != null && !string.IsNullOrEmpty(registry.lastLoggedInUser))
        {
            UserAccount autoUser = registry.allUsers.Find(x => x.username == registry.lastLoggedInUser);
            
            if (autoUser != null)
            {
                activeUser = autoUser;
                
                Debug.Log("Auto-Login Berhasil! Selamat datang kembali, " + activeUser.username);

                // Cek apakah pemain baru saja kembali dari scene game pertempuran
                if (GameDataManager.instance != null && GameDataManager.instance.kembaliDariGame)
                {
                    // ---> nama pemain ke UI sebelum proses di-bypass <---
                    if (AccountUIManager.instance != null)
                    {
                        AccountUIManager.instance.TampilkanMainMenuHome(activeUser.username);
                    }
                    
                    return; // Rem ditarik, tidak lanjut meriset data pusat
                }

                // Reset data pusat di awal sesi baru
                ResetDataPusatSebelumMasuk();
                
                // Perintahkan UIManager untuk langsung lompat ke Home karena session tersimpan
                if (AccountUIManager.instance != null)
                {
                    AccountUIManager.instance.TampilkanMainMenuHome(activeUser.username);
                }
                return; // Keluar dari Start, bypass fungsi login awal
            }
        }

        // Jika tidak ada data auto-login terdeteksi, perintahkan UI untuk buka panel login biasa
        if (AccountUIManager.instance != null)
        {
            AccountUIManager.instance.BukaPanelLoginAwal();
        }
    }

    // --- FUNGSI MURNI FITUR AKUN ---

    public void TriggerRegister()
    {
        string user = AccountUIManager.instance.regUser.text.Trim();
        string pass = AccountUIManager.instance.regPass.text.Trim();

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            // Panggil .GetLocalizedString() untuk mengubah kunci menjadi teks sesuai bahasa aktif
            AccountUIManager.instance.ShowNotif(notifInputKosong.GetLocalizedString(), false);
            return;
        }

        if (registry.allUsers.Exists(x => x.username == user))
        {
            AccountUIManager.instance.ShowNotif(notifAkunTerdaftar.GetLocalizedString(), false);
            return;
        }

        if (!HasEnoughDiskSpace())
        {
            AccountUIManager.instance.ShowNotif(notifDiskPenuh.GetLocalizedString(), false);
            return;
        }

        UserAccount newAcc = new UserAccount { username = user, password = pass };
        registry.allUsers.Add(newAcc);
        
        SaveRegistry();
        AccountUIManager.instance.ShowNotif(notifBuatAkunSukses.GetLocalizedString(), true);
        
        AccountUIManager.instance.Invoke("BukaPanelLoginAwal", 1.5f);
    }

    public void TriggerLogin()
    {
        CancelInvoke("MasukKeHomeSetelahDelay");

        string user = AccountUIManager.instance.loginUser.text.Trim();
        string pass = AccountUIManager.instance.loginPass.text.Trim();

        UserAccount found = registry.allUsers.Find(x => x.username == user && x.password == pass);

        if (found != null)
        {
            activeUser = found;
            registry.lastLoggedInUser = user; 
            SaveRegistry();

            ResetDataPusatSebelumMasuk();
            
            AccountUIManager.instance.ShowNotif(notifLoginSukses.GetLocalizedString(), true);
            Invoke("MasukKeHomeSetelahDelay", 0.5f);
        }
        else
        {
            AccountUIManager.instance.ShowNotif(notifLoginGagal.GetLocalizedString(), false);
        }
    }

    void MasukKeHomeSetelahDelay()
    {
        AccountUIManager.instance.TampilkanMainMenuHome(activeUser.username);
    }

    public void TriggerLogout()
    {
        activeUser = null;

        if (registry != null)
        {
            registry.lastLoggedInUser = "";
            SaveRegistry();
        }

        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.ResetSesiPusat();
        }

        if (SaveSlotManager.instance != null)
        {
            SaveSlotManager.instance.MatikanSeluruhPanelMain();
        }

        if (LevelSelectionManager.instance != null && LevelSelectionManager.instance.panelBabak != null)
        {
            LevelSelectionManager.instance.panelBabak.SetActive(false);
        }

        AccountUIManager.instance.BukaPanelLoginAwal();
        AccountUIManager.instance.ShowNotif(notifLogoutSukses.GetLocalizedString(), true);
    }

    public void TriggerDeleteAccount()
    {
        if (activeUser != null)
        {
            string folderPath = Path.Combine(Application.persistentDataPath, activeUser.username);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true); 
            }
            
            registry.allUsers.RemoveAll(x => x.username == activeUser.username);
            SaveRegistry();
            AccountUIManager.instance.ShowNotif(notifHapusAkunSukses.GetLocalizedString(), true);
            TriggerLogout();
        }
    }

    void ResetDataPusatSebelumMasuk()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.ResetSesiPusat();
        }
    }

    // --- SYSTEM FILE SYSTEM BINARY ---
    void SaveRegistry()
    {
        try {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);
            bf.Serialize(file, registry);
            file.Close();
        } catch (System.Exception e) {
            Debug.LogError("Gagal Simpan Binary: " + e.Message);
        }
    }

    void LoadRegistry()
    {
        if (File.Exists(savePath)) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(savePath, FileMode.Open);
                registry = (GlobalAccountRegistry)bf.Deserialize(file);
                file.Close();
            } catch {
                registry = new GlobalAccountRegistry();
            }
        }
    }

    bool HasEnoughDiskSpace()
    {
        string drivePath = Path.GetPathRoot(Application.persistentDataPath);
        DriveInfo drive = new DriveInfo(drivePath);
        long requiredSpace = 5 * 1024 * 1024; 
        return drive.AvailableFreeSpace > requiredSpace;
    }
}