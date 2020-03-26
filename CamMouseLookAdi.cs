using UnityEngine;

public class CamMouseLookAdi : MonoBehaviour
{
    // The following script enables the user to have limited directional freedom when controlling the camera inside Unity.
    // Holistic3d. (2016). How to construct a simple First Person Controller with Camera Mouse Look in Unity 5. [Video]. 
    // Available at: https://www.youtube.com/watch?v=blO039OzUZc [Accessed 15 September 2019].

    Vector2 mouseLook; // This variable will allow us to keep track of how much movement the camera has made.
    Vector2 smoothV; // This variable will aid in keeping the translation of mouse to camera movement smooth.
    public float sensitivity = 5.0f; // Initialising a deafult for how fast the players mouse should affect the characters view.
    public float smoothing = 2.0f; // The deafult smoothing value.
    GameObject character; // Resevering a variable for the physical body of the character. 

    void Start()  // Start is called before the first frame update.
	{
        character = this.transform.parent.gameObject; // Attaching the characters physical body position to the variable "character".
    }

    void Update() // Update is called once per frame
	{
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); // Keeping track of the current change in the mouses movement since the last update.
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing); // Stabilising movements across the x axis.
		smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing); // Stabilising movements across the y axis.
		mouseLook += smoothV; // Attaching the total movement of the Camera to the smoothing value.
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f); // Locking the camera view to + and - 90 degrees on the vertical axis so that the camera does not rotate a full 360 degrees. 
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right); // Inverting the mouse movements on the y axis
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up); // Making sure that the player is moving in the direction they are looking within the game.
    }
}
