using UnityEngine;
using UnityEngine.UI;

public class WaypointUI : MonoBehaviour
{
    public FollowPath tankScript; // De tank die moet rijden
    public WPManager wpManager;   // De manager met de lijst van waypoints

    // Deze functie roepen we aan vanuit de Dropdown
    public void HandleDropdownChange(int index)
    {
        // Controleer of de index geldig is in onze waypoint-lijst
        if (index >= 0 && index < wpManager.waypoints.Length)
        {
            GameObject targetPalm = wpManager.waypoints[index];
            tankScript.GoToWaypoint(targetPalm);
            Debug.Log("Tank gestuurd naar: " + targetPalm.name);
        }
    }
}
