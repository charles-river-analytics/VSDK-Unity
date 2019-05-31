*ManusVR SDK Prefab Setup*
Since Unity sometimes has trouble importing prefabs, these instructions detail how to set up a pair of hands for ManusVR with VIRTUOSO

1. Make an empty prefab called Hands under your SDK Setups root if the setup doesn't currently have one
2. Drag the prefab from Assets/ManusVR/Prefabs/ManusVRSeperatedArms to the Hands prefab
4. Add an empty node under the ManusVR root prefab and attach the SDK_ManusVRGestureLibrary script to it
5. To set up hands that can actually interact with VRTK objects, find the hands renderers (Left/TrackerOffset_l/hand_l), add a collider and set isTrigger to true
6. Add the scripts GestureControllerEvent, VRTK_InteractTouch, VRTK_InteractGrab, and VRTK_InteractUse scripts to the 'Left' and 'Right' hand GameObjects
7. Fill in the VRTK scripts with the created colliders and other script references

Note: The ManusVR currently depends on the SteamVR tracker. With the current implementation of VRTK, trying to use a tracker will throw the error:
Assertion failed: Assertion failed on expression: 'IsMatrixValid(matrix)'
over and over again. There is currently a fix in VIRTUOSO, but we have not yet implemented the tracker info into the SDK yet.