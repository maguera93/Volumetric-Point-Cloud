# Volumetric Point Cloud
A Unity package for processing a real-time stream of volumetric point-cloud geometry using the C# Job System and Burst Compiler, with a processing core that has zero dependency on Unity's rendering pipeline.

*TO SEE THE FULL PROJECT CHECK "DEVELOP" BRANCH OF THIS REPOSITORY*

## 1. Installing

To install the package follow this instructions:

  
1. Download the package from the repository page and unzip it.
2. Copy the `com.marc-aguera-garcia.volumsdk` folder into your Unity project's `Packages/` directory, so you
   end up with `<YourProject>/Packages/com.marc-aguera-garcia.volumsdk/package.json`.
3. Open the project in Unity (6000.3.10f1 or newer recommended).
4. Import sample demo from Package Manager to get the sample demo or download it from the develop branch of this repository.

```csharp
using Volum.SDK;

var config = new VolumStreamConfig 
{
    FrameRate = 60,
    PointCount = 50000,
    VolumRadius = 5F,
    NoiseFrequency = 0.15F,
    Speed = 1F
};

var stream = VolumSDK.InitStream(config);
stream.OnFrameReady += UpdateMesh;
stream.Start();
```

That is the entire public API surface. Everything else — job scheduling, native memory, double buffering, per-frame throttling — lives behind it.
