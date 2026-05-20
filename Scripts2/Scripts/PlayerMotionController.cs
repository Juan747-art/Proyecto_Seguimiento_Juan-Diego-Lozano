using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotionController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public Animator animator;
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 120.0f; 
    public KeyCode holdBreathKey = KeyCode.LeftShift;

    [Header("Variables de Estado")]
    public float anxietyLevel = 0f;
    private bool isWalking;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Rotación con A y D (Eje Horizontal)
        float rotX = Input.GetAxis("Horizontal");
        // Giramos al personaje en el eje Y (arriba/abajo) basado en la tecla que presiones
        transform.Rotate(0, rotX * rotationSpeed * Time.deltaTime, 0);

        // 2. Movimiento Adelante/Atrás con W y S (Eje Vertical)
        float moveZ = Input.GetAxis("Vertical");
      
        Vector3 move = transform.forward * moveZ;

        // Aplicar el movimiento físico
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 3. Determinar si estamos caminando para las animaciones
        // Usamos Mathf.Abs para detectar si te mueves hacia adelante o hacia atrás
        isWalking = Mathf.Abs(moveZ) > 0.1f;

        // 4. Control de Respiración
        bool holdingBreath = Input.GetKey(holdBreathKey);

        // 5. Lógica de Ansiedad
        if (isWalking && !holdingBreath)
        {
            anxietyLevel = Mathf.Lerp(anxietyLevel, 1f, Time.deltaTime * 0.5f);
        }
        else if (holdingBreath)
        {
            anxietyLevel = Mathf.Lerp(anxietyLevel, 0f, Time.deltaTime * 2f);
        }

        // 6. Actualizar Animator
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isHoldingBreath", holdingBreath);
            animator.SetFloat("breathingIntensity", anxietyLevel);
        }
    }
}