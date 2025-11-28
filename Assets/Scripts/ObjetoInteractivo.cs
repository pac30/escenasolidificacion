using UnityEngine;

/// <summary>
/// Objeto interactivo: hover muestra explicacion larga, interact ejecuta acción.
/// </summary>
public class ObjetoInteractivo : MonoBehaviour
{
    [TextArea]
    public string mensajeExplicacion;

    [Header("Tipo (marcar según el objeto)")]
    public bool esNevera;
    public bool esOlla;
    public bool esBotonEstufa;

    [Header("Mensajes cortos (al interactuar)")]
    [TextArea] public string mensajeAccionNevera = "Has tomado un cubo de hielo.";
    [TextArea] public string mensajeAccionOlla = "Has colocado el hielo en la olla.";
    [TextArea] public string mensajeAccionEstufaEncendida = "La estufa está encendida.";
    [TextArea] public string mensajeAccionEstufaApagada = "La estufa se ha apagado.";
    [TextArea] public string mensajeSinAccion = "No hay acción para este objeto.";

    [Header("Referencias")]
    public GameObject prefabHielo;      // prefab del cubo de hielo para instanciar
    public ControlFusion olla;          // referencia al controller de la olla (ControlFusion)

    // referencias cacheadas
    MensajeVRPro mensajeVR;
    InventarioJugador jugador;
    UIExplicacionLaboratorio uiExplicacion;

    void Awake()
    {
        mensajeVR = FindObjectOfType<MensajeVRPro>();
        jugador = FindObjectOfType<InventarioJugador>();
        uiExplicacion = FindObjectOfType<UIExplicacionLaboratorio>();
    }

    // Hover enter
    public void OnHoverEnter()
    {
        if (!string.IsNullOrEmpty(mensajeExplicacion))
        {
            // Mostrar explicación larga en HUD; la ocultaremos en OnHoverExit
            mensajeVR?.MostrarMensaje(mensajeExplicacion, 999f); // duración larga, ocultamos manualmente
            // También actualizar panel UI si quieres (opcional)
            if (uiExplicacion != null)
                uiExplicacion.MostrarExplicacion(mensajeExplicacion);
        }
    }

    // Hover exit
    public void OnHoverExit()
    {
        // ocultar mensaje mostrado por hover
        mensajeVR?.OcultarAhora();
    }

    // Acción (gatillo)
    public void Interactuar()
    {
        Debug.Log("Interacción con " + gameObject.name);

        // intentar asegurar referencias dinámicas
        if (mensajeVR == null) mensajeVR = FindObjectOfType<MensajeVRPro>();
        if (jugador == null) jugador = FindObjectOfType<InventarioJugador>();

        // NEVERA: tomar hielo (instanciar prefab en mano)
        if (esNevera && prefabHielo != null && jugador != null)
        {
            jugador.TomarHielo(prefabHielo);
            mensajeVR?.MostrarMensaje(mensajeAccionNevera);
            return;
        }

        // OLLA: colocar hielo en la olla
        if (esOlla && olla != null && jugador != null)
        {
            jugador.ColocarHieloEnOlla(olla);
            mensajeVR?.MostrarMensaje(mensajeAccionOlla);
            return;
        }

        // BOTON ESTUFA: toggle
        if (esBotonEstufa)
        {
            var control = FindObjectOfType<ControlFusion>();
            if (control != null)
            {
                control.ToggleEstufa();

                // Mostrar mensaje según nuevo estado
                if (control.EstufaEncendida)
                    mensajeVR?.MostrarMensaje(mensajeAccionEstufaEncendida);
                else
                    mensajeVR?.MostrarMensaje(mensajeAccionEstufaApagada);
            }
            return;
        }

        // Si no aplica
        mensajeVR?.MostrarMensaje(mensajeSinAccion);
    }
}
