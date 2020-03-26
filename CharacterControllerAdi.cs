using UnityEngine;

public class CharacterControllerAdi : MonoBehaviour
{
    // The purpose of this script is to give the player control of the players body so that they can move around within the game.
    // Holistic3d. (2016). How to construct a simple First Person Controller with Camera Mouse Look in Unity 5. [Video]. 
    // Available at: https://www.youtube.com/watch?v=blO039OzUZc [Accessed 15 September 2019].

    public float speed = 10.0F; // Creates a changeable character speed. 
    
    void Start() // Start is called before the first frame update.
	{
        Cursor.lockState = CursorLockMode.Locked; // Locks the view of the player in one place, so that the player moves in the direction they are looking. 
    }
    
    void Update() // Update is called once per frame, where the w, a, s and d keys are read and applied to the characters physical body in the game. 
	{
        float translation = Input.GetAxis("Vertical") * speed; // Reading whether the player is pressing the correct keys to moving forward or backward. The player speed is applied to this reading to have the player physically move. 
        float straffe = Input.GetAxis("Horizontal") * speed; // Reading whether the player is pressing the correct keys to moving left or right. The player speed is applied to this reading to have the player physically move. 
		translation *= Time.deltaTime; // Making sure the players movements on the vertical axis are smooth by using "delta.Time", which represents the time between the current and preivous update. 
        straffe *= Time.deltaTime; // Updating the players movement state on the hortizontal axis.
		transform.Translate(straffe, 0, translation); // Applying the players choice in movement to the physical body of their character within the game.

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None; // Allows the user to get access to the mouse again (useful when working within the unity editor).
    }
}
