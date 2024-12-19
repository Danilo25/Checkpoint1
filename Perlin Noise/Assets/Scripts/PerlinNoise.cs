using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PerlinNoise
{
    public static float[,] GenerateHeightMap(
    int mapWidth,
    int mapHeight,
    int seed,
    float scale,
    int octaves,
    float persistence,
    float lacunarity,
    Vector2 globalOffset
)
{
    float[,] noiseMap = new float[mapWidth, mapHeight];

    System.Random prng = new System.Random(seed);
    Vector2[] octaveOffsets = new Vector2[octaves];

    for (int i = 0; i < octaves; i++)
    {
        float offsetX = prng.Next(-100000, 100000) + globalOffset.x;
        float offsetY = prng.Next(-100000, 100000) + globalOffset.y;
        octaveOffsets[i] = new Vector2(offsetX, offsetY);
        Debug.Log("Octavar: " + i +" | "+ octaveOffsets[i].x + " | " + octaveOffsets[i].y);
    }

    if (scale <= 0)
    {
        scale = 0.0001f;
    }

    float maxNoiseHeight = float.MinValue;
    float minNoiseHeight = float.MaxValue;

    float halfWidth = mapWidth / 2f;
    float halfHeight = mapHeight / 2f;

    for (int x = 0; x < mapWidth; x++)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
                // Aplique o deslocamento global e o deslocamento da oitava
                float sampleX = ((x + globalOffset.x) / scale * frequency + octaveOffsets[i].x);
                float sampleY = ((y + globalOffset.y) / scale * frequency + octaveOffsets[i].y);

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            if (noiseHeight > maxNoiseHeight)
            {
                maxNoiseHeight = noiseHeight;
            }
            else if (noiseHeight < minNoiseHeight)
            {
                minNoiseHeight = noiseHeight;
            }

            noiseMap[x, y] = noiseHeight;
        }
    }

    for (int x = 0; x < mapWidth; x++)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        }
    }

    return noiseMap;
}
}
