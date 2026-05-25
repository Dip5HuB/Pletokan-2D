using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public VariableJoystick variableJoystick;

    [Header("Healing System")]
    public int maxHealth = 100;
    public int currentHealth;
    public int healAmount = 15;
    
    public int maxHealCharges = 3;
    public int currentHealCharges;
    public float healRechargeTime = 30f; 
    private float healRechargeTimer = 0f;

    public float healUseCooldown = 3f;
    private bool canUseHeal = true;
    public Button btnHeal;
    public TextMeshProUGUI healText;

    [Header("Health Bar Settings")]
    public Image healthBarFill;
    public Color fullHealthColor = Color.green;
    public Color halfHealthColor = new Color(1f, 0.5f, 0f); // Orange
    public Color lowHealthColor = Color.red;

    [Header("Shooting Settings")]
    public float shootInterval = 0.5f; 
    private float nextShootTime = 0f; 

    [Header("Peluru Pletokan")]
    public GameObject peluruPrefab;
    public Transform firePoint;
    public float peluruSpeed = 10f;
    public float shootDelay = 0.33f;

    [Header("Peluru Emas")]
    public GameObject peluruEmasPrefab;
    public Button btnPeluruEmas;
    public float cooldownEmas = 60f; 
    private bool isEmasReady = true; 

    private Animator anim;
    private Vector2 keyboardInput;
    private Vector2 finalMoveInput;
    private bool isFacingRight = true;
    private bool isDead = false;

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentHealCharges = maxHealCharges;
        UpdateHealUI();
        UpdateHealthBarUI();
    }

    void Update()
    {
        if (isDead || (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan))
        {
            anim.SetBool("isRunning", false);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.SetSuaraBerjalan(false); 
            }
            return; 
        }

        //--- PERGERAKAN ---
        Vector2 joystickInput = new Vector2(variableJoystick.Horizontal, variableJoystick.Vertical);

        if (joystickInput.magnitude > 0.1f)
        {
            finalMoveInput = joystickInput;
        }
        else
        {
            finalMoveInput = keyboardInput;
        }
        
        bool isMoving = finalMoveInput.magnitude > 0.1f; 
        anim.SetBool("isRunning", isMoving);

        FlipController();
        TimeHealRecharge(); 

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSuaraBerjalan(isMoving);
        }   
    }

    void FixedUpdate()
    {
        if (isDead || (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan)) 
        {
            rb.velocity = Vector2.zero; 
            return;
        }
        rb.velocity = finalMoveInput * moveSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        keyboardInput = context.ReadValue<Vector2>();
    }

    //------------- Healing System --------------
    public void TimeHealRecharge()
    {
        if (currentHealCharges < maxHealCharges)
        {
            healRechargeTimer += Time.deltaTime;

            if (healRechargeTimer >= healRechargeTime)
            {
                currentHealCharges++;
                healRechargeTimer = 0f;
                UpdateHealUI();
                Debug.Log("Satu Heal Charge telah terisi kembali! Current Heal Charges: " + currentHealCharges);
            }
        }
    }

    public void OnHeal(InputAction.CallbackContext context)
    {  
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;

        if (context.started)
        {
            PerformHeal();
        }
    }

    public void PerformHeal()
    {
        if (isDead) return; 
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;

        if (currentHealth < maxHealth && currentHealCharges > 0 && canUseHeal)
        {   
            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; 
            }

            currentHealCharges--;

            if (MisiManager.instance != null)
            { 
                MisiManager.instance.LaporTindakan(TipeMisi.GunakanPenyembuh);
                MisiManager.instance.LaporTindakan(TipeMisi.BatasPenyembuh);
                MisiManager.instance.GagalkanMisi(TipeMisi.TanpaPenyembuh);

                float persenDarah = (float)currentHealth / maxHealth * 100f;
                MisiManager.instance.LaporKondisiDarahRealTime(persenDarah);
            }
            
            StartCoroutine(HealSpamCooldown());
            UpdateHealUI();
            UpdateHealthBarUI();
        }
    }

    public void UpdateHealthBarUI()
    {
        if (healthBarFill != null)
        {
            float healthFraction = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthFraction;

            if (healthFraction > 0.5f)
            {
                float t = (healthFraction - 0.5f) / 0.5f; 
                healthBarFill.color = Color.Lerp(halfHealthColor, fullHealthColor, t); 
            }
            else
            {
                float t = healthFraction / 0.5f; 
                healthBarFill.color = Color.Lerp(lowHealthColor, halfHealthColor, t); 
            }
        }
    }

    IEnumerator HealSpamCooldown()
    {
        canUseHeal = false; 
        if (btnHeal != null) 
        {
            btnHeal.interactable = false; 
        }
        yield return new WaitForSeconds(healUseCooldown); 
        canUseHeal = true; 
        UpdateHealUI();
    }

    void UpdateHealUI()
    {
        if (healText != null)
        {
            healText.text = currentHealCharges.ToString();
        }

        if (btnHeal != null) 
        {
            btnHeal.interactable = (currentHealCharges > 0 && canUseHeal);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; 
        
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBarUI(); 

        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("Player Kalah!");
        }

        if (MisiManager.instance != null)
        {
            MisiManager.instance.GagalkanMisi(TipeMisi.TanpaLuka);

            float persenDarah = (float)currentHealth / maxHealth * 100f;
            MisiManager.instance.LaporKondisiDarahRealTime(persenDarah);
        }
    }

    void Die()
    {
        isDead = true; 
        anim.SetTrigger("Die"); 
        rb.velocity = Vector2.zero;

        // PERBAIKAN LOGIKA SUARA PEMAIN MATI SINKRON DENGAN AUDIO MANAGER BARU
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSuaraBerjalan(false);
            AudioManager.instance.GantiMusikAkhir(false); // Langsung setel musik kalah
        }
        
        StartCoroutine(ProsesKematian());
    }

    IEnumerator ProsesKematian()
    {
        yield return new WaitForSeconds(1.5f);

        if (GameTimer.instance != null)
        {
            GameTimer.instance.WaktuHabis(); 
        }
        else
        {
            Time.timeScale = 0f; 
        }
    }
        
    // ------------- Peluru Biasa --------------
    public void OnShoot(InputAction.CallbackContext context)
    {   
        if (isDead) return; 
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;

        if (context.started)
        {
            PerformShoot();
        }
    }

    public void PerformShoot()
    {    
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;
        if (isDead) return;

        if (Time.time >= nextShootTime)
        {
            anim.SetTrigger("Shoot");
            Debug.Log("Shoot!");

            if (MisiManager.instance != null)
            {
                MisiManager.instance.GagalkanMisi(TipeMisi.TanpaTembak);
            }

            nextShootTime = Time.time + shootInterval; 
            StartCoroutine(ShootDenganJeda());
        }
    }

    IEnumerator ShootDenganJeda()
    {
        yield return new WaitForSeconds(shootDelay); 
        ShootPeluru(); 
    }

    void ShootPeluru()
    {
        if (isDead) return; 

        GameObject peluruTerspawn = Instantiate(peluruPrefab, firePoint.position, firePoint.rotation);
        Vector3 scalePeluru = peluruTerspawn.transform.localScale;
        scalePeluru.x = isFacingRight ? Mathf.Abs(scalePeluru.x) : -Mathf.Abs(scalePeluru.x); 
        peluruTerspawn.transform.localScale = scalePeluru;  

        Rigidbody2D peluruRb = peluruTerspawn.GetComponent<Rigidbody2D>();
        float arahTembak = isFacingRight ? 1f : -1f; 
        peluruRb.velocity = new Vector2(arahTembak * peluruSpeed, 0f); 

        // PERBAIKAN LOGIKA: PEMUTARAN SFX TEMBAKAN BIASA LEWAT AUDIO MANAGER BARU
        if (AudioManager.instance != null && AudioManager.instance.sfxSource != null)
        {
            AudioManager.instance.sfxSource.PlayOneShot(AudioManager.instance.suaraPletokanBiasa);
        }

        Destroy(peluruTerspawn, 2f);
    }

    // ------------- Peluru Emas --------------
    public void OnShootEmas(InputAction.CallbackContext context)
    {
        if (isDead) return; 
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;

        if (context.started && isEmasReady)
        {
            PerformShootEmas();
        }
    }

    public void PerformShootEmas()
    {
        if (GameTimer.instance != null && !GameTimer.instance.isGameBerjalan) return;
        if (isDead) return;

        if (isEmasReady)
        {
            anim.SetTrigger("Shoot");
            Debug.Log("Shoot Peluru Emas!");

            if (MisiManager.instance != null)
            {
                MisiManager.instance.GagalkanMisi(TipeMisi.TanpaTembak);
                MisiManager.instance.LaporTindakan(TipeMisi.GunakanPeluruEmas);
            }
            
            StartCoroutine(ShootEmasDenganJeda());
            StartCoroutine(MulaiCooldownEmas());
        }
    }

    IEnumerator ShootEmasDenganJeda()
    {
        yield return new WaitForSeconds(shootDelay); 
        ShootPeluruEmas(); 
    }

    void ShootPeluruEmas()
    {
        if (isDead) return; 
        
        GameObject peluruEmasTerspawn = Instantiate(peluruEmasPrefab, firePoint.position, firePoint.rotation);
        Vector3 scalePeluruEmas = peluruEmasTerspawn.transform.localScale;
        scalePeluruEmas.x = isFacingRight ? Mathf.Abs(scalePeluruEmas.x) : -Mathf.Abs(scalePeluruEmas.x); 
        peluruEmasTerspawn.transform.localScale = scalePeluruEmas;  

        Rigidbody2D peluruEmasRb = peluruEmasTerspawn.GetComponent<Rigidbody2D>();
        float arahTembak = isFacingRight ? 1f : -1f; 
        peluruEmasRb.velocity = new Vector2(arahTembak * peluruSpeed, 0f); 

        // PERBAIKAN LOGIKA: PEMUTARAN SFX TEMBAKAN EMAS LEWAT AUDIO MANAGER BARU
        if (AudioManager.instance != null && AudioManager.instance.sfxSource != null)
        {
            AudioManager.instance.sfxSource.PlayOneShot(AudioManager.instance.suaraPletokanEmas);
        }

        Destroy(peluruEmasTerspawn, 3f);
    }

    IEnumerator MulaiCooldownEmas()
    {
        isEmasReady = false; 

        if (btnPeluruEmas != null) 
        {
            btnPeluruEmas.interactable = false;
        }

        yield return new WaitForSeconds(cooldownEmas);

        isEmasReady = true; 
        
        if (btnPeluruEmas != null) 
        {
            btnPeluruEmas.interactable = true;
        }
        Debug.Log("Peluru Emas Siap Digunakan!");
    }

    // ------------- Membalik Arah Karakter --------------
    private void FlipController()
    {
        if (finalMoveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (finalMoveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}