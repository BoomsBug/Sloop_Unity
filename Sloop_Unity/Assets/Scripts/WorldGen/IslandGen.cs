//using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;

//using Unity.Mathematics;
using UnityEngine;

/*
    TODO:
    - ?Add textures?
        - animated water texture can easily be added by just having it underneath the generated world (since water has .5 opacity)

    - ?Add zero to three small brown blobs to be buildings?
        - Or just add it after proc gen

*/

public class IslandGen : MonoBehaviour
{
    [Header("Procedural Generation Settings")]
    public int zoom;
    //public int seed; // basically (seed, seed) offset
    public int octaves;
    public float baseFrequency;
    public float baseAmplitude;
    public float persistance;
    public float lacunarity;

    [Header("Terrain Settings")]
    public float deepWaterLevel;
    public float waterLevel;
    public float shoalLevel;
    public float beachLevel;
    public float forestLevel;
    public float hillLevel;
    public float mountainLevel;
    public float snowLevel;
    public float level;

    [Header("Terrain Colours (temporary)")]
    public Color deepWater;
    public Color water;
    public Color shoal;
    public Color beach;
    public Color forest;
    public Color hill;
    public Color mountain;
    public Color snow;

    public GameObject port;

    private Texture2D noiseTex;
    private Color[] pix;
    private Sprite islandSprite;

    //noIsland for if just an empty patch of water (so it doesn't add a collider)
    public GameObject Generate(int seed, float extraLevel, bool noIsland, int resolution, int islandCounter, Vector2 tile) 
    {
        //rend = GetComponent<SpriteRenderer>();
        Random.InitState(seed);

        bool hasPort = false;

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(resolution, resolution);
        noiseTex.filterMode = FilterMode.Point;
        noiseTex.wrapMode = TextureWrapMode.Clamp;

        pix = new Color[noiseTex.width * noiseTex.height];
        
        islandSprite = Sprite.Create(
            noiseTex,
            new Rect(0, 0, resolution, resolution),
            new Vector2(0.5f, 0.5f),
            resolution
        );
        //rend.sprite = islandSprite;


        float xCenter = noiseTex.width / 2;
        float yCenter = noiseTex.height / 2;
        //add a random offset 
        xCenter += Random.Range(-noiseTex.width / 4, noiseTex.width / 4);
        yCenter += Random.Range(-noiseTex.height / 4, noiseTex.height / 4);

        // For each pixel in the texture...
        for (float y = 0.0f; y < noiseTex.height; y++)
        {
            for (float x = 0.0f; x < noiseTex.width; x++)
            {
                //Calculate falloff map
                float deltaX = x - xCenter;
                float deltaY = y - yCenter;
                float distance = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY) / Mathf.Max(xCenter, yCenter);
                float falloff = Mathf.Clamp01(1f - distance);

                // Add level of detail (LOD)
                float frequency = baseFrequency;
                float amplitude = baseAmplitude;
                float maxValue = 0f;
                float noiseHeight = 0f;

                // for each octave, take a sample
                for (int oct = 0; oct < octaves; oct++)
                {
                    float xCoord = (x / (float)noiseTex.width * zoom * frequency) + seed;
                    float yCoord = (y / (float)noiseTex.height * zoom * frequency) + seed;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    noiseHeight += sample * amplitude;
                    maxValue += amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseHeight /= maxValue;

                //apply falloff
                noiseHeight = (noiseHeight * falloff) + 0.2f + extraLevel;

                // Paint pixel
                Color pixColor = new Color(
                    CalculateColor(noiseHeight)[0], 
                    CalculateColor(noiseHeight)[1], 
                    CalculateColor(noiseHeight)[2], 
                    (noiseHeight - level < beachLevel) ? 0.5f : 1.0f // if water, make semi transparent so unity can generate a collider
                );
                pix[(int)y * noiseTex.width + (int)x] = pixColor;

                //if island is not small and sample is forest, place a port there (temporary?)
                // place at x / noiseTex.width, y / noiseTex.height?
                if (!hasPort && extraLevel >= 0 && noiseHeight + level > forestLevel)
                {
                    // x,y / resolution SHOULD be right?? ahhhhhhhhhhg! the formula makes sense and should work!! fml
                    // will have to do for now, they are all in the right general area but definitely not exact
                    Instantiate(port, (new Vector2(x, y) / resolution) + tile - (0.5f* new Vector2(xCenter/resolution, yCenter/resolution)), Quaternion.identity);
                    //Debug.Log($"Res: {resolution}, x: {x}, y: {y}, placed at: {(new Vector2(x, y) / resolution) + tile}");
                    hasPort = true;
                }
            }
        }
        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
        //rend.material.mainTexture = noiseTex;

        return SaveIsland(noIsland, resolution, islandCounter, new Vector2(xCenter, yCenter), tile, extraLevel);
    }

    private Color CalculateColor(float sample)
    {
        sample -= level;
        //return new Color(sample,sample,sample);
        if (sample < waterLevel) {return deepWater;}
        if (sample < shoalLevel) {return water;}
        if (sample < beachLevel) {return shoal;}
        if (sample < forestLevel) {return beach;}
        if (sample < hillLevel) {return forest;}
        if (sample < mountainLevel) {return hill;}
        if (sample < snowLevel) {return mountain;}
        return snow;
    }

    private GameObject SaveIsland(bool noIsland, int resolution, int islandCounter, Vector2 center, Vector2 tile, float extraLevel)
    {
        GameObject islandObject = new GameObject($"Island {islandCounter}");
        //give sprite
        islandObject.AddComponent<SpriteRenderer>();
        islandObject.GetComponent<SpriteRenderer>().sprite = islandSprite;

        //Give island a script and data
        Island islandScript = islandObject.AddComponent<Island>();
        islandScript.islandID = islandCounter;
        islandScript.tileCoordinates = tile;
        islandScript.isIsland = !noIsland;

        islandScript.islandCenter = (center / resolution) + tile; //how tf do i convert the center (pixels) into world space??

        //generate and give collider
        if (!noIsland)
            islandObject.AddComponent<PolygonCollider2D>();

        return islandObject;
    }
}
