# WW3 — Soldier Character Preview (Unity 6)

This first change adds the **editor automation** for inspecting ONE realistic, licensed soldier model. It does **not** contain or claim to contain a finished soldier mesh, animations, or 4K textures.

## Workflow
1. Obtain a soldier FBX or Unity-compatible prefab under a license that allows use and redistribution in your game. Keep the attribution and proof of license.
2. Import the model and its properly linked textures into your Unity project's Assets folder.
3. Select the model **asset** (not an instance in the Hierarchy) in the Project window.
4. Choose **WW3 > Character > Build Soldier Preview** in the menu bar.
5. Unity saves **Assets/WW3/Scenes/WW3_SoldierPreview.unity** and **Assets/WW3/Characters/Soldier01/Prefabs/Soldier01.prefab**.
6. Inspect materials, skinning, scale, mesh clipping, rig, and performance; then press Play.

## Why no automatic remote model download yet?
Only commit a model once its provenance, game-use license, rigging, and Unity import compatibility are verified. GitHub text-file editing does not magically retrieve a binary FBX or license it. The tool will ask you to select a model until that dependency is provided.

## Safety
- Does not edit your current scene; prompts to save changes before opening a new one.
- Does not modify project packages or game settings.
- Work belongs on a feature branch until tested in Unity.
- This is an **untested editor script**; it has not been compiled in your local Unity Editor.
