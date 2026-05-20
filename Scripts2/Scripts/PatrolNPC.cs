using UnityEngine;

public class PatrolNPC : MonoBehaviour
{
    [Header("Configuración de Patrullaje")]
    public Transform[] waypoints; // Lista de puntos a los que caminará
    public float moveSpeed = 2f; // Velocidad de caminata
    private int currentWaypointIndex = 0;

    [Header("Objetivos")]
    public Transform player;

    [Header("Configuración de Visión")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public float anxietyRate = 20f;

    [Header("Físicas")]
    public LayerMask obstacleMask;

    void Update()
    {
        Patrol();
        DetectPlayer();
    }

    void Patrol()
    {
        // Si no le hemos puesto puntos de ruta, no hace nada
        if (waypoints.Length == 0) return;

        // 1. Identificar hacia dónde vamos
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Mantenemos la altura actual del NPC para que no intente volar o hundirse
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);

        // 2. Moverse hacia ese punto
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 3. Rotar el cuerpo para mirar hacia donde camina (Esto hace que el cono de visión gire)
        Vector3 directionToWaypoint = (targetPosition - transform.position).normalized;
        if (directionToWaypoint != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToWaypoint);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // 4. Si llegó al punto, cambiar al siguiente punto en la lista
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // El % hace que vuelva al 0 al terminar
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        Vector3 npcEyes = transform.position + Vector3.up * 1.5f;
        Vector3 playerChest = player.position + Vector3.up * 1.5f;

        Vector3 directionToPlayer = (playerChest - npcEyes).normalized;
        float distanceToPlayer = Vector3.Distance(npcEyes, playerChest);

        if (distanceToPlayer < viewRadius)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Raycast(npcEyes, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    Debug.DrawRay(npcEyes, directionToPlayer * distanceToPlayer, Color.red);
                    PlayerAnxiety pa = player.GetComponent<PlayerAnxiety>();
                    if (pa != null) pa.IncreaseAnxiety(anxietyRate);
                }
                else
                {
                    Debug.DrawRay(npcEyes, directionToPlayer * distanceToPlayer, Color.green);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}