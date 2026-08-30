# volum-sdk-test
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


## 2. Architectural Reasoning & Technical Trade-offs

### 1. Cross-Platform and XR
On a headset like Meta Quest, the CPU is weaker and has fewer cores than a desktop and must run at a higher frame rate to avoid motion sickness. That's why I'd doblecheck that the one frame latency pipeline still holds up at the faster frame rate.

For the heat problems that the headsets may have, I'd add a check that scales down the number of points if the device is overheating. Unity has an Adaptative Performance package that helps tracking the heat of the device and we can use it instead of letting the framerate drop down.

### 2. API Versioning and Breaking Changes
To avoid breaking changes for the client I made sure that the not public engine is internal and hidden behind VolumStream, so we can rewrite the entire job/threading logic without consumers noticing anything, as long as the public methods keep working the same way.

To make this more rigorous, I'd tie version numbers directly to changes in the public API and for changes that could break things. I'd add new methods alongside the oldones and make sure that they're tagged with the `[Obsolete]` tag rather than replacing them. This way, we are giving time to the clients to migrate their code to the newer SDK.

### 3. Performance Trade-offs

The clearest example is the double-buffering setup in PointProcessor: I schedule next frame's job early and only "collect" the result at the start of the following frame, instead of the much simpler approach of just scheduling and immediately waiting for it to finish. The simple version would have been easier to read and debug.

I chose the more complex version because the goal was specifically to never block the main thread and the simpler version does technically block it a little, waiting on the job every frame.
