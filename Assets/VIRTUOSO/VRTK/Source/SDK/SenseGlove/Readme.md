*Sense Glove SDK Prefab Setup*
Since Unity sometimes has trouble importing prefabs, these instructions detail how to set up a pair of hands for Sense Glove with VIRTUOSO

1. Make an empty GameObject called Hands under your SDK Setups root if the setup doesn't currently have one. Keep it enabled.
2. Create an empty GameObject under hands, called it 'SenseGloves'. Disable this GameObject.
3. Attach the script 'SenseGlove_DeviceManager' on this GameObject
4. Attach the script 'SDK_SenseGloveGestureLibrary' on this GameObject.
5. Drag the Prefabs from 'Assets/SenseGlove/Prefabs/SenseGloveHand - Left' and 'Assets/SenseGlove/Prefabs/SenseGloveHand - Right'
6. Select the SenseGlove SDK under Hands for your set-up