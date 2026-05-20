using UnityEngine;
using UnityEngine.EventSystems; // ¡VITAL para controlar el foco!
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Configuración del Menú")]
    public string gameSceneName = "Principal";
    public GameObject firstSelectedButton; // Tu botón de Play

    private GameObject ultimoBotonSeleccionado;

    void Start()
    {
        // Forzar la selección inicial al arrancar el juego
        EnfocarBotonInicial();
    }

    void Update()
    {
        // PROTECCIÓN 1: Si pasa el tiempo y el usuario deselecciona el botón sin querer,
        // o si el EventSystem se queda flotando en el vacío, re-enfocamos el último botón válido.
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                // Si teníamos un botón guardado, regresamos a ese, si no, al de Play inicial
                if (ultimoBotonSeleccionado != null && ultimoBotonSeleccionado.activeInHierarchy)
                {
                    EventSystem.current.SetSelectedGameObject(ultimoBotonSeleccionado);
                }
                else
                {
                    EnfocarBotonInicial();
                }
            }
            else
            {
                // Si hay un botón seleccionado legalmente por el joystick, guardamos la referencia
                ultimoBotonSeleccionado = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    // PROTECCIÓN 2: Se ejecuta automáticamente en Android al bloquear/desbloquear el celular
    void OnApplicationFocus(bool hasFocus)
    {
        // Si el juego recupera el foco tras un bloqueo de pantalla
        if (hasFocus)
        {
            StartCoroutine(ReenfocarConDelay());
        }
    }

    // Pequeña corrutina de retraso para esperar a que los hilos de Android se sincronicen de nuevo
    private System.Collections.IEnumerator ReenfocarConDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        EnfocarBotonInicial();
    }

    private void EnfocarBotonInicial()
    {
        if (EventSystem.current != null && firstSelectedButton != null)
        {
            // Limpiamos selección actual y forzamos el foco en el botón objetivo
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            ultimoBotonSeleccionado = firstSelectedButton;
            Debug.Log("Foco del menú VR restaurado con éxito.");
        }
    }

    // Tus funciones existentes para los botones
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}