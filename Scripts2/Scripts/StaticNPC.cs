using UnityEngine;

public class StaticNPC : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform player; 

    [Header("Configuración de Visión")]
    public float viewRadius = 10f; // Qué tan lejos ve
    [Range(0, 360)] public float viewAngle = 90f; // Amplitud del cono de visión
    public float anxietyRate = 20f; // Cuánta ansiedad suma por segundo al verte

    [Header("Físicas")]
    public LayerMask obstacleMask; // Qué cosas bloquean su vista (Paredes/Casilleros)

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        // Levantamos el origen del rayo 1.5 metros hacia arriba (altura de los ojos/pecho)
        Vector3 npcEyes = transform.position + Vector3.up * 1.5f;
        Vector3 playerChest = player.position + Vector3.up * 1.5f;

        // Calculamos la dirección y la distancia con las nuevas alturas
        Vector3 directionToPlayer = (playerChest - npcEyes).normalized;
        float distanceToPlayer = Vector3.Distance(npcEyes, playerChest);

        // 1. ¿El jugador está lo suficientemente cerca?
        if (distanceToPlayer < viewRadius)
        {
            // 2. ¿El jugador está dentro del ángulo de visión frontal?
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2f)
            {
                // 3. El Raycast disparando desde los ojos
                if (!Physics.Raycast(npcEyes, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    // NO chocó con nada. ¡El NPC te está viendo!
                    Debug.DrawRay(npcEyes, directionToPlayer * distanceToPlayer, Color.red);

                    // Conectamos con el script de ansiedad del jugador
                    PlayerAnxiety pa = player.GetComponent<PlayerAnxiety>();
                    if (pa != null)
                    {
                        pa.IncreaseAnxiety(anxietyRate);
                    }
                }
                else
                {
                    // Chocó con un casillero o pared. Estás a salvo.
                    Debug.DrawRay(npcEyes, directionToPlayer * distanceToPlayer, Color.green);
                }
            }
        }
    }

    // Esto dibuja un círculo amarillo en Unity para que veas el radio de visión del NPC
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}