using UnityEngine;
using FMOD.Unity;
using FMOD.Studio;

public class PlayerFootsteps : MonoBehaviour
{
    // The following script is responsible for scanning the terrain, indexing its layers and selecting the appropriate footstep parameters within an FMOD event.
    // By automating the volume parameters of each footstep sounds audio track in FMOD, this script are able to determine what terrain the player is on and play its respective footstep sound.

    private TerrainDetector terrainDetector; // Reserving the space in memory for class Terrain Dector in this script (terrain dector is a seperate script that returns the terrain layer that the player is currently above).
    private Transform playerTransform; // Creating a variable to store their position in the 3D environment.
    private Vector3 offset = new Vector3(0f, 1.33f, 0f); // A fixed offset that will place the terrain detector closer to the ground. 
    private int currentAudioIdx; // Creating a variable that will handle the currentindex number.
    private bool isColliding = false; // Creating a true or false variable that will inform whether or not the user is within a trigger zone (e.g., a house, tavern or between two stone cliffs).

	// As the following is a custom script that controls an FMOD event, we must be able to select an event in the first place.
	[Header("FMOD Event")] // The following header and public string will show in the inspection window in the Unity Editor when the script is applied to an object in the game environment. 
	[EventRef] // Calling FMODunity package, allowing us to reference an FMOD event.
	public string InputFootsteps; // Allows the user to select an FMOD event.
	EventInstance FootstepsEvent; //Creating an FMOD instance named that can be associated with the user chosen FMOD event. 

    [Header("Footstep Speed")] // User control for the rate of playback for the footstep audio. 
    [Range(0f, 1f)] // The user is able to alter the rate of footsteps within the game between 0f (0 seconds) and 1f (1 second).
    public float footstepinterval = 0.5f; // The deafult interval between each footstep
    private float nextupdate; // This variable represents when the next footstep sound should be played in real time.

     void Awake() // Awake is called before the game begins.
    {
        FootstepsEvent = RuntimeManager.CreateInstance(InputFootsteps); // Initialize the user selected FMOD event to the variable "FootstepsEvent".
    }

	void Start() // Start is called before the first frame update
	{
        playerTransform = GameObject.Find("Capsule").transform; //Finds the game object called "Capsule" that is used to represent the body of the player.
        nextupdate = Time.time + footstepinterval; //Set when to play the first footstep. Time.time expresses the current time, where footstep interval applies the time offset between footsteps. 
        terrainDetector = new TerrainDetector(); //Creating a new instance of class TerrainDetector
    }
    
    void Update() // Update is called once per frame. Within the update, the appropriate terrain footstep audio is played.
	{
        if (Input.GetAxis("Vertical") >= 0.01f || Input.GetAxis("Horizontal") >= 0.01f || Input.GetAxis("Vertical") <= -0.01f || Input.GetAxis("Horizontal") <= -0.01f) //If player is pressing keys
        {
            SetTerrain(); // Calling the function "SetTerrain" to recieve the triggerable footstep area zones layer index.
            if (Time.time > nextupdate) //Checking if it is time for the next footstep sound.
            {
                PlayStep(); //Outputs the footstep information. This includes, the current terrain index, and the volume parameter values that are required to only play the audio for the current footstep.
                PlayAudio(); //Plays through FMOD event once (this function is defined further in the code)
                nextupdate = Time.time + footstepinterval; //Set when the next footstep should be played. 
            }
        }
    }

    void SetTerrain() // This function outputs the players current surface if they are not currently on the base terrain layer.
    {
        if (!isColliding) // If the player is not colliding with the triggable footstep area zones, the terrain detector and footsteps will be prioritized.
        {
            Vector3 playerPos = playerTransform.position - offset; // Finding the player position and removing the offset placed earlier to imporve the accuracy of the terrain detector.
            currentAudioIdx = terrainDetector.GetActiveTerrainTextureIdx(playerPos); // Sets the current audio index to whatever the terrain detector outputs as the terrain layer the player is currently on.
        }
    }


