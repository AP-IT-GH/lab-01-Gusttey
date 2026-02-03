using UnityEngine;

public class FollowWaypoint : MonoBehaviour
{
    // De lijst met waypoints (bijv. de palmbomen)
    public GameObject[] waypoints;

    // Index van het huidige waypoint waar we naartoe rijden
    private int currentWaypoint = 0;

    public float speed = 5.0f;
    public float rotSpeed = 2.0f;

    public float accuracy = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Als er geen waypoints zijn, doe niets
        if (waypoints.Length == 0) return;

        // 1. Bereken de richting naar het huidige waypoint
        Vector3 direction = waypoints[currentWaypoint].transform.position - transform.position;

        // 2. Rotatie: Draai de tank geleidelijk richting het waypoint
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);

        // 3. Check afstand: Zijn we dicht genoeg bij het waypoint?
        if (direction.magnitude < accuracy)
        {
            currentWaypoint++;

            // Als we bij het laatste waypoint zijn, ga terug naar de eerste (looping)
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }

        // 4. Beweging: Verplaats de tank over de lokale Z-as
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
