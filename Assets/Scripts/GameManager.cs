using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // We gebruiken hitInfo.point direct zodat de hoogte altijd klopt
                agent.SetDestination(hitInfo.point);

                // Dit helpt je om te zien of het klikken werkt in de Console (onderaan je scherm)
                Debug.Log("Doel gezet op: " + hitInfo.point);
            }
        }
    }
}
