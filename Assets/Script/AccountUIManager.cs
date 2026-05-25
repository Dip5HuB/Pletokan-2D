using UnityEngine;
using TMPro;

public class AccountUIManager : MonoBehaviour
{
    public static AccountUIManager instance;

    [Header("Referensi Kelompok Utama UI")]
    public GameObject uiHome;         
    public GameObject panelAkun;      

    [Header("Panels Internal Akun")]
    public GameObject panelLogin;
    public GameObject panelRegister;
    public GameObject panelProfile; 
    public GameObject panelConfirmLogout;
    public GameObject panelConfirmDelete;

    [Header("Input Fields")]
    public TMP_InputField loginUser;
    public TMP_InputField loginPass;
    public TMP_InputField regUser;
    public TMP_InputField regPass;

    [Header("Profile Info")]
    public TextMeshProUGUI txtUsernameProfile;

    [Header("Notification System")]
    public TextMeshProUGUI textNotif;
    public Color colorSuccess = Color.green;
    public Color colorFail = Color.red;

    void Awake()
    {
        instance = this;
    }


    // --- MANAJEMEN PERPINDAHAN PANEL ---
    public void BukaPanelLoginAwal()
    {
        if (uiHome != null) uiHome.SetActive(false);
        if (panelAkun != null) panelAkun.SetActive(true);
        
        panelLogin.SetActive(true);
        panelRegister.SetActive(false);
        panelProfile.SetActive(false);
        if (panelConfirmLogout != null) panelConfirmLogout.SetActive(false);
        if (panelConfirmDelete != null) panelConfirmDelete.SetActive(false);
        textNotif.text = "";
    }

    public void TampilkanPanelRegister()
    {
        panelLogin.SetActive(false);
        panelRegister.SetActive(true);
    }

    public void TampilkanMainMenuHome(string namaPemain)
    {
        if (panelAkun != null) panelAkun.SetActive(false);
        if (uiHome != null) uiHome.SetActive(true);
        
        if (txtUsernameProfile != null) txtUsernameProfile.text = namaPemain;

        // Bersihkan sisa ketikan form
        loginUser.text = ""; loginPass.text = "";
        regUser.text = ""; regPass.text = "";
    }

    public void BukaProfilDariHome()
    {
        if (uiHome != null) uiHome.SetActive(false);
        if (panelAkun != null) panelAkun.SetActive(true);
        
        panelLogin.SetActive(false);
        panelRegister.SetActive(false);
        panelProfile.SetActive(true);
    }

    public void KembaliKeHomeDariProfil()
    {
        panelProfile.SetActive(false);
        if (panelAkun != null) panelAkun.SetActive(false);
        if (uiHome != null) uiHome.SetActive(true);
    }

    // --- TAMPILAN NOTIFIKASI UI ---
    public void ShowNotif(string msg, bool isSuccess)
    {
        textNotif.text = msg;
        textNotif.color = isSuccess ? colorSuccess : colorFail;
        CancelInvoke("ClearNotif");
        Invoke("ClearNotif", 1.5f);
    }

    public void ClearNotif() => textNotif.text = "";

    public void KeluarDariGameAplikasi()
    {
        Debug.Log("Pemain menekan tombol keluar aplikasi...");

        Application.Quit();
    }
}