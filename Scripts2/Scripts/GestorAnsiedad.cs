using UnityEngine;
using UnityEngine.UI; // Importante para manejar la UI

public class GestorAnsiedad : MonoBehaviour
{
    [Header("Configuración de Ansiedad")]
    [Range(0, 100)] public float ansiedadActual = 0f;
    [Range(0, 100)] public float umbralEfecto = 30f; // Nivel de ansiedad objetivo (30%)
    public Image imagenAnsiedad; // Arrastra tu Imagen_Ansiedad aquí

    [Header("Efectos Visuales")]
    public float opacidadMaximaEfecto = 0.3f; // Opacidad final deseada (30%)
    public float suavizadoEfecto = 2f; // Qué tan rápido cambia el efecto

    void Update()
    {
        // En un juego real, la ansiedad sube aquí por scripts externos
        ActualizarEfectosVisuales();
    }

    void ActualizarEfectosVisuales()
    {
        if (imagenAnsiedad == null) return;

        // Calculamos la opacidad basándonos en el nivel de ansiedad actual, escalando
        // hasta la opacidad máxima deseada al 30% de ansiedad.
        float alphaDestino = (ansiedadActual / umbralEfecto) * opacidadMaximaEfecto;
        // Nos aseguramos de no superar la opacidad máxima deseada
        alphaDestino = Mathf.Clamp(alphaDestino, 0f, opacidadMaximaEfecto);

        // Aplicamos los cambios con suavidad (Lerp) para no marear en VR
        Color colorActual = imagenAnsiedad.color;
        colorActual.a = Mathf.Lerp(imagenAnsiedad.color.a, alphaDestino, Time.deltaTime * suavizadoEfecto);
        imagenAnsiedad.color = colorActual;
    }
}