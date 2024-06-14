using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public float moveSpeed = 5f; // Hareket hızı

    // Update her frame'de çağrılır
    void Update()
    {
        // W, A, S, D tuşlarıyla hareket etme
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Yön vektörü oluştur
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);

        // Düzenleme yap
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
    
