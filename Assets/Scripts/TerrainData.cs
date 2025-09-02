using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Procedural Terrain/Terrain Data", order = 1)]
public class TerrainData : ScriptableObject
{
    [Header("Terrain Dimensions")]
    [Tooltip("Width of the terrain map in points.")]
    public int terrainWidth = 256;
    [Tooltip("Height of the terrain map in points.")]
    public int terrainHeight = 256;
    [Tooltip("Maximum height of the terrain in Unity units.")]
    public float terrainDepth = 20;

    [Header("Noise Settings")]
    [Tooltip("Seed for the random number generator. Same seed will always produce the same terrain.")]
    public int seed;
    [Tooltip("Controls the zoom level of the noise. Higher values zoom out, lower values zoom in.")]
    public float noiseScale = 25f;
    [Tooltip("Number of layers of noise to combine. More octaves add more detail.")]
    public int octaves = 4;
    [Tooltip("Controls how much smaller details affect the overall shape. (0-1)")]
    [Range(0,1)]
    public float persistence = 0.5f;
    [Tooltip("Controls how much detail is added with each octave. Higher values mean more detail.")]
    public float lacunarity = 2f;
    [Tooltip("Global offset for the noise map.")]
    public Vector2 offset;

    [Header("Erosion Settings")]
    [Tooltip("Enable or disable hydraulic erosion simulation.")]
    public bool performErosion = true;
    [Tooltip("Number of droplets to simulate.")]
    public int erosionIterations = 50000;
    [Tooltip("The radius of the erosion brush.")]
    [Range(2, 8)]
    public int erosionRadius = 3;
    [Tooltip("How much a droplet's direction is influenced by the terrain slope.")]
    [Range(0, 1)]
    public float inertia = .05f;
    [Tooltip("Multiplier for how much sediment a droplet can carry.")]
    public float sedimentCapacityFactor = 4;
    [Tooltip("Minimum sediment capacity.")]
    public float minSedimentCapacity = .01f;
    [Tooltip("How quickly the terrain is eroded.")]
    [Range(0, 1)]
    public float erodeSpeed = .3f;
    [Tooltip("How quickly sediment is deposited.")]
    [Range(0, 1)]
    public float depositSpeed = .3f;
    [Tooltip("How quickly water evaporates.")]
    [Range(0, 1)]
    public float evaporateSpeed = .01f;
    [Tooltip("The effect of gravity on the droplet.")]
    public float gravity = 4;
    [Tooltip("The maximum number of steps a droplet can take.")]
    public int maxDropletLifetime = 30;
    [Tooltip("The initial amount of water in a droplet.")]
    public float initialWaterVolume = 1;
    [Tooltip("The initial speed of a droplet.")]
    public float initialSpeed = 1;

    [Header("Global Shaping")]
    [Tooltip("A curve to remap the terrain height after generation. Allows for creating plateaus, etc.")]
    public AnimationCurve shapingCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Domain Warping")]
    [Tooltip("Enable or disable domain warping for more natural features.")]
    public bool useDomainWarping = true;
    [Tooltip("Strength of the coordinate distortion.")]
    public float domainWarpStrength = 10f;
    [Tooltip("Frequency of the noise used for distortion.")]
    public float domainWarpFrequency = 1f;

    [Header("Water")]
    [Tooltip("Global water level. Values below this will be considered water.")]
    [Range(0, 1)]
    public float waterLevel = 0.3f;
}
