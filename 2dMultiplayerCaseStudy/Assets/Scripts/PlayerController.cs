using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public float rotationSpeed = 5f; // Dönme hızı

    public float moveSpeed = 5f; // Hareket hızı
    private Rigidbody2D rb; // Rigidbody2D referansı
    private Vector2 moveDirection; // Hareket yönü

    public float minDistance = 0.1f; // Minimum mesafe

    void Start()
    {
        // Rigidbody2D bileşenini al
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {


        // "A" tuşuna basıldığında rotation z arttır
        if (Input.GetKey(KeyCode.A))
        {
            RotateCharacter(rotationSpeed);
        }

        // "D" tuşuna basıldığında rotation z azalt
        if (Input.GetKey(KeyCode.D))
        {
            RotateCharacter(-rotationSpeed);
        }



        // Fare pozisyonunu dünya uzayında al
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D ortamda olduğumuz için Z konumunu sıfıra ayarlayın

        // Karakterin pozisyonunu al
        Vector3 playerPosition = transform.position;

        // Fare pozisyonuna doğru yönelen bir vektör oluştur
        moveDirection = (mousePosition - playerPosition).normalized;

        // Karakter ile fare arasındaki mesafayı kontrol et
        if (Vector3.Distance(playerPosition, mousePosition) < minDistance)
        {
            // Eğer mesafe minDistance'tan küçükse, hareket yönünü sıfırla
            moveDirection = Vector2.zero;
        }
    }


    void RotateCharacter(float amount)
    {
        // Karakterin rotasyonunu belirle
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float newRotationZ = currentRotation.z + amount;
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newRotationZ);
    }




    void FixedUpdate()
    {
        // Hareketi uygula
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }


    //Girl

    // [SerializeField] private float speed = 3f;
    // private Camera _mainCamera;
    // private Vector3 _mouseInput;
    //
    // private void Initialize()
    // {
    //     _mainCamera = Camera.main;
    // }
    //
    // public override void OnNetworkSpawn()
    // {
    //     base.OnNetworkSpawn();
    //     Initialize();
    // }
    //
    // private void Update()
    // {
    //     if(!Application.isFocused) return;
    //     //Movement
    //     _mouseInput.x = Input.mousePosition.x;
    //     _mouseInput.y = Input.mousePosition.y;
    //     _mouseInput.z = _mainCamera.nearClipPlane;
    //     Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
    //     transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);
    //
    //     //Rotate
    //     if (mouseWorldCoordinates != transform.position)
    //     {
    //         Vector3 targetDirection = mouseWorldCoordinates - transform.position;
    //         transform.up = targetDirection;
    //     }
    //     
    // }
}
    
