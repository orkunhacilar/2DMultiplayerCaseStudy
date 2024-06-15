using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    private PlayerController player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Çarpan objenin oyuncu olup olmadığını kontrol et
        {
            player = other.gameObject.GetComponent<PlayerController>();
            
            if (IsServer)
            {
                // Sunucu tarafında çarpışma olayını işle
                HandleCollision();
            }
            else
            {
                // İstemci tarafında sunucuya çarpışma olayını bildir
                NotifyServerOnCollisionServerRpc();
            }
        }
    }

    [ServerRpc]
    private void NotifyServerOnCollisionServerRpc()
    {
        // Sunucu tarafında çarpışma olayını işle
        HandleCollision();
    }

    private void HandleCollision()
    {
        // Sunucu tarafında canavarı yok et
        DestroyEnemy();
        

        // Tüm istemcilere canavarın yok edilmesini bildir
        DestroyEnemyClientRpc();
    }

    [ClientRpc]
    private void DestroyEnemyClientRpc()
    {
        // İstemci tarafında canavarı yok et
        DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        // Canavarı yok et
        Destroy(gameObject);
    }
}