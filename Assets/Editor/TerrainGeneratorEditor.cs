using UnityEngine;
using UnityEditor;

public class TerrainGeneratorEditor : EditorWindow
{
    private TerrainData terrainData;
    private Terrain targetTerrain;
    private Texture2D previewTexture;

    [MenuItem("Window/Procedural Terrain Generator")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGeneratorEditor>("Terrain Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Procedural Terrain Generator", EditorStyles.boldLabel);

        terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", terrainData, typeof(TerrainData), false);
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", targetTerrain, typeof(Terrain), true);

        if (GUILayout.Button("Generate Preview"))
        {
            if (terrainData != null)
            {
                GeneratePreview();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Terrain Data asset.", "OK");
            }
        }

        if (GUILayout.Button("Apply to Terrain"))
        {
            if (terrainData != null && targetTerrain != null)
            {
                ApplyToTerrain();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign both a Terrain Data asset and a Target Terrain.", "OK");
            }
        }

        if (previewTexture != null)
        {
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetAspectRect((float)previewTexture.width / previewTexture.height);
            EditorGUI.DrawTextureTransparent(previewRect, previewTexture, ScaleMode.ScaleToFit);
        }
    }

    private float[,] GenerateHeightMap()
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(
            terrainData.terrainWidth,
            terrainData.terrainHeight,
            terrainData.seed,
            terrainData.noiseScale,
            terrainData.octaves,
            terrainData.persistence,
            terrainData.lacunarity,
            terrainData.offset,
            terrainData.useDomainWarping,
            terrainData.domainWarpStrength,
            terrainData.domainWarpFrequency
        );

        if (terrainData.performErosion)
        {
            ErosionSimulator.Erode(noiseMap, terrainData.terrainWidth, terrainData.terrainHeight, terrainData);
        }

        // Apply shaping curve
        for (int y = 0; y < terrainData.terrainHeight; y++)
        {
            for (int x = 0; x < terrainData.terrainWidth; x++)
            {
                noiseMap[x, y] = terrainData.shapingCurve.Evaluate(noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private void GeneratePreview()
    {
        float[,] heightMap = GenerateHeightMap();

        if (previewTexture == null || previewTexture.width != terrainData.terrainWidth || previewTexture.height != terrainData.terrainHeight)
        {
            previewTexture = new Texture2D(terrainData.terrainWidth, terrainData.terrainHeight);
        }

        Color[] colorMap = new Color[terrainData.terrainWidth * terrainData.terrainHeight];
        for (int y = 0; y < terrainData.terrainHeight; y++)
        {
            for (int x = 0; x < terrainData.terrainWidth; x++)
            {
                float height = heightMap[x, y];
                if (height < terrainData.waterLevel)
                {
                    colorMap[y * terrainData.terrainWidth + x] = new Color(0.2f, 0.3f, 0.8f, 1f);
                }
                else
                {
                    colorMap[y * terrainData.terrainWidth + x] = new Color(height, height, height);
                }
            }
        }

        previewTexture.SetPixels(colorMap);
        previewTexture.Apply();
    }

    private void ApplyToTerrain()
    {
        float[,] heightMap = GenerateHeightMap();

        UnityEngine.TerrainData unityTerrainData = targetTerrain.terrainData;
        int resolution = terrainData.terrainWidth;
        // Unity terrain heightmap resolution must be a power of 2 plus 1 (e.g., 257x257)
        if (unityTerrainData.heightmapResolution != resolution + 1)
        {
            unityTerrainData.heightmapResolution = resolution + 1;
        }

        unityTerrainData.size = new Vector3(terrainData.terrainWidth, terrainData.terrainDepth, terrainData.terrainHeight);

        // We need to create a new heightmap with resolution + 1 for Unity's terrain data,
        // and transpose it from [x, y] to [y, x].
        // Let's assume terrainWidth and terrainHeight are the same (resolution).
        float[,] finalHeightMap = new float[resolution + 1, resolution + 1];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                finalHeightMap[y, x] = heightMap[x, y];
            }
        }

        // Duplicate the last row and column to fill the +1 dimension
        for (int x = 0; x < resolution; x++)
        {
            finalHeightMap[resolution, x] = finalHeightMap[resolution - 1, x];
        }
        for (int y = 0; y < resolution; y++)
        {
            finalHeightMap[y, resolution] = finalHeightMap[y, resolution - 1];
        }
        // Set the corner point
        finalHeightMap[resolution, resolution] = finalHeightMap[resolution - 1, resolution - 1];

        unityTerrainData.SetHeights(0, 0, finalHeightMap);
    }
}
