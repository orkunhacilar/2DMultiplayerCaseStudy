using UnityEngine;
using Unity.Netcode;

public class HpPot : NetworkBehaviour
{
    public int healAmount = 30; // Can artış miktarı

    public void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Potu yok et
            if (player.IsLocalPlayer) // Sadece yerel oyuncunun sahip olduğu player'a etki edecek
            {
                Debug.Log("Pota Carpti, Can Artiriliyor");
                if (NetworkManager.Singleton.IsServer) // Eğer host ise
                {
                    Debug.Log("Host icin can degistiriliyor.....");
                    player.HealCharServerRpc(healAmount);
                }
                else // Eğer istemci ise
                {
                    Debug.Log("Client icin can degistiriliyor....");
                    player.HealCharClientRpc(healAmount);
                }
            }
        }
    }
}