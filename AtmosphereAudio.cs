using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AtmosphereAudio : MonoBehaviour
{
    // The purpose of this script is to call on the atmosphere audio (e.g. wind) FMOD eventl, start the audio and change the occlusion values.
    // The occlusion values change from this script when the player is inside the pre-defined areas (find in script "AtmosCollision"). 

    // As the following is a custom script that controls an FMOD event, we must be able to select an event in the first place.
    [Header("FMOD Event")] // Header indicating purpose of the following 
    [EventRef] // Calling FMODunity package, allowing us to reference an FMOD event.
    public string SelectAudio; // Allows the user to select an FMOD event.
    EventInstance ATMOS; //C reating an empty instance of event "ATMOS", so we can add an FMOD event and change aspects of the event in FMOD from Unity.

    // The following controls will allow for live control of the FMOD event parameters, giving more dyanmic control when mixing.
    [Header("LPF Options")] // Low-pass filter control header.
    [Range(0f, 12f)] // Setting the range from 0 (22kHz) to 12 (10kHz) to correspond with the values in the FMOD events audio inserts. An inital value of 10 is where, where "f" indicates the value as a float.
    public float LPFValue = 10f; // Initializing a variable "LPFValue" as a float so it can be changed by the user and code. 
    [Header("Volume Options")] // Volume control header.
    [Range(0f, 1f)] // Setting the range from 0 (0dB) to 1 (minus infinity). 
    public float VolumeValue = 0.5f; // Initializing a variable "Volume" as a float so it can be changed by the user and code. An inital value of 0.5 is set.

	void Awake()// Initialize what is inside the function before the game starts.
    {
        ATMOS = RuntimeManager.CreateInstance(SelectAudio); // Setting the instance "ATMOS" to the FMOD event the user has selected. 
    }

    
    void Start() // This function is called before the first frame update.
    {
		// The following is pre-established code from FMOD that begins playing the audio.
		// Since we have put it in the "Start" function, the audio will begin playing as soon as the game starts and will keep playing until the game ends or through specific code.
        PLAYBACK_STATE PbState; 
        ATMOS.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            ATMOS.start(); // Starts the audio within the selected FMOD event.
        }
    }

    public void setEntered()// Here we esablish an action for when "setEntered()" is called. This function is called in the "AtmosCollision" script.
    {
		// If the player has entered the zones set in "AtmosCollision", we automate the corresponding parameters found in the FMOD event".
		// The paramter "LPF" and "Volume" are set as strings so that we are able to represent text, where in this case the text has to correspond to the text in FMOD.
		ATMOS.setParameterByName("LPF", LPFValue, false); // The following lines allows us to (in order), call on a parameter, set the value to the value selected for the parameter in Unity and/or this code and enable seek speed, which allows the paramters to change gradually rather than instantaniously. 
        ATMOS.setParameterByName("Volume", VolumeValue, false);
    }
    public void setOutside()
    {
        ATMOS.setParameterByName("LPF", 0f, false); // Similar to the lines above, but instead we set both parameter values to 0f so that no effect is applied.
        ATMOS.setParameterByName("Volume", 0f, false);
    }

}
