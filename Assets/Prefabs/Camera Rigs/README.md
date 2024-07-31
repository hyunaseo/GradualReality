# Camera Rig Settings 

1. **Main Camera Rig for Virtual Environment**
    - Add `Assets/Prefabs/Main Camera Rig` to the scene, and set its rotation as (0, 90, 0). Or you can follow the below steps to generate the prefab by yourselves. 
        - Add `Assets/Steam/Prefabs/[Camera Rig]` to the scene.
        - Rename it to *Main Camera Rig* and rename its child game object Camera to *Main Camera*.
        - Remove or disable all unnecessary components of *Main Camera Rig* (i.e., Mesh Renderer, Steam VR_Play Area).
        - Add a component called *Tracked Pose Driver* to the *Main Camera*. This is for VR tracking; without it, the scene will not change according to head movements during play.

    - **Important!** After finishing the setup, make sure to set the *Main Camera Rig* and *all its child game objects* to **inactive** in the scene!


2. **Zed Camera Rig to obtain camera feeds** 
    - Add `Assets/Prefabs/Zed Camera Rig` to the scene. Or you can follow the below steps to generate the prefab by yourselves. 
        - Add `Assets/Zed/Prefabs/Zed_Rig_Stereo` to the scene. 
        - Set the *Zed Manager* component of *Zed_Rig_Stereo* as follows:
            - Resolution: HD 1080
            - FPS: 50
            - Enable Tracking, Enable Spatial Memory: false
            - Depth Occlusion, AR Post-Processing: false
        - Disable all components of *Camera_eyes*.
        - Set the Camera component of *Left_eye* and *Right_eye* as follows:
            - Field of view: 101
            - Clipping Planes Near: 0.01
            - Rendering Path: use graphics settings
        - Set the child objects *Frame* of both *Left_eye* and *Right_eye* to **inactive** in the scene.


3. **Pass-Through Camera Rig to obtain render texture for Pass-Through and render it** 
    - Add `Assets/Prefabs/Pass-Through Camera Rig` to the scene. Or yu can follow the below steps to generate the prefab by yourselves. 
    - Create an empty game object in the scene and name it *Pass-Through Camera Rig*. Set its rotation to (0, 90, 0).
    - Under Pass-Through Camera Rig, create two empty game objects and name them *Render Texture Camera* and *Pass-Through UI Camera* respectively.
    - For *Render Texture Camera*, perform the following tasks:
        - Add a Camera component and set the following settings:
            - Clear Flags: Solid Color
            - Background: (0, 0, 0, 0)
            - Culling Mask: Pass-Through Area
            - Clipping Planes Near: 0.01
            - Target Texture: Pass Through Area RT
        - Add a Tracked Pose Driver.
    - For Pass-Through UI Camera, perform the following tasks:
        - Add a Camera component and set the following settings:
            - Clear Flags: Depth Only
            - Culling Mask: UI
            - FoV: 101
            - Clipping planes Near: 0.01
            - Depth: 1
        - Add a Tracked Pose Driver.