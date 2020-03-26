using UnityEngine;

public class HouseOcclusion : MonoBehaviour
{
    // The following script calls in game objects and applies the following code to them. In this scenario, if the waypoint is inside of a house with tag "House" the occlusion value changes through to a pre-set number in the script "WaypointAudio".
    // Allowing the user to select waypoint game objects that will have the following script applied to them.

    [Header("waypoint")]
    public GameObject waypoint; // Creating a empty game object window, where the user can drag and drop the desired waypoint. 
    public GameObject waypoint2; // Same as previous.

    void OnTriggerEnter(Collider player) // This function starts when the player enters a collider that is has it's properties set to "is trigger". An empty collider with name player is defined so that we can attached the tag "player" to it for this function.
    {
        if (player.tag == "Player" && this.tag == "House") // If the collider with tag "House" has the the players collider inside its trigger zone, the following occurs. 
        {
            waypoint.GetComponent<WaypointAudio>().setocclusion(true); // Calling the "WaypointAudio" script and setting the function "setocclusion" function as true so that the occlusion value of the game object defined in this script can be changed. 
            waypoint2.GetComponent<WaypointAudio>().setocclusion(true);
        }
    }

    private void OnTriggerExit(Collider player) // If the player exits the collider the following occurs. The same occurs as the when the player enters expect the "setocclusion" function is set to as false.  
    {
        if (player.tag == "Player" && this.tag == "House") 
        {
            waypoint.GetComponent<WaypointAudio>().setocclusion(false);
            waypoint2.GetComponent<WaypointAudio>().setocclusion(false);
        }
    }
}



