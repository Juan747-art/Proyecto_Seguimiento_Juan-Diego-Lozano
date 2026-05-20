using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class InteraccionPuerta : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaInteraccion = 3f;
    public string mensajePuertaCerrada = "Parece que esta no es...";

    [Header("¿Es la Salida?")]
    public bool esPuertaDeSalida = false; // Marca esto solo en la última puerta
    public string nombreEscenaMenu = "MenuPrincipal";

    [Header("Referencias")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoPuertaCerrada;
    public TMPro.TextMeshProUGUI textoUI;

    private GameObject jugador;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Detectar Tecla E o Botón X del mando (JoystickButton2)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            if (Vector3.Distance(transform.position, jugador.transform.position) <= distanciaInteraccion)
            {
                Interactuar();
            }
        }
    }

    void Interactuar()
    {
        if (esPuertaDeSalida)
        {
            // Lógica de Victoria
            Debug.Log("¡Ganaste! Volviendo al menú...");
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            // Lógica de Puerta Falsa
            if (fuenteAudio != null && sonidoPuertaCerrada != null)
                fuenteAudio.PlayOneShot(sonidoPuertaCerrada);

            if (textoUI != null)
                StartCoroutine(MostrarMensaje());
        }
    }

    System.Collections.IEnumerator MostrarMensaje()
    {
        textoUI.text = mensajePuertaCerrada;
        textoUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        textoUI.gameObject.SetActive(false);
    }
}