    void PlayStep() // The "PlayStep" function is responsible for choosing the correct footstep.
    {
        switch (currentAudioIdx) // A switch function that will select the appropriate case number depending on the current audio index.
        {
            case 0:
                Grass(); // This will call the grass function shown further in the code, where all the volume parameters of each audio track are controlled.
                break; // Preventing the script from running any other cases in this cycle. 
            case 1:
                Grass();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                Sand();
                break;
            case 5:
                Sand();
                break;
            case 6:
                break;
            case 7:
                GrassThick();
                break;
            case 8:
                GrassThick();
                break;
            case 9:
                Dirt();
                break;
            case 10:
                Gravel();
                break;
            case 11:
                Dirt();
                break;
            case 12:
                Dirt();
                break;
            case 13:
                Mud();
                break;
            case 14:
                Gravel();
                break;
            case 15:
                Gravel();
                break;
            case 16:
                Sand();
                break;
            case 17:
                PlankDirt();
                break;
            case 18:
                Wood();
                break;
            case 19:
                Stone();
                break;
            case 20:
                Water();
                break;
            case 21:
                Carpet();
                break;
        }
    }

    void PlayAudio() // A function that will play the trigger the audio in FMOD to be played (no loop, so the tracks are played through once when the function is called). 
    {
        FootstepsEvent.start(); // Starting the FMOD event to play through once. 
    }

    void OnTriggerStay(Collider floor) // This function establishes what the current audio index is.
    {
        setAudioIdx(floor.tag); // Calling "tag" to set the current audio index. 
    }

    void OnTriggerEnter(Collider other) //We are able to detected whether the player has stepped on any of the following surfaces through identifying the surfaces tag. Once triggered, we are also able to identify that the player is no longer walking on the terrain layers.
    {
        if ((other.tag == "Plank" && currentAudioIdx == 17) || (other.tag == "Wood" && currentAudioIdx == 18) || (other.tag == "Stone" && currentAudioIdx == 19) || (other.tag == "Water" && currentAudioIdx == 20) || (other.tag == "Carpet" && currentAudioIdx == 21))
		{
            isColliding = true; // Once they have stepped on a triggerable surface, the bool created earlier is now true.
            setAudioIdx(other.tag); // We are able to identify the current surface by its index (established in the function below - "setAudioIdx").
        }
    }

    void setAudioIdx(string tag) // Since the different trigger surfaces are not in the terrain detectors index, we establish new index numbers for them through the currentAudioIdx variable created at the start of the script. As there are several surfaces, a new name "tag" was created that will be used to inform the script whether the player has remained on that surface for more than one frame.
    {
        if (tag == "Plank")
        {
            currentAudioIdx = 17;
        }
        else if (tag == "Wood")
        {
            currentAudioIdx = 18;
        }
        else if (tag == "Stone")
        {
            currentAudioIdx = 19;
        }
        else if (tag == "Water")
        {
            currentAudioIdx = 20;
        }
        else if (tag == "Carpet")
        {
            currentAudioIdx = 21;
        }
    }


    void OnTriggerExit(Collider other) // Once the player has left the following triggerable surfaces, the bool "iscolliding" is set to false and the player is now walking on the terrain.
    {
        if ((other.tag == "Plank" && currentAudioIdx == 17) || (other.tag == "Wood" && currentAudioIdx == 18) || (other.tag == "Stone" && currentAudioIdx == 19) || (other.tag == "Water" && currentAudioIdx == 20)|| (other.tag == "Carpet" && currentAudioIdx == 21))
        {
            isColliding = false; // Since they are no longer on a triggerable surface, the player is no longer colliding and priority for the current footstep sound is given to class "Terrain Detector". 
        }
    }

    void Dirt() // Creating a function that will automate all volume parameters on all FMOD event audio tracks. 
    {
        FootstepsEvent.setParameterByName("DirtVolume", 1f, false); // Calling the parameter (in this case, "DirtVolume") that automates the volume of the audio track (from 0f
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Grass()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 1f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Mud()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 1f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Sand()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 1f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Gravel()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 1f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void GrassThick()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 1f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void PlankDirt()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 1f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Wood()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 1f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Stone()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 1f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }

    void Water()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 1f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 0f, false);
    }
    void Carpet()
    {
        FootstepsEvent.setParameterByName("DirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassVolume", 0f, false);
        FootstepsEvent.setParameterByName("MudVolume", 0f, false);
        FootstepsEvent.setParameterByName("SandVolume", 0f, false);
        FootstepsEvent.setParameterByName("GravelVolume", 0f, false);
        FootstepsEvent.setParameterByName("GrassThickVolume", 0f, false);
        FootstepsEvent.setParameterByName("PlankDirtVolume", 0f, false);
        FootstepsEvent.setParameterByName("WoodVolume", 0f, false);
        FootstepsEvent.setParameterByName("StoneVolume", 0f, false);
        FootstepsEvent.setParameterByName("WaterVolume", 0f, false);
        FootstepsEvent.setParameterByName("CarpetVolume", 1f, false);
    }

}

