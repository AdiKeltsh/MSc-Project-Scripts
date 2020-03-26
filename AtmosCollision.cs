using UnityEngine;

public class AtmosCollision : MonoBehaviour
{
	// The purpose of this script is to indicate whether the camera object in unity has entered the following trigger zones.
	// This script is nessessary as the studio listner is on the camera object and not the capsule/rigid body.

	[Header("Camera")] // Creates a header in the insepctor window that will indicate that a camera gameObject needs to be called.
    public GameObject Camera; // Creates a visible (e.g. public) empty window, where the user can drag over an object to indicate what they want called by the code.
    bool Entered; // A true or false statment called "Entered" that will indicate whether the player has entered the trigger areas written below.


    void OnTriggerEnter(Collider collider) // If the player is inside a collider with tag the following game object tages, then the filter is applied.
    {
        if (collider.gameObject.tag == "House" && this.tag == "Player") // If a component with the tag "player" is inside the house trigger zone, the player entered is true.
        {
            Camera.GetComponent<AtmosphereAudio>().setEntered(); // Calling the camera game object and calling the "AtmosphereAudio" script to indicate the player has entered a trigger zone. "Set entered" is a function that will change the set parameters (e.g., volume and a low-pass filter) of the wind atmosphere FMOD event.
        }
        if (collider.gameObject.tag == "Stone" && this.tag == "Player") // If a component with the tag "player" is inside stone trigger zone, the player entered is true.
        {
            Camera.GetComponent<AtmosphereAudio>().setEntered(); // Same action as previous statement.
        }

        if (collider.gameObject.tag == "Tavern" && this.tag == "Player") // If a component with the tag "player" is inside the tavern trigger, the player entered is true.
        {      
            Camera.GetComponent<AtmosphereAudio>().setEntered(); // Same action as previous statement.
		}
    }

    void OnTriggerExit(Collider collider) // If the player is not inside the colliders with the following game object tags, then no filter is applied.
    {
        if (collider.gameObject.tag == "House") // If the collider does not have a component with tag "player" inside of its trigger zone, then the player is not inside the collider.
        {
            Camera.GetComponent<AtmosphereAudio>().setOutside(); // Calling the frunction "setOutside" from the "AtmosphereAudio" script, where it will change the volume and low-pass filter parameters called from wind atmosphere FMOD event.
        }
        if (collider.gameObject.tag == "Stone") // If the collider does not have a component with tag "player" inside of its trigger zone, then the player is not inside the collider.
		{
            Camera.GetComponent<AtmosphereAudio>().setOutside(); // Same action as previous statment.
        }
        if (collider.gameObject.tag == "Tavern") // If the collider does not have a component with tag "player" inside of its trigger zone, then the player is not inside the collider.
		{
            Camera.GetComponent<AtmosphereAudio>().setOutside(); // Same action as previous statment.
		}
    }
}
