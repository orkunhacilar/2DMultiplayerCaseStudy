using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public float moveSpeed = 5f; // Hareket hızı

    private void Start()
    {
        
        
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update her frame'de çağrılır
    void Update()
    {
        if (!IsOwner)  // Sadece yerel oyuncu kontrol etsin
        {
            return;
        }

        // W, A, S, D tuşlarıyla hareket etme
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Yön vektörü oluştur
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);

        // Düzenleme yap
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }
    
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TakeDamage(30);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    
    [ServerRpc]
    public void HealCharServerRpc(int healHp)
    {
        HealChar(healHp); // Sadece hostun canını artırmak için
    }

    [ClientRpc]
    public void HealCharClientRpc(int healHp)
    {
        HealChar(healHp); // Sadece yerel oyuncunun canını artırmak için
    }

    public void HealChar(int healHp)
    {
        Debug.Log("CAN DEGISTIRILIYOR.......");
        currentHealth += healHp;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Maksimum sağlık kontrolü
        healthBar.SetHealth(currentHealth);
    }
}