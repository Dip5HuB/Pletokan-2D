using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Musuh : MonoBehaviour
{
    public enum StatusMusuh {MasukArena, Bertarung}

    [Header("=== KARTU DATA MUSUH ===")]
    public DataMusuh dataStatistik; // Masukkan file data musuh ke sini nanti!

    [Header("Pengaturan state machine")]
    public StatusMusuh statusSekarang = StatusMusuh.MasukArena;

    [Header("Pengaturan masuk arena")]
    public float jarakJalanMasuk = 3f;
    private Vector2 titikMasuk;

    [Header("Komponen)")]
    public Collider2D groundCollider; 
    public Image healthBarFill;
    public Color fullColor = Color.green;
    public Color halfColor = new Color(1f, 0.5f, 0f);
    public Color lowColor = Color.red;
    public Transform firePoint;
    private int poinSkor;

    // --- VARIABEL STATISTIK (SEKARANG DISIMPAN SECARA INTERNAL) ---
    private int maxHealth;
    private int currentHealth;
    private float moveSpeed;
    private float attackRange;
    private float stopDistance;
    private float retreatDistance;
    private GameObject peluruMusuhPrefab;
    private float peluruSpeed;
    private float shootInterval;
    private float delayTembakan;

    private float nextShootTime = 0f;
    private Transform player;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;



    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (dataStatistik != null)
        {
            maxHealth = dataStatistik.maxHealth;
            moveSpeed = dataStatistik.moveSpeed;
            attackRange = dataStatistik.attackRange;
            retreatDistance = dataStatistik.retreatDistance;
            peluruMusuhPrefab = dataStatistik.peluruMusuhPrefab;
            peluruSpeed = dataStatistik.peluruSpeed;
            shootInterval = dataStatistik.shootInterval;
            delayTembakan = dataStatistik.delayTembakan;
            poinSkor = dataStatistik.poinSkor;
            
            // Acak stop distance berdasarkan rentang yang ada di data statistik
            stopDistance = Random.Range(dataStatistik.minStopDistance, dataStatistik.maxStopDistance);
        }
        else
        {
            Debug.LogWarning("AWAS: Kamu lupa memasukkan Data Statistik Musuh ke " + gameObject.name);
        }

        currentHealth = maxHealth;
        UpdateHealthBar();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (groundCollider != null)
        {
            groundCollider.isTrigger = true; 
        }

        float arahMasuk = transform.position.x < 0 ? 1f : -1f;
        titikMasuk = new Vector2(transform.position.x + arahMasuk * jarakJalanMasuk, transform.position.y);
    }

    void Update ()
    {
        if (isDead || player == null) return;

        if (statusSekarang == StatusMusuh.MasukArena){
            LogikaMasukArena();
        }
        else if (statusSekarang == StatusMusuh.Bertarung){
            LogikaBertarung();
        }
    }

    void LogikaMasukArena(){
        Vector2 arahGerak = (titikMasuk - (Vector2)transform.position).normalized;
        rb.velocity = arahGerak * moveSpeed;
        anim.SetBool("isRunning", true);

        if (titikMasuk.x > transform.position.x && !isFacingRight) Flip();
        else if (titikMasuk.x < transform.position.x && isFacingRight) Flip();

        if (Vector2.Distance(transform.position, titikMasuk) < 0.2f){
            statusSekarang = StatusMusuh.Bertarung;

            if (groundCollider != null){
                groundCollider.isTrigger = false;
            }
        }
    }

    void LogikaBertarung(){
        float jarakKePlayer = Vector2.Distance(transform.position, player.position);

        if (player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (player.position.x < transform.position.x && isFacingRight) Flip();

        Vector2 arahGerak = (player.position - transform.position).normalized;
        Vector2 targetKecepatan = Vector2.zero;

        if (jarakKePlayer > stopDistance ){
            targetKecepatan = arahGerak * moveSpeed;
            anim.SetBool("isRunning", true);
        }
        else if ( jarakKePlayer < retreatDistance){
            targetKecepatan = -arahGerak * moveSpeed;
            anim.SetBool("isRunning", true);
        }
        else{
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
        }

        rb.velocity = Vector2.Lerp(rb.velocity, targetKecepatan, 5f * Time.deltaTime);

        if (jarakKePlayer <= attackRange && Time.time >= nextShootTime)
        {
            PerformShooot();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f; 
        transform.localScale = localScale;

        if (healthBarFill != null)
        {
            Vector3 healthBarScale = healthBarFill.transform.parent.localScale;
            healthBarScale.x *= -1f; 
            healthBarFill.transform.parent.localScale = healthBarScale;
        }
    }

    void PerformShooot()
    {
        if (anim != null) anim.SetTrigger("Shoot");
        StartCoroutine(ShootDenganJeda());
    }

    IEnumerator ShootDenganJeda()
    {
        yield return new WaitForSeconds(delayTembakan); 
        ShootPeluru();
    }

    void ShootPeluru()
    {
        if (peluruMusuhPrefab == null || firePoint == null) return;

        GameObject peluruTerspawn = Instantiate(peluruMusuhPrefab, firePoint.position, firePoint.rotation);

        Vector3 scalePeluru = peluruTerspawn.transform.localScale;
        scalePeluru.x = isFacingRight ? Mathf.Abs(scalePeluru.x) : -Mathf.Abs(scalePeluru.x);
        peluruTerspawn.transform.localScale = scalePeluru;

        Rigidbody2D peluruRb = peluruTerspawn.GetComponent<Rigidbody2D>();
        float arahTembak = isFacingRight ? 1f : -1f;

        peluruRb.velocity = new Vector2(arahTembak * peluruSpeed, 0f);

        Destroy(peluruTerspawn, 2f);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        currentHealth -= damageAmount;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthFraction = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthFraction;

            if (healthFraction > 0.5f)
            {
                float t = (healthFraction - 0.5f) / 0.5f; 
                healthBarFill.color = Color.Lerp(halfColor, fullColor, t); 
            }
            else
            {
                float t = healthFraction / 0.5f; 
                healthBarFill.color = Color.Lerp(lowColor, halfColor, t); 
            }
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log(gameObject.name + " mati! Player dapat " + poinSkor + " poin!");

        if (ScoreManager.instance != null){
            ScoreManager.instance.TambahSkor(poinSkor);
        }

        if (MisiManager.instance != null && dataStatistik != null)
        {
            MisiManager.instance.LaporMusuhMati(dataStatistik.isMusuhElit, dataStatistik.isMusuhBoss);
        }
        
        if (anim != null) anim.SetTrigger("Die");

        rb.velocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (healthBarFill != null && healthBarFill.transform.parent != null)
        {
            healthBarFill.transform.parent.gameObject.SetActive(false);
        }

        StartCoroutine(ProsesKematianMusuh());
    }

    IEnumerator ProsesKematianMusuh()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}