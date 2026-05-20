using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Configuración del Ratón")]
    public float mouseSensitivity = 300f; // Qué tan rápido gira la cámara
    public Transform playerBody; 

    private float xRotation = 0f;

    void Start()
    {
        // Esto oculta la flecha del ratón y la bloquea en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Obtener hacia dónde estás moviendo el ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Calcular la rotación vertical (mirar hacia el techo o el suelo)
        xRotation -= mouseY;
        // El Clamp evita que te rompas el cuello (limita la vista a 90 grados arriba y abajo)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 3. Aplicar rotación a la cámara (arriba/abajo)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 4. Aplicar rotación al jugador entero (izquierda/derecha)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}