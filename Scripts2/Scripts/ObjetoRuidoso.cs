using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjetoRuidoso : MonoBehaviour
{
    [Header("Configuración del Ruido")]
    public float penalizacionAnsiedad = 20f; // 1 bloque exacto
    public AudioClip sonidoRuido;            // El sonido específico de este objeto

    private AudioSource audioSource;
    private bool jugadorAdentro = false;     // Evita que suba 100 puntos en un segundo

    void Start()
    {
        // Configuramos el reproductor de sonido automáticamente
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = sonidoRuido;
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Verificamos que sea el jugador quien chocó
        if (other.CompareTag("Player") && !jugadorAdentro)
        {
            jugadorAdentro = true;

            // 2. Reproducir el sonido
            if (sonidoRuido != null)
            {
                audioSource.Play();
            }

            // 3. Aplicar la ansiedad instantánea
            PlayerAnxiety scriptJugador = other.GetComponent<PlayerAnxiety>();
            if (scriptJugador != null)
            {
                scriptJugador.AddInstantAnxiety(penalizacionAnsiedad);
            }
        }
    }

    // Cuando el jugador se aleja del objeto, lo reseteamos por si vuelve a chocar más tarde
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorAdentro = false;
        }
    }
}