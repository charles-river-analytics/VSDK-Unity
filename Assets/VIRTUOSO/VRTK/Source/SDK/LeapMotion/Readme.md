*Leap Motion SDK Prefab Setup*
Since Unity sometimes has trouble importing prefabs, these instructions detail how to set up a pair of hands for Leap Motion with VIRTUOSO

1. Make an empty prefab called Hands under your SDK Setups root if the setup doesn't currently have one
2. Drag the prefab from Assets/LeapMotion/Core/Prefabs/LeapRig to the Hands prefab
3. Disable the camera and audio listener on the leap motion prefab
4. Add an empty node under the Leap Motion root prefab and attach a SDK_LeapMotionGestureLibrary script to it
5. To set up hands that can actually interact with VRTK objects, find the Leap Motion capsule hands and change every capsule to a trigger collider and remove all rigid bodies
6. Add gesture event controller, interact touch, interact grab, and interact use scripts to the capsule hands