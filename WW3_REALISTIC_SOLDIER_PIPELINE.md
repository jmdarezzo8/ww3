# WW3 — Realistic soldier asset pipeline

**Current status (2026-11-04):** Character movement and automatic model-preview scene generation are in place as SOURCE CODE; not tested inside the user's Mac Unity editor. The realistic mesh and textures are **not present**. The existing toy placeholder remains until a proper asset is imported.

## Quality bar
- Modern U.S.-style military uniform; credible materials, gear and anatomy.
- 4K base-color/normal/roughness source maps where licensed and available.
- Rigged hips/spine/neck/head/clavicles/upper arms/forearms/hands/thighs/shins/feet/toes.
- Every finger individually articulated (thumb, index, middle, ring, little, multiple phalanges per finger on both hands).
- Inspect skin weights at shoulder, elbow, wrist, fingers, pelvis, knee and ankle.
- Commercial redistribution rights documented; exclude ripped game models.
- M2 performance: sensible character mesh budget, texture compression, 2K runtime option and LODs.

## Leading external model candidates (NOT DOWNLOADED OR VERIFIED IN UNITY)

1. **kolopolo — Soldier low-poly mesh game-ready 4K PBR rigged (CC BY)**:
https://sketchfab.com/3d-models/soldier-low-poly-mesh-game-ready-4k-pbr-rigged-ca94a10b1bbf428fad5a63158fb691ef
Author describes 4K PBR and walk animation; around 61.8k triangles. **Finger bone count unknown.** May not have U.S. uniform styling.

2. **RedVeil Studio — American Soldier Rigged (CC BY)**:
https://sketchfab.com/3d-models/american-soldier-rigged-5b1967e7f83547be8a1c3ae656ab1ad4
U.S. styling, rigged according to author, ~9.7k triangles. **Texture resolution and individual fingers unverified.**

3. **BAMEN — Free Military Soldier Rigged (CC BY)**:
https://sketchfab.com/3d-models/free-military-soldier-rigged-e9c56308a67d4a3db62e914fafa4d198
FBX, rigged per author, basic materials. Not yet high-fidelity enough as verified.

Do not use third-party files from sites that describe assets as 'ripped' from commercial games.
Do not substitute 1K textures with upscaled files marketed as native 4K.
CC-BY models require the author, title, license, source URL and modifications to be credited in distribution.

## Integration workflow after licensed model acquired
- Place FBX and original textures in `Assets/WW3/Characters/Soldier01/Models/`.
- Import with Unity 6. When compiler is ready, `WW3RealSoldierImporter` automatically detects the FBX, saves `Assets/WW3/Scenes/WW3_RealSoldierTest.unity`, and configures a character controller and follow camera.
- WASD moves; Left Shift sprints; Space jumps.
- The importer writes `Assets/WW3/Characters/Soldier01/RIG_AUDIT.txt`, listing skinned bone names and a heuristic finger count. **This only inspects the rig, not weights/animation quality.**
- Model shaders/materials may require URP conversion; inspect clipping, scale and deformation manually in Unity.
- Do not call the character production ready until the rig passes fingers/skin-weight check and in-editor verification.

## Current dependency
A legitimate, downloadable, realistically detailed FBX+textures with verified individual finger bones; Sketchfab downloads may require an authorized interactive account sign-in. GitHub text editing cannot legally or technically extract arbitrary binary models from a third-party site, and no asset provider access has been granted.
