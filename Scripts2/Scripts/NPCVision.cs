using UnityEngine;

public class NPCVision : MonoBehaviour
{
    [Header("Configuración de Visión")]
    public Transform puntoDeVision; // Un objeto vacío en la cabeza del NPC
    public float distanciaVision = 8f; // Qué tan lejos puede ver (en metros)
    [Range(0, 180)]
    public float anguloDeVision = 90f; // 90 grados es una visión periférica normal

    [Header("Penalización")]
    public float ansiedadPorSegundo = 15f; // Lo que le subirá al jugador

    [Header("Simulación (Opcional)")]
    public bool girarPorCodigo = true; // Para probar antes de tener animaciones
    public float velocidadGiro = 2f;
    public float anguloMaximoGiro = 45f;

    private Transform jugador;
    private PlayerAnxiety scriptJugador;
    private Quaternion rotacionInicial;

    void Start()
    {
        rotacionInicial = transform.rotation;

        // Buscamos al jugador automáticamente
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
            scriptJugador = objJugador.GetComponent<PlayerAnxiety>();
        }
    }

    void Update()
    {
        // 1. Simular el giro si aún no tienes el Animator configurado
        if (girarPorCodigo)
        {
            SimularAnimacionMirar();
        }

        // 2. Detectar al jugador
        DetectarJugador();
    }

    void DetectarJugador()
    {
        if (jugador == null) return;

        // Pregunta 1: ¿Está el jugador lo suficientemente cerca?
        float distanciaAlJugador = Vector3.Distance(puntoDeVision.position, jugador.position);
        if (distanciaAlJugador <= distanciaVision)
        {
            // Pregunta 2: ¿Está dentro del cono de visión?
            Vector3 direccionAlJugador = (jugador.position - puntoDeVision.position).normalized;
            float anguloAlJugador = Vector3.Angle(puntoDeVision.forward, direccionAlJugador);

            if (anguloAlJugador <= anguloDeVision / 2f)
            {
                // Pregunta 3: ¿El jugador está escondido detrás de algo? (Raycast)
                // Lanzamos un láser invisible desde los ojos del NPC hasta el jugador
                if (Physics.Raycast(puntoDeVision.position, direccionAlJugador, out RaycastHit hit, distanciaVision))
                {
                    // Si el láser choca con el jugador... ¡Nos está viendo!
                    if (hit.collider.CompareTag("Player"))
                    {
                        if (scriptJugador != null)
                        {
                            scriptJugador.IncreaseAnxiety(ansiedadPorSegundo);
                            // Descomenta la siguiente línea si quieres ver en consola cuando te mira:
                            // Debug.Log("¡El NPC te está juzgando con la mirada!");
                        }
                    }
                }
            }
        }
    }

    // Un pequeño truco matemático para que gire la cabeza sin necesitar animaciones complejas aún
    void SimularAnimacionMirar()
    {
        float anguloActual = Mathf.Sin(Time.time * velocidadGiro) * anguloMaximoGiro;
        transform.rotation = rotacionInicial * Quaternion.Euler(0, anguloActual, 0);
    }

    // Esto dibuja el cono en el editor para que puedas ajustar la distancia y el ángulo visualmente
    void OnDrawGizmosSelected()
    {
        if (puntoDeVision != null)
        {
            Gizmos.color = Color.red;
            // Dibuja la línea de distancia máxima
            Gizmos.DrawWireSphere(puntoDeVision.position, distanciaVision);

            // Dibuja las líneas laterales del cono
            Vector3 limiteDerecho = Quaternion.Euler(0, anguloDeVision / 2, 0) * puntoDeVision.forward;
            Vector3 limiteIzquierdo = Quaternion.Euler(0, -anguloDeVision / 2, 0) * puntoDeVision.forward;
            Gizmos.DrawRay(puntoDeVision.position, limiteDerecho * distanciaVision);
            Gizmos.DrawRay(puntoDeVision.position, limiteIzquierdo * distanciaVision);
        }
    }
}