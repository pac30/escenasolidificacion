using UnityEngine;
using TMPro;

/// <summary>
/// Control del proceso: hielo -> agua por calentamiento, con punto de fusión (0°C).
/// </summary>
public class ControlFusion : MonoBehaviour
{
    [Header("Referencias de escena")]
    public GameObject hielo;
    public GameObject agua;
    public Light luzEstufa;
    public TextMeshProUGUI textoUI;

    [Header("Ajustes")]
    // Velocidades ajustadas para un proceso gradual
    public float velocidadAumentoTemp = 10f; // Sube la temperatura a 10 grados por segundo
    public float velocidadDerretir = 0.005f;  // Reduce la escala del hielo lentamente
    public float velocidadSubidaAgua = 0.05f;

    // estado
    bool estufaEncendida = false;
    float temperatura = 0f;
    bool transicionHieloAguaCompleta = false; // Controla que la activación del agua se haga solo una vez

    Vector3 escalaInicialAgua;
    Vector3 posicionInicialAgua;

    void Start()
    {
        if (agua != null)
        {
            escalaInicialAgua = agua.transform.localScale;
            posicionInicialAgua = agua.transform.position;
            agua.SetActive(false);
        }

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        if (luzEstufa != null) luzEstufa.enabled = estufaEncendida;
    }

    void Update()
    {
        if (!estufaEncendida) return;

        temperatura += Time.deltaTime * velocidadAumentoTemp;
        if (textoUI != null) textoUI.text = "Temperatura: " + (int)temperatura + " °C";

        // Derretir hielo: solo ocurre si la estufa está encendida Y la temperatura alcanza 0°C
        if (hielo != null && hielo.activeSelf && temperatura >= 0f)
        {
            // 1. Reducir la escala del hielo
            hielo.transform.localScale -= Vector3.one * (velocidadDerretir * Time.deltaTime);

            // 2. Transición a agua: solo si la escala es mínima Y la transición NO ha ocurrido
            if (hielo.transform.localScale.x <= 0.05f && !transicionHieloAguaCompleta)
            {
                // Apagar el hielo
                hielo.transform.localScale = Vector3.zero;
                hielo.SetActive(false);

                // Activar el agua (se ejecuta solo una vez)
                if (agua != null)
                {
                    agua.SetActive(true);
                    agua.transform.localScale = new Vector3(escalaInicialAgua.x, 0.01f, escalaInicialAgua.z);
                    agua.transform.position = posicionInicialAgua;

                    transicionHieloAguaCompleta = true; // Marcar como completa
                }
            }
        }

        // Subir agua: solo si el objeto 'agua' está activo
        if (agua != null && agua.activeSelf)
        {
            Vector3 esc = agua.transform.localScale;
            if (esc.y < escalaInicialAgua.y)
            {
                esc.y += Time.deltaTime * velocidadSubidaAgua;
                agua.transform.localScale = esc;

                Vector3 pos = agua.transform.position;
                pos.y += Time.deltaTime * (velocidadSubidaAgua * 0.5f);
                agua.transform.position = pos;
            }
        }
    }

    public void ToggleEstufa()
    {
        estufaEncendida = !estufaEncendida;
        if (luzEstufa != null) luzEstufa.enabled = estufaEncendida;
        Debug.Log("Estufa: " + (estufaEncendida ? "ENCENDIDA" : "APAGADA"));
    }

    public bool EstufaEncendida => estufaEncendida;

    public void ResetProceso()
    {
        // Reiniciar todas las variables de estado
        transicionHieloAguaCompleta = false;
        temperatura = 0f;
        estufaEncendida = false;
        if (luzEstufa != null) luzEstufa.enabled = false;

        if (textoUI != null) textoUI.text = "Temperatura: 0 °C";

        if (agua != null) agua.SetActive(false);

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
}