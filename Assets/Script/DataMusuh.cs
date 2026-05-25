using UnityEngine;

// Baris ini akan membuat menu klik kanan baru di Unity
[CreateAssetMenu(fileName = "Stat_", menuName = "Game Pletokan/Data Musuh")]
public class DataMusuh : ScriptableObject
{
    [Header("Health System")]
    public int maxHealth = 100;

    [Header("Movement System")]
    public float moveSpeed = 2f;
    public float attackRange = 7f;
    public float retreatDistance = 2.5f;
    
    [Tooltip("Batas minimum jarak berhenti acak")]
    public float minStopDistance = 4f; 
    [Tooltip("Batas maksimum jarak berhenti acak")]
    public float maxStopDistance = 6f; 

    [Header("Shooting System")]
    public GameObject peluruMusuhPrefab;
    public float peluruSpeed = 10f;
    public float shootInterval = 2f;
    public float delayTembakan = 0.33f;

    [Header("Scoring & Jenis Type")]
    public int poinSkor = 10;
    public bool isMusuhElit = false;
    public bool isMusuhBoss = false;
}