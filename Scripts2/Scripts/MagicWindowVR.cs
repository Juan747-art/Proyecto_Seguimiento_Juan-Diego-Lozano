using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class MagicWindowVR : MonoBehaviour
{
    [Header("Movimiento y Sensores")]
    public float speed = 3.0f;
    public float horizontalSensitivity = 1.5f;
    public float gravity = 9.8f;

    [Header("Sistema de Ansiedad")]
    public Animator animator;
    public KeyCode xboxHoldBreathKey = KeyCode.JoystickButton4; // LB en Xbox
    public float anxietyLevel = 0f;

    [Header("Interfaz de Misión")]
    public TextMeshProUGUI textoDialogo;
    public float tiempoVisible = 6.0f;

    private CharacterController characterController;
    private Transform playerBody; // Almacena la referencia directa al Padre (Player)
    private bool gyroActive;
    private bool isWalking;

    private float rotacionXManual = 0f;
    private float rotacionYManual = 0f;

    void Start()
    {
        // Guardamos la referencia al cuerpo del jugador (Padre)
        if (transform.parent != null)
        {
            playerBody = transform.parent;
            characterController = playerBody.GetComponent<CharacterController>();
        }

        // 1. Detección de Hardware (Giroscopio)
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            gyroActive = true;
            Debug.Log("Giroscopio detectado: Modo VR activado.");
        }
        else
        {
            Debug.LogWarning("Sin giroscopio: Modo Fallback (Táctil/Mando) activado.");
        }

        // Sincronización inicial de rotación
        rotacionXManual = transform.localEulerAngles.x;
        if (playerBody != null)
            rotacionYManual = playerBody.localEulerAngles.y;

        if (textoDialogo != null)
            StartCoroutine(OcultarDialogoAlInicio());
    }

    IEnumerator OcultarDialogoAlInicio()
    {
        yield return new WaitForSeconds(tiempoVisible);
        if (textoDialogo != null) textoDialogo.gameObject.SetActive(false);
    }

    void Update()
    {
        // === 1. LECTURA DE ROTACIÓN (SISTEMA HÍBRIDO) ===
        float lookX = 0f;
        float lookY = 0f;

        // Intento A: Input System Moderno (Mando de Xbox)
        if (Gamepad.current != null)
        {
            Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
            lookX = rightStick.x;
            lookY = rightStick.y;
        }

        // Intento B: Fallback Legacy para inputs clásicos
        if (Mathf.Abs(lookX) < 0.01f && Mathf.Abs(lookY) < 0.01f)
        {
            lookX = Input.GetAxis("Mouse X");
            lookY = Input.GetAxis("Mouse Y");
        }

        // === 2. ROTACIÓN DEL CUERPO (Eje Y - Solo afecta al Padre) ===
        // El stick derecho rota los pies/cuerpo del personaje en el mundo real
        if (playerBody != null && Mathf.Abs(lookX) > 0.01f)
        {
            rotacionYManual += lookX * horizontalSensitivity * 2f;
            playerBody.localRotation = Quaternion.Euler(0f, rotacionYManual, 0f);
        }

        // === 3. ROTACIÓN DE LA CÁMARA (Visor Cardboard) ===
        if (gyroActive)
        {
            // MODO VR (Motorola): El giroscopio maneja la vista local RELATIVA al cuerpo
            Quaternion att = Input.gyro.attitude;
            // Inversión de matrices estándar para adaptar sensores móviles a Unity
            Quaternion rawGyro = new Quaternion(att.x, att.y, -att.z, -att.w);
            transform.localRotation = Quaternion.Euler(90f, 0f, 0f) * rawGyro;
        }
        else
        {
            // MODO FALLBACK (vivo Y03): Control táctil en pantalla plana
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == UnityEngine.TouchPhase.Moved)
                {
                    float touchX = touch.deltaPosition.x * 0.1f;
                    lookY += touch.deltaPosition.y * 0.1f;

                    rotacionYManual += touchX * horizontalSensitivity;
                    if (playerBody != null)
                        playerBody.localRotation = Quaternion.Euler(0f, rotacionYManual, 0f);
                }
            }

            rotacionXManual -= lookY * horizontalSensitivity * 2f;
            rotacionXManual = Mathf.Clamp(rotacionXManual, -80f, 80f);
            transform.localRotation = Quaternion.Euler(rotacionXManual, 0f, 0f);
        }

        // === 4. SISTEMA DE ANSIEDAD ===
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        isWalking = Mathf.Abs(v) > 0.1f || Mathf.Abs(h) > 0.1f;

        bool holdingBreath = Input.GetKey(xboxHoldBreathKey) || Input.GetKey(KeyCode.LeftShift);

        if (isWalking && !holdingBreath)
            anxietyLevel = Mathf.Lerp(anxietyLevel, 1f, Time.deltaTime * 0.5f);
        else if (holdingBreath)
            anxietyLevel = Mathf.Lerp(anxietyLevel, 0f, Time.deltaTime * 2f);

        // === 5. FÍSICAS DE MOVIMIENTO VR (Dirección Absolute Forward) ===
        if (characterController != null)
        {
            if (isWalking)
            {
                // Calculamos vectores de dirección basados en la mirada actual del jugador
                Vector3 camForward = transform.forward;
                Vector3 camRight = transform.right;

                // Forzamos proyección al suelo (y = 0) para evitar que flote o se entierre en el pasillo
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                // Dirección final del vector de movimiento traslacional
                Vector3 moveDir = (camForward * v) + (camRight * h);

                // Aplicamos velocidad constante afectada por la gravedad base
                Vector3 velocity = moveDir * speed;
                velocity.y = -gravity;

                characterController.Move(velocity * Time.deltaTime);
            }
            else
            {
                // Mantener gravedad activa estable para evitar fugas del Character Controller
                characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            }
        }

        // === 6. ANIMATOR ===
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isHoldingBreath", holdingBreath);
            animator.SetFloat("breathingIntensity", anxietyLevel);
        }
    }
}