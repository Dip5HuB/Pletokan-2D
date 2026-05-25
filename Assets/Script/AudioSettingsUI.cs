using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Hubungkan dengan Slider di Scene Ini")]
    public Slider sliderVolumeSceneIni;

    private bool isSelesaiInisialisasi = false;

    void OnEnable()
    {
        InisialisasiSliderSistem();
    }

    void Start()
    {
        InisialisasiSliderSistem();
    }

    void InisialisasiSliderSistem()
    {
        if (sliderVolumeSceneIni != null && AudioManager.instance != null)
        {
            // 1. Kunci status inisialisasi agar fungsi UbahVolume mengabaikan pemicu palsu Unity
            isSelesaiInisialisasi = false;

            sliderVolumeSceneIni.onValueChanged.RemoveAllListeners();

            // 2. Ambil nilai asli yang murni dari memori
            float volumeAsli = AudioManager.instance.AmbilVolumeSaatIni();
            sliderVolumeSceneIni.value = volumeAsli;

            // 3. Paksa AudioManager menerapkan volume ini tanpa menyentuh PlayerPrefs lokal
            AudioManager.instance.AturVolumeTanpaSimpan(volumeAsli);

            sliderVolumeSceneIni.onValueChanged.AddListener(UbahVolume);
            
            // 4. Inisialisasi selesai, sekarang sistem siap mendeteksi geseran manual dari tangan pemain
            isSelesaiInisialisasi = true;
        }
    }

    public void UbahVolume(float nilaiBaru)
    {
        if (!isSelesaiInisialisasi)
        {
            return;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.AturVolume(nilaiBaru);
        }
    }
}