using UnityEngine;

public static class ErosionSimulator
{
    // Static fields to cache the erosion brush data
    private static int[][] erosionBrushIndices;
    private static float[][] erosionBrushWeights;
    private static int currentBrushSize;
    private static int currentMapSize;

    public static void Erode(float[,] map, int mapSize, TerrainData data)
    {
        Initialize(mapSize, data);
        System.Random prng = new System.Random(data.seed);

        for (int i = 0; i < data.erosionIterations; i++)
        {
            float posX = prng.Next(0, mapSize - 1);
            float posY = prng.Next(0, mapSize - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = data.initialSpeed;
            float water = data.initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < data.maxDropletLifetime; lifetime++)
            {
                int nodeX = (int)posX;
                int nodeY = (int)posY;

                if (nodeX < 0 || nodeX >= mapSize - 1 || nodeY < 0 || nodeY >= mapSize - 1)
                {
                    break;
                }

                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, mapSize, posX, posY);

                dirX = (dirX * data.inertia - heightAndGradient.gradientX * (1 - data.inertia));
                dirY = (dirY * data.inertia - heightAndGradient.gradientY * (1 - data.inertia));

                float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }
                posX += dirX;
                posY += dirY;

                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1)
                {
                    break;
                }

                float newHeight = CalculateHeightAndGradient(map, mapSize, posX, posY).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * data.sedimentCapacityFactor, data.minSedimentCapacity);

                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * data.depositSpeed;
                    sediment -= amountToDeposit;

                    map[nodeX, nodeY] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                    map[nodeX + 1, nodeY] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                    map[nodeX, nodeY + 1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                    map[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * cellOffsetY;
                }
                else
                {
                    float amountToErode = Mathf.Min((sedimentCapacity - sediment) * data.erodeSpeed, -deltaHeight);

                    int brushIndex = nodeY * mapSize + nodeX;
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[brushIndex].Length; brushPointIndex++)
                    {
                        int nodeIndex = erosionBrushIndices[brushIndex][brushPointIndex];
                        int brushNodeX = nodeIndex % mapSize;
                        int brushNodeY = nodeIndex / mapSize;

                        float weighedErodeAmount = amountToErode * erosionBrushWeights[brushIndex][brushPointIndex];
                        float deltaSediment = (map[brushNodeX, brushNodeY] < weighedErodeAmount) ? map[brushNodeX, brushNodeY] : weighedErodeAmount;

                        map[brushNodeX, brushNodeY] -= deltaSediment;
                        sediment += deltaSediment;
                    }
                }

                speed = Mathf.Sqrt(Mathf.Max(0, speed * speed + deltaHeight * data.gravity));
                water *= (1 - data.evaporateSpeed);
            }
        }
    }

    private static void Initialize(int mapSize, TerrainData data)
    {
        if (erosionBrushIndices == null || currentBrushSize != data.erosionRadius || currentMapSize != mapSize)
        {
            InitializeBrushIndices(mapSize, data.erosionRadius);
            currentBrushSize = data.erosionRadius;
            currentMapSize = mapSize;
        }
    }

    private static void InitializeBrushIndices(int mapSize, int radius)
    {
        erosionBrushIndices = new int[mapSize * mapSize][];
        erosionBrushWeights = new float[mapSize * mapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
        {
            int centreX = i % mapSize;
            int centreY = i / mapSize;

            weightSum = 0;
            addIndex = 0;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    float sqrDst = x * x + y * y;
                    if (sqrDst < radius * radius)
                    {
                        int coordX = centreX + x;
                        int coordY = centreY + y;

                        if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                        {
                            float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                            weightSum += weight;
                            weights[addIndex] = weight;
                            xOffsets[addIndex] = x;
                            yOffsets[addIndex] = y;
                            addIndex++;
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new int[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++)
            {
                erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
    }

    private static HeightAndGradient CalculateHeightAndGradient(float[,] nodes, int mapSize, float posX, float posY)
    {
        int coordX = (int)posX;
        int coordY = (int)posY;

        float x = posX - coordX;
        float y = posY - coordY;

        int nodeX = (int)posX;
        int nodeY = (int)posY;

        if (nodeX < 0 || nodeX >= mapSize - 1 || nodeY < 0 || nodeY >= mapSize - 1)
        {
            return new HeightAndGradient() { height = 0, gradientX = 0, gradientY = 0 };
        }

        float heightNW = nodes[nodeX, nodeY];
        float heightNE = nodes[nodeX + 1, nodeY];
        float heightSW = nodes[nodeX, nodeY + 1];
        float heightSE = nodes[nodeX + 1, nodeY + 1];

        float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    private struct HeightAndGradient
    {
        public float height;
        public float gradientX;
        public float gradientY;
    }
}
