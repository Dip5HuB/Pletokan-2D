using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Pengaturan Sumber Suara (Audio Source)")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource langkahSource;

    [Header("Koleksi Suara Latar (Audio Clip)")]
    public AudioClip musikMenuUtama;  
    public AudioClip musikGameplay;   
    public AudioClip musikMenang;
    public AudioClip musikKalah;

    [Header("Koleksi Efek Suara SFX (Audio Clip)")]
    public AudioClip suaraPletokanBiasa;
    public AudioClip suaraPletokanEmas;
    public AudioClip suaraLangkahKaki;

    private float globalVolume = 1f; 

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        MuatPengaturanVolume(); 
    }

    void Start()
    {
        if (instance != this)
        {
            return;
        }

        if (musikMenuUtama != null)
        {
            PutarMusik(musikMenuUtama);
        }
    }

    // --- SISTEM MANAGEMENT VOLUME GLOBAL ---

    public void AturVolume(float nilaiVolume)
    {
        globalVolume = nilaiVolume;
        
        TerapkanVolumeKeAudioSource(globalVolume);

        // Simpan secara permanen ke PlayerPrefs setiap kali diubah
        PlayerPrefs.SetFloat("VolumeGame", globalVolume);
        PlayerPrefs.Save();
    }

    // Digunakan murni untuk sinkronisasi UI di awal scene tanpa merusak PlayerPrefs
    public void AturVolumeTanpaSimpan(float nilaiVolume)
    {
        globalVolume = nilaiVolume;
        
        TerapkanVolumeKeAudioSource(globalVolume);
    }

    void TerapkanVolumeKeAudioSource(float volume)
    {
        if (bgmSource != null) 
        {
            bgmSource.volume = volume;
        }
        
        if (sfxSource != null) 
        {
            sfxSource.volume = volume;
        }
        
        if (langkahSource != null) 
        {
            langkahSource.volume = volume;
        }
    }

    public float AmbilVolumeSaatIni()
    {
        return globalVolume;
    }

    void MuatPengaturanVolume()
    {
        globalVolume = PlayerPrefs.GetFloat("VolumeGame", 1f);
        
        // Gunakan fungsi tanpa simpan saat baru inisialisasi game awal
        AturVolumeTanpaSimpan(globalVolume);
    }

    // --- SISTEM FUNGSI KONTROL MUSIK & SFX ---

    public void PutarMusik(AudioClip klip)
    {
        if (bgmSource != null && klip != null)
        {
            // Jika lagu yang ingin diputar sudah menyala saat ini, abaikan agar tidak mengulang dari nol
            if (bgmSource.clip == klip && bgmSource.isPlaying)
            {
                return;
            }

            bgmSource.clip = klip;
            bgmSource.Play();
        }
    }

    public void GantiMusikAkhir(bool isMenang)
    {
        // ---> Matikan loop saat panel selesai muncul <---
        if (bgmSource != null)
        {
            bgmSource.loop = false; 
        }

        if (isMenang)
        {
            PutarMusik(musikMenang);
        }
        else
        {
            PutarMusik(musikKalah);
        }
    }

    public void ResetMusikKeAmbient()
    {
        // ---> Hidupkan kembali loop saat kembali bermain/ke menu utama <---
        if (bgmSource != null)
        {
            bgmSource.loop = true; 
        }

        if (musikGameplay != null)
        {
            PutarMusik(musikGameplay);
        }
    }

    public void SetSuaraBerjalan(bool isNyala)
    {
        if (langkahSource != null)
        {
            if (isNyala)
            {
                // Pasang klip langkah kaki jika belum terpasang
                if (langkahSource.clip == null)
                {
                    langkahSource.clip = suaraLangkahKaki;
                    langkahSource.loop = true;
                }

                if (!langkahSource.isPlaying)
                {
                    langkahSource.Play();
                }
            }
            else
            {
                if (langkahSource.isPlaying)
                {
                    langkahSource.Stop();
                }
            }
        }
    }
}