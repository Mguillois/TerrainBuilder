using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(
        int mapWidth, int mapHeight, int seed, float scale,
        int octaves, float persistence, float lacunarity, Vector2 offset,
        bool useDomainWarping, float domainWarpStrength, float domainWarpFrequency)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Offsets for domain warping noise to ensure it's different from the main noise
        Vector2 domainWarpOffset1 = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
        Vector2 domainWarpOffset2 = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x - mapWidth / 2f);
                float sampleY = (y - mapHeight / 2f);

                if (useDomainWarping)
                {
                    float warpX_noise = Mathf.PerlinNoise((sampleX / scale + domainWarpOffset1.x) * domainWarpFrequency, (sampleY / scale + domainWarpOffset1.y) * domainWarpFrequency) * 2 - 1;
                    float warpY_noise = Mathf.PerlinNoise((sampleX / scale + domainWarpOffset2.x) * domainWarpFrequency, (sampleY / scale + domainWarpOffset2.y) * domainWarpFrequency) * 2 - 1;
                    sampleX += warpX_noise * domainWarpStrength;
                    sampleY += warpY_noise * domainWarpStrength;
                }

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float finalSampleX = sampleX / scale * frequency + octaveOffsets[i].x;
                    float finalSampleY = sampleY / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(finalSampleX, finalSampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map to be between 0 and 1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
