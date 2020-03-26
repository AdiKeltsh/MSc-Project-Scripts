using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class WaypointAudio : MonoBehaviour
{
    // The purpose of this script is to allow the user to define text they want the player to see if they get within the game objects trigger zone, play the waypoint audio and disable it once the waypoint is toggled, ...
    // creating occlusion through raycasting and parameter automation and enabling the next waypoint once the current one is disabled. 

    public GameObject text; // Creating an empty window in the inspector window where the user can drag and drop a fixed text message.
    public GameObject text2;
    public GameObject[] WaypointSounds; // Creating an empty array where the waypoint objects can defined. 
    bool playerEntered = false; // Creating true or false variable. The bool is set to as initially false (will be explained later in the script).

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
		SlLocation = GameObject.FindObjectOfType<StudioListener>().transform; // Setting the variable SlLocation equal to the poisitoin of the the studio listner that is attached to the camera in the virtual environment.
        Audio = RuntimeManager.CreateInstance(SelectAudio); // Attaching the user selected FMOD event to the instance "Audio", allowing the script to change the parameter values of the FMOD event.
    }

    void Start()// Start is called before the first frame update
	{
		// The following is pre-established code from FMOD that begins playing the audio.
		// Since we have put it in the "Start" function, the audio will begin playing as soon as the game starts and will keep playing until the game ends or through specific code.
		PLAYBACK_STATE PbState;
        Audio.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Audio.start(); // Start playing the user selected FMOD event.
        }
    }

    public void setocclusion(bool Enter) // Creating a public function with a bool called "Enter" as it needs to be controlled from the script "HouseOcclusion". 
    {
        if (Enter) // If the player is inside a house then changed the occlusion value to 0.05f instead of the normal occlusion value.
        {
            OcclusionValue = 0.05f;
        }
        else // Or else, use the default occlusion value. 
        {
            OcclusionValue = 0.5f;
        }
    }
    
    void Update() // Update is called once per frame. The function checks if the player is able to toggle a waypoint, and if they do to trigger the next waypoint. In addition, the function casts a line from the waypoint to the player to see whether it is occluded and applies the correct automatation in FMOD.  
	{
        if (Input.GetKeyDown(KeyCode.E) && playerEntered == true) // Checking if the player is in the waypoints trigger zone and is pressing the "E" key.
        {
            this.enabled = false; // Disable this script.
            text.SetActive(false); // Disable the text attached to the waypoint so it does not show anymore.
            nextsphere(); // Calls on the function "nextsphere" that disables the current game object and actives the next. 
            Audio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // Stop playing the FMOD event that is attached to the waypoint game object.
        }

        RuntimeManager.AttachInstanceToGameObject(Audio, GetComponent<Transform>(), GetComponent<Rigidbody>()); // Setting the instance "ATMOS" to the FMOD event the user has selected. 

		RaycastHit hit; // Creating a collider that will be recognised by the Raycast when it is hit.
		Physics.Linecast(transform.position, SlLocation.position, out hit, OcclusionLayer); // Attaching the FMOD event, "Audio" to the positional information of game object with this script attached.

		if (hit.collider != null) // Checking that the linecast is hitting a collider, and if so to continue with the script. 
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

    public void nextsphere() // A function that disables the current waypoint game object and activates the next in the WaypointSounds array, created at the start of the script.  
    {
        WaypointSounds[0].SetActive(false);
        WaypointSounds[1].SetActive(true);
    }

    void OnTriggerEnter(Collider player) // This function starts when the player enters a collider that is has it's properties set to "is trigger". An empty collider with name player is defined so that we can attached the tag "player" to it for this function.
	{
        if (player.tag == "Player" && this.tag == "Untagged") // If the collider (in this case the waypoint) with tag "Untagged" has the the players collider inside its trigger zone, the following occurs. 
		{
            text.SetActive(true); // Displays the pre-defined text (tells the player to press "E"). 
            playerEntered = true; // Part of the condition of the if statement in update that will allow the user to get to the next sphere. 
        } 
        else // This function is built so that this else statement will activate when it reaches
        {
            text2.SetActive(true); // Displays the pre-defined text two (tells the player the listening test has finished). 
		}
    }

    void OnTriggerExit(Collider player) // If the player exits the colliders trigger zone the following occurs. 
	{
        if (player.tag == "Player") // If the player is not inside the trigger zone of the waypoint, then the text is not active and the bool, "playerEntered" is set to as false. 
		{
            text.SetActive(false); // Deactive the text. 
            playerEntered = false; // Setting the bool to false as the player is no longer in the waypoints trigger zone.
        }
    }

    void Occluded() // A function that changes the Occlusion parameter of the waypoints in FMOD to the value of "Occlusion value". Seeking is also enabled so that the change in occlusion values is not instant. 
    {
        Audio.setParameterByName("Occlusion", OcclusionValue, false);
    }

    void NotOccluded() // A function that sets the Occlusion paramter to 0 (not active). 
    {
        Audio.setParameterByName("Occlusion", 0f, false);
    }
}
