# SteamVR

## Instructions for using SteamVR

 * Import the [SteamVR 1.2.3 Plugin](https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/1.2.3/SteamVR.Plugin.unitypackage) from GitHub.
 * Follow the initial [Getting Started](/Assets/VRTK/Documentation/GETTING_STARTED.md) steps and then add the `[CameraRig]` prefab from the [SteamVR 1.2.3 Plugin](https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/1.2.3/SteamVR.Plugin.unitypackage) as a child of the SDK Setup GameObject.

## Additional Instructions for SteamVR 2.0

 * SteamVR 2.0 introduced a new input system that prohibits reading the raw controller data in favor of 'actions' (this enables user remapping with minimal developer effort, so is overall good for consumers)
 * You must compile the SteamVR inputs in order to compile the new SDK components:
 * Navigate to Window > SteamVR Input and press the 'Generate' button at the base of the window