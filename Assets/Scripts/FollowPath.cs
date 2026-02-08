using UnityEngine;

public class FollowPath : MonoBehaviour
{

    public WPManager wpManager; // Sleep de WPManager hierin
    public GameObject currentNode; // Waar de tank nu is (startpunt)
    public float speed = 5.0f;
    public float rotSpeed = 2.0f;

    private int currentPathIndex = 0;

    // Methode voor het UI-dropdown event (Taak 1.1 in PDF)
    public void GoToWaypoint(GameObject targetNode)
    {
        // Voer A* uit op de graph in WPManager
        bool pathFound = wpManager.graph.AStar(currentNode, targetNode);

        if (pathFound)
        {
            currentPathIndex = 0; // Reset de voortgang
            Debug.Log("Pad gevonden naar: " + targetNode.name);
        }
    }

    void Update()
    {
        // 1. Haal de route op
        var path = wpManager.graph.pathList;

        // Stop als er geen pad is of als we aan het einde zijn
        if (path == null || path.Count == 0 || currentPathIndex >= path.Count) return;

        // 2. Waar moeten we nu heen?
        GameObject targetNode = path[currentPathIndex].getID();
        Vector3 targetPos = targetNode.transform.position;

        // BELANGRIJK: Zet de hoogte van het doel gelijk aan de tank
        // Hierdoor negeren we hoogteverschillen en kijkt hij alleen naar X en Z
        targetPos.y = this.transform.position.y;

        // 3. Bereken richting en afstand
        Vector3 direction = targetPos - this.transform.position;
        float distance = direction.magnitude;

        // 4. Zijn we er al? (Gebruik een iets ruimere marge, bijv. 1.0f of 2.0f)
        if (distance < 1.0f)
        {
            // Ja! Ga naar de volgende
            currentPathIndex++;
        }
        else
        {
            // Nee, blijf rijden

            // Draai rustig naar het doel
            Quaternion lookRot = Quaternion.LookRotation(direction);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                       lookRot,
                                                       Time.deltaTime * rotSpeed);

            // Rij vooruit
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }
}
