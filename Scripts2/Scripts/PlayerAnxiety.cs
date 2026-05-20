using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAnxiety : MonoBehaviour
{
    [Header("Configuración de Interfaz")]
    public Slider anxietySlider;

    [Header("Valores de Ansiedad")]
    public float currentAnxiety = 0f;
    public float maxAnxiety = 100f;

    [Header("Mecánica de Respiración")]
    public KeyCode breathKey = KeyCode.LeftShift;
    public float recoveryRate = 10f;
    public float maxRecoveryPerBreath = 20f;
    public float breathCooldown = 30f;

    [Header("Efectos Visuales (Visión de Túnel)")]
    public Image imagenAnsiedad;
    public float umbralEfecto = 30f; // A partir de qué nivel o cómo escala
    public float opacidadMaximaEfecto = 0.5f;
    public float suavizadoEfecto = 10f;

    [Header("Efectos Físicos (Temblor de Cámara)")]
    public Transform camaraJugador;
    public float intensidadTemblor = 0.05f;

    // Variables internas de control
    private float nextBreathTime = 0f;
    private float amountRecoveredThisBreath = 0f;
    private bool isBreathing = false;
    private Vector3 posicionOriginalCamara;

    void Start()
    {
        // Guardamos la posición original (relativa) de la cámara para el temblor
        if (camaraJugador != null)
        {
            posicionOriginalCamara = camaraJugador.localPosition;
        }

        // --- SOLUCIÓN AL DESTELLO INICIAL ---
        if (imagenAnsiedad != null)
        {
            Color colorInicial = imagenAnsiedad.color;
            colorInicial.a = 0f; // Forzamos el Alpha a 0 de forma instantánea
            imagenAnsiedad.color = colorInicial;
        }
    }

    void Update()
    {
        // === 1. MECÁNICAS DE JUGABILIDAD ===

        // Iniciar la respiración
        if (Input.GetKeyDown(breathKey) && Time.time >= nextBreathTime && currentAnxiety > 0)
        {
            isBreathing = true;
            amountRecoveredThisBreath = 0f;
        }

        // Proceso gradual de respiración
        if (isBreathing)
        {
            if (Input.GetKey(breathKey))
            {
                float step = recoveryRate * Time.deltaTime;

                if (amountRecoveredThisBreath + step >= maxRecoveryPerBreath || currentAnxiety - step <= 0)
                {
                    float allowedStep = Mathf.Min(step, maxRecoveryPerBreath - amountRecoveredThisBreath, currentAnxiety);
                    currentAnxiety -= allowedStep;
                    amountRecoveredThisBreath += allowedStep;
                    TerminarRespiracion();
                }
                else
                {
                    currentAnxiety -= step;
                    amountRecoveredThisBreath += step;
                }
            }
            else
            {
                TerminarRespiracion();
            }
        }

        if (Input.GetKeyDown(breathKey) && Time.time < nextBreathTime && !isBreathing)
        {
            float tiempoRestante = nextBreathTime - Time.time;
            Debug.Log("Pulmones recuperándose... faltan " + Mathf.Round(tiempoRestante) + "s");
        }

        // Seguridad de límites
        currentAnxiety = Mathf.Clamp(currentAnxiety, 0, maxAnxiety);

        // === 2. ACTUALIZACIÓN VISUAL Y FÍSICA ===

        // Actualizar UI del Slider
        if (anxietySlider != null)
        {
            anxietySlider.value = currentAnxiety;
        }

        // Desplegar efectos basados en currentAnxiety
        ActualizarVisiónTúnel();
        ActualizarTemblor();

        // === 3. CONDICIÓN DE DERROTA ===
        if (currentAnxiety >= maxAnxiety)
        {
            GameOver();
        }
    }

    // Métodos de Efectos
    void ActualizarVisiónTúnel()
    {
        if (imagenAnsiedad == null) return;

        // La opacidad sube automáticamente a medida que currentAnxiety se acerca al umbral
        float alphaDestino = (currentAnxiety / umbralEfecto) * opacidadMaximaEfecto;
        alphaDestino = Mathf.Clamp(alphaDestino, 0f, opacidadMaximaEfecto);

        Color colorActual = imagenAnsiedad.color;
        colorActual.a = Mathf.Lerp(imagenAnsiedad.color.a, alphaDestino, Time.deltaTime * suavizadoEfecto);
        imagenAnsiedad.color = colorActual;
    }

    void ActualizarTemblor()
    {
        if (camaraJugador == null) return;

        // El temblor se activa si la ansiedad supera un nivel bajo (ej. 10)
        if (currentAnxiety > 10f)
        {
            float factorAnsiedad = (currentAnxiety - 10f) / (maxAnxiety - 10f);

            float offsetX = (Mathf.PerlinNoise(Time.time * 10f, 0f) - 0.5f) * 2f;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * 10f) - 0.5f) * 2f;

            Vector3 temblor = new Vector3(offsetX, offsetY, 0f) * intensidadTemblor * factorAnsiedad;
            camaraJugador.localPosition = posicionOriginalCamara + temblor;
        }
        else
        {
            camaraJugador.localPosition = Vector3.Lerp(camaraJugador.localPosition, posicionOriginalCamara, Time.deltaTime * 5f);
        }
    }

    // Métodos de Control
    void TerminarRespiracion()
    {
        isBreathing = false;
        nextBreathTime = Time.time + breathCooldown;
        Debug.Log("Respiración finalizada. Se curó: " + Mathf.Round(amountRecoveredThisBreath) + ". Cooldown de 30s iniciado.");
    }

    public void IncreaseAnxiety(float amountPerSecond)
    {
        currentAnxiety += amountPerSecond * Time.deltaTime;
    }

    public void AddInstantAnxiety(float amount)
    {
        currentAnxiety += amount;
        Debug.Log("¡Autosugestión por ruido! Ansiedad + " + amount);
    }

    void GameOver()
    {
        Debug.Log("¡Pánico total! Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}