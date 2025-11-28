using UnityEngine;

public class InventarioJugador : MonoBehaviour
{
    public bool tieneHielo = false;
    public GameObject hieloEnMano; // referencia visual opcional

    // Método para tomar el hielo
    public void TomarHielo(GameObject prefabHielo)
    {
        if (!tieneHielo)
        {
            tieneHielo = true;

            // Instanciar el hielo en la mano del jugador (opcional)
            if (hieloEnMano != null && prefabHielo != null)
            {
                GameObject hielo = Instantiate(prefabHielo, hieloEnMano.transform.position, Quaternion.identity);
                hielo.transform.SetParent(hieloEnMano.transform);
            }

            Debug.Log("El jugador ha tomado un cubo de hielo.");
        }
    }

    // Método para colocar el hielo en la olla
    public void ColocarHieloEnOlla(ControlFusion controlFusion)
    {
        if (tieneHielo && controlFusion != null)
        {
            tieneHielo = false;

            // Si tenías un hielo en mano, lo destruyes
            foreach (Transform child in hieloEnMano.transform)
            {
                Destroy(child.gameObject);
            }

            // Activar el hielo de la olla
            if (controlFusion.hielo != null)
            {
                controlFusion.hielo.SetActive(true);
                controlFusion.ResetProceso(); // reiniciar temperatura
            }

            Debug.Log("El jugador colocó el hielo en la olla.");
        }
    }
}