using UnityEngine;

public class TerrainDetector
{
    // The purpose of this function is to process the terrain layer within Unity and return the texture layer(s) used to create the virtual world.
    // The script requires the alpha and splat map, where it determines the structure and elements used to construct the terrain.
    // Furthermore, the script maps out the terrain with a coordinate system, that will allow it to output the current terrain layer that the character is standing on.
    // This script is called from "PlayerFootsteps.cs", where the class "TerrainDetector" returns the players current terrain layer index.

    // Unity3DCollege (2017). TerrainDetector Source Code (Version 1.0) [Source Code]. Last updated: 18 October 2017. 
    // Available at: https://gist.github.com/unity3dcollege/f4e7b3fdb95210561580a0d14c4c4f8a [Accessed 15 September 2019].

    private TerrainData terrainData; // Reserving the place in memory for the 
    private int alphamapWidth; // A numberical variable that will express the width of the entire terrain.
    private int alphamapHeight; // A numberical variable that holds the various elevation values of the terrain. 
    private float[,,] splatmapData; // A splat map is used to blend the different terrain textures.
    private int numTextures; // A numerical variable that will hold the number of terrains in the environment. 

    // This method constructs the Terrain Detector object that can then be used to detect the most common texture in a given position in the virtual environment. 
    public TerrainDetector() 
    {
        terrainData = Terrain.activeTerrain.terrainData; 
        alphamapWidth = terrainData.alphamapWidth; 
        alphamapHeight = terrainData.alphamapHeight;
        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

	// This method converts the the Splat map into its coordinates, so that the different textures can be identified and indexed by the method below (GetActiveTerrainTextureIdx).
	private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Terrain ter = Terrain.activeTerrain; 
        Vector3 terPosition = ter.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return splatPosition;
    }

    // This method takes a vector and returns the index of the common terrain texture that is most prominent in a given position in the virtual environment.
    public int GetActiveTerrainTextureIdx(Vector3 position)
    {
        Vector3 terrainCord = ConvertToSplatMapCoordinate(position);
        int activeTerrainIndex = 0; 
        float largestOpacity = 0f;

        for (int i = 0; i < numTextures; i++) // Creating a loop that will continue to read and index the different terrain texure layers.
        {
            if (largestOpacity < splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
            {
                activeTerrainIndex = i; // The terrain index is equal to the current loop cycle.
                largestOpacity = splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
            }
        }

        return activeTerrainIndex; // Output the current terrain index. This return is used to call the correct footstep sound within "PlayerFootsteps.cs". 
    }
}