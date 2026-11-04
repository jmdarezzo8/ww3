# WW3 Third-Party Character Credits

**Model:** soldier low poly mesh game ready 4k PBR rigged
**Creator:** kolopolo (https://sketchfab.com/kolopolo)
**Source:** https://sketchfab.com/3d-models/soldier-low-poly-mesh-game-ready-4k-pbr-rigged-ca94a10b1bbf428fad5a63158fb691ef
**License:** Creative Commons Attribution 4.0 International (CC BY 4.0)
**License text:** https://creativecommons.org/licenses/by/4.0/

These attribution details are embedded in the original uploaded GLB's glTF asset metadata.
This repository contains **Unity integration code only**. The 180 MB third-party model is installed locally, rather than redistributed through GitHub.
If WW3 ships using this model, keep this creator credit in game credits and release materials, identify any alterations, and retain the license link.

### Technical inspection, 2026-11-04
- Binary glTF 2.0, one skin with 111 listed joints.
- 17 embedded PNG images, most 4096×4096; one 2048×2048 and one 1024×1024.
- Finger chains named for thumb/index/middle/ring/pinky, three segments each on each side.
- One walk animation (223 channel bindings).
- 8 mesh objects; 7 material definitions. Uses `KHR_materials_pbrSpecularGlossiness`.
- 187,841,304-byte GLB file.
- Not yet rendered or physically deformation-tested in Unity. Bones present does not prove correct skin weights or animation.

### Technical notes
- Unity 6 requires `com.unity.cloud.gltfast` for .glb Editor importing.
- Import/compile must finish before testing.
- The model contains a walk animation, not a verified idle/run/jump animation set. The current character controller supports movement but animation blending and full-body controls still need validation/implementation.
