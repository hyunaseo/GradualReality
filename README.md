# **GradualReality**
![Introduction](Figures/1.Introduction.png)

**Authors**: [Hyuna Seo](https://hyunaseo.github.io), Juheon Yi, Rajesh Balan, Youngki Lee 

**Publication**: ACM UIST, Oct 2024

**Paper**: [GradualReality: Enhancing Physical Object Interaction in Virtual Reality via Interaction State-Aware Blending](https://doi.org/10.1145/3654777.3676463)

## Research project description
We present **GradualReality** system that enables interaction with physical objects in the immersive virutal environment. Most prior work has relied on the Pass-Through technique, which excessively exposes real-world information in the virtual environment to support interaction. However, this causes an extreme trade-off between usability and immersion, deteriorating the user experience and hindering daily VR usage. To address this issue, we propose an **Interaction State-Aware Blending** approach for GradualReality system, which gradually blends real-world information in the virtual environment based on the current user's interaction context.

## Main Features 

## Prerequisites 
### Hardware Requirements
- HTC VIVE PRO 2
- VIVE Tracker 3.0 for object tracking 
- Leap Motion for hand tracking 
- ZED Mini camera for Pass-Through implementation 
- Windows 11 computer with 8 GB RAM or more and NVIDIA GTX 2070 or higher satisfying ZED Mini requirements 

#### ZED Mini and Leap Motion Setup with HTC Vive Pro 2
We have attached the ZED Mini and Leap Motion to the HTC VIVE PRO 2 as shown below.
<p align="left">
    <img src="Figures/7.DeviceSetup.png" alt="Device Setup" width="30%" style="float:left; margin-right:30px;">
</p>

1. Zed Mini
- The ZED Mini should be aligned with the center of the front camera of the HTC VIVE PRO 2. 
- Putting it below the HMD's camera is one possible option, but you will need to adjust the zed camera's parameters, which ~~is a disaster~~ can make implementing Pass-Through very difficult (it becomes challenging to accurately apply Pass-Through to the exact position of objects).

2. Leap Motion 
- The Leap Motion is attached below the front camera of the HTC VIVE PRO 2. 
- Since the ZED Mini obstructs the front camera, it is not possible to use hand tracking provided by VIVE, Steam, or OpenXR, making the Leap Motion necessary.


## Code

## Initial Settings

### Camera rig settings 