using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    
    //Zamanım olsaydı şu CurrentHealth variableını NetworkVarıable olarak yaratıp denemek isterdim.
    
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public float moveSpeed = 5f; // Hareket hızı
    
    private bool hasLost = false;
    
    public GameObject LoseScreen;

    public List<Transform> spawnPoints;
    public GameObject enemyPrefab;
    
    private int repeatCount = 3; // Toplam kaç kez tekrarlanacağı
    private int currentRepeat = 0; // Şu an hangi tekrarda olduğumuzu tutar
   
    
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
        if (IsServer) // sadece server tarafında çalıştır
        {
            StartCoroutine(RepeatEnemyMethod());
        }
    }

    #region Task8RandomEnemySpawn

    IEnumerator RepeatEnemyMethod()
    {
        while (currentRepeat < repeatCount)
        {
            Debug.Log("Metod Çağrıldı - Tekrar: " + (currentRepeat + 1));

            SpawnEnemyServerRpc();

            yield return new WaitForSeconds(20f); // 20 saniye bekle

            currentRepeat++; // Tekrar sayısını artır
        }

        Debug.Log("İşlem tamamlandı.");
    }
    
    
    //SpawnEnemyMethod
    [ServerRpc] // bu metod sadece server tarafından çağrılır
    public void SpawnEnemyServerRpc()
    {
        Transform spawnPoint = GetAndRemoveRandomSpawnPoint();
        if (spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
            networkObject.Spawn(); // enemy'i network üzerinden spawn et
        }
    }
    
    Transform GetAndRemoveRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("SpawnPoints listesi boş!");
            return null;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform selectedSpawnPoint = spawnPoints[randomIndex];
        spawnPoints.RemoveAt(randomIndex);
        return selectedSpawnPoint;
    }

        #endregion
        
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
        
        
        if (currentHealth <= 0 && !hasLost)
        {
            hasLost = true;
            ShowLoseScreen();
        }
        
    }
    
    
    
    
    // Kaybetme ekranını gösterme işlemi
    private void ShowLoseScreen()
    {
        if (IsLocalPlayer) // Sadece yerel oyuncu için göster
        {
            Instantiate(LoseScreen);
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
    
    
    
    
   


    #region HealthMethods

    // Host kendi canını artırır
    [ServerRpc]
    public void HealCharServerRpc(int healHp)
    {
        if (IsServer) // Bu kontrol gereksiz çünkü ServerRpc sadece server tarafından çalıştırılabilir.
        {
            HealChar(healHp); // Hostun canını artırmak için
        }
    }

    // ServerRpcParams, hangi istemcinin bu isteği gönderdiğini belirler ve SenderClientId ile bu istemciyi tespit eder.
    // ClientRpcParams, belirli istemcilere gönderilecek RPC çağrılarını yapılandırmak için kullanılır. Bu durumda sadece belirli bir istemciye gönderilecek.
    
    // Server üzerinden client'ın canını artırmak için
    [ServerRpc]
    public void RequestHealServerRpc(int healHp, ServerRpcParams serverRpcParams = default)
    {
        if (IsServer)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            HealCharClientRpc(healHp, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } } });
        }
    }

    // Client kendi canını artırır
    [ClientRpc]
    public void HealCharClientRpc(int healHp, ClientRpcParams clientRpcParams = default)
    {
        if (IsClient) // Bu kontrol gereksiz çünkü ClientRpc sadece client tarafından çalıştırılabilir.
        {
            HealChar(healHp); // Yerel oyuncunun canını artırmak için
        }
    }

    // Can artırma işlemi
    public void HealChar(int healHp)
    {
        Debug.Log("CAN DEGISTIRILIYOR.......");
        currentHealth += healHp;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Maksimum sağlık kontrolü
        healthBar.SetHealth(currentHealth); // Sağlık barını güncelle
    }

    #endregion
    
    
}