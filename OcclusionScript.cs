using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class OcclusionScript : MonoBehaviour
{
    // The following scripts acts as an event emitter although addititionally casts a ray from the object to the player to see whether there are any objects inbetween.
    // If there are no objects inbetween no occlusion will be applied, but if there are objects, the script changes the occlusion parameter in FMOD. 
    // Scott Game Sounds. (2017). FMOD & Unity | Creating Audio Occlusion. [Video]. Available at: https://www.youtube.com/watch?v=gTMP5Bxd3X0 [Accessed 15 September 2019]. 

    // As the following is a custom script that controls an FMOD event, we must be able to select an event in the first place.
    [Header("FMOD Event")] // The following header and public string will show in the inspection window in the Unity Editor when the script is applied to an object in the game environment. 
	[EventRef] // Calling FMODunity package, allowing us to reference an FMOD event.
	public string SelectAudio; // Allows the user to select an FMOD event.
	EventInstance Audio; // Creating an empty instance of event "Audio", so we can add an FMOD event and change aspects of the event in FMOD from Unity.
	Transform SlLocation; // Creating a transform (can store positional data of an object in all 3 dimensions).

    [Header("Occlusion Options")] // Creating a space in the insepctor window that will allow the user to control the occlusion value and what should be assumed as a surface that will create an occlusion effect.
    [Range(0f, 1f)] // Setting the range for the parameter 0f (0 in FMOD) to 1f (10 in FMOD). 
    public float OcclusionValue = 0.5f; // Set the default occlusion value to 0.5.
    public LayerMask OcclusionLayer = 1; // Setting the Occlusion Layer to default so it assumes game objects and terrain in the 3D environment will occlude the audio.

    void Awake() // Initialize what is inside the function before the game starts.
	{
        SlLocation = GameObject.FindObjectOfType<StudioListener>().transform; //Sets the transform "SlLocation" to be attached to the studio listner script attached to the camera object in Unity.
        Audio = RuntimeManager.CreateInstance(SelectAudio); //Setting the instance "ATMOS" to the FMOD event the user has selected. 
	}

	void Start() // The following commands are run before the first update within the game.
    {
		// The following is pre-established code from FMOD that begins playing the audio.
		// Since we have put it in the "Start" function, the audio will begin playing as soon as the game starts and will keep playing until the game ends or through specific code.
		PLAYBACK_STATE PbState;
        Audio.getPlaybackState(out PbState);

        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        { 
            Audio.start(); //Begin playing the audio from the selected FMOD event.
        }  
    }

    void Update() // The following function checks whether the game object with this script attached has its line cast hitting the player.
    {
        RuntimeManager.AttachInstanceToGameObject(Audio, GetComponent<Transform>(), GetComponent<Rigidbody>()); // Attaching the FMOD event, "Audio" to the positional information of game object with this script attached.
        RaycastHit hit; // Creating a collider that will be recognised by the Raycast when it is hit.
        Physics.Linecast(transform.position, SlLocation.position, out hit, OcclusionLayer); // Casting a line from the game object to current position of the player. 

        if (hit.collider != null) // This statement checks whether the line cast is hitting a collider (is not null). 
        {
            if (hit.collider.gameObject.name == "Capsule") // If the line hits the collider of the game object with the tag "Capsule" (e.g. this is the name game object the player is controlling). 
            {
                NotOccluded(); // Calls this function (seen below) that sets the occlusion to 0f.
            }
            else // If the line does not hit the player, then the FMOD event is occluded with value = OcclusionValue.
            {
                Occluded(); // Calls this function (seen below) that sets the occlusion to 0f.
			}
        }
    }

    void Occluded() // Setting the occlusion values when occluded.
    { 
        Audio.setParameterByName("Occlusion", OcclusionValue, false); // Calls the FMOD event "Audio" and finds the user created parameter "Occlusion", where the value is set to the user defined OcclusionValue. "False" enables seeking that smoothens the change in occlusion value so it is not instant. 
    }

    void NotOccluded() // Setting the occlusion values when not occluded.
	{
        Audio.setParameterByName("Occlusion", 0f, false); // Turns the occlusion parameters value to 0 within FMOD.
    }
}
