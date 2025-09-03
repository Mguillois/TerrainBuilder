# Procedural Terrain Generator for Unity

## Overview

This repository contains a powerful and flexible procedural terrain generator for the Unity game engine. It allows you to create detailed and realistic landscapes from scratch using a variety of advanced techniques, including fractal noise, domain warping, and hydraulic erosion.

The entire generation process is controlled through a user-friendly custom editor window and a `ScriptableObject`-based settings asset, which allows for easy creation, modification, and reuse of terrain presets.

## Features

- **Procedural Noise Foundation:** Generates terrain using multi-layered fractal noise (fBm) for a detailed base.
- **Advanced Topography:** Implements domain warping to create more natural and organic features like winding mountain ranges and valleys.
- **Hydraulic Erosion Simulation:** A key feature for realism. A particle-based simulation carves out river networks, creates realistic slopes, and forms sedimentary deposits.
- **Custom Editor Window:** A dedicated editor window (`Window > Procedural Terrain Generator`) provides a central hub for controlling the generation process.
- **Live Preview:** See the generated terrain (including water levels) in a real-time preview before applying it.
- **Seed-Based Generation:** Every terrain is based on a seed, allowing for infinite variations that are fully reproducible.
- **ScriptableObject-Based Settings:** Create and save different terrain presets as `TerrainData` assets, making your settings portable and easy to manage.
- **Artistic Control:** Fine-tune the final look of the terrain with a global shaping curve and an adjustable water level.

## How to Use

### 1. Open the Generator Window
You can access the main tool by navigating to **Window > Procedural Terrain Generator** in the Unity top menu bar.

### 2. Create a Terrain Data Asset
The generator's behavior is controlled by a `TerrainData` asset. To create one:
- In the `Project` panel, right-click to open the context menu.
- Navigate to **Create > Procedural Terrain > Terrain Data**.
- A new `TerrainData` asset will be created. Select it to view and edit its parameters in the Inspector.

### 3. Create a Terrain Object
This tool applies the generated heightmap to a standard Unity `Terrain` object.
- In your scene, create a new `Terrain` object by navigating to **GameObject > 3D Object > Terrain**.

### 4. Use the Generator
1.  **Dock the Generator Window:** It's helpful to dock the "Procedural Terrain Generator" window somewhere accessible, like next to the Inspector.
2.  **Assign Assets:**
    *   Drag your newly created **`TerrainData`** asset from the Project panel into the "Terrain Data" slot in the generator window.
    *   Drag the **`Terrain`** object from your Hierarchy panel into the "Target Terrain" slot.
3.  **Adjust Parameters:** Select your `TerrainData` asset and tweak the parameters in the Inspector. See the "Parameters Explained" section below for details on what each setting does.
4.  **Generate a Preview:** Click the **"Generate Preview"** button in the generator window. A preview of the heightmap will appear in the window. Any changes you make to the `TerrainData` parameters will require you to click this button again to see the updated preview.
5.  **Apply to Terrain:** Once you are happy with the preview, click the **"Apply to Terrain"** button. The generator will apply the final heightmap to your `Terrain` object in the scene.

## Parameters Explained

All parameters are located on the `TerrainData` ScriptableObject.

### Terrain Dimensions
- **Terrain Width / Height:** The resolution of the generated heightmap. For best results with Unity's terrain system, this should be a power of two (e.g., 256, 512, 1024).
- **Terrain Depth:** The maximum height of the terrain in Unity units.

### Noise Settings
- **Seed:** The starting point for the random number generator. The same seed will always produce the same terrain.
- **Noise Scale:** Controls the "zoom" level of the noise. Higher values result in larger, more spread-out features.
- **Octaves:** The number of noise layers to combine. More octaves add more fine detail to the terrain.
- **Persistence:** Controls how much the amplitude of each successive octave decreases. Lower values result in smoother terrain.
- **Lacunarity:** Controls how much the frequency of each successive octave increases. Higher values add more small-scale detail.
- **Offset:** A global X/Y offset for the noise map, allowing you to pan around the noise space.

### Erosion Settings
- **Perform Erosion:** A toggle to enable or disable the hydraulic erosion simulation.
- **Erosion Iterations:** The number of water droplets to simulate. More droplets result in a more heavily eroded landscape.
- **Erosion Radius:** The radius around a droplet where terrain is eroded.
- **Inertia:** How much a droplet's momentum is preserved. Lower values cause water to follow the slope more closely.
- **Sediment Capacity Factor:** A multiplier for how much sediment a droplet can carry.
- **Min Sediment Capacity:** A floor value to ensure droplets can always carry a small amount of sediment, even on flat terrain.
- **Erode Speed:** How quickly terrain is eroded when a droplet has spare sediment capacity.
- **Deposit Speed:** How quickly sediment is deposited when a droplet is over-capacity.
- **Evaporate Speed:** The rate at which water evaporates from a droplet, reducing its sediment capacity over time.
- **Gravity:** A multiplier for the force of gravity, affecting droplet acceleration.
- **Max Droplet Lifetime:** The maximum number of steps a droplet can move before it is removed.
- **Initial Water Volume / Speed:** The starting values for each new droplet.

### Global Shaping
- **Shaping Curve:** An `AnimationCurve` that remaps the final height values of the terrain. This is a powerful tool for artistic control, allowing you to create features like plateaus, flat plains, or sharper peaks by modifying the curve.

### Domain Warping
- **Use Domain Warping:** A toggle to enable or disable domain warping.
- **Domain Warp Strength / Frequency:** Controls the intensity and scale of the noise used to distort the terrain coordinates, creating more organic and less grid-like features.

### Water
- **Water Level:** A value between 0 and 1 representing the global sea level. Any part of the terrain with a height value below this will be colored blue in the preview.
