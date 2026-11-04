#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class WW3RealSoldierImporter
{
    private const string ModelFolder = "Assets/WW3/Characters/Soldier01/Models";
    private const string ScenePath = "Assets/WW3/Scenes/WW3_RealSoldierTest.unity";
    private const string ReportPath = "Assets/WW3/Characters/Soldier01/RIG_AUDIT.txt";

    static WW3RealSoldierImporter()
    {
        EditorApplication.delayCall += TryCreateFromImportedModel;
    }

    private sealed class WW3SoldierGLBPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (Array.Exists(importedAssets, path =>
                path.StartsWith(ModelFolder + "/", StringComparison.Ordinal) &&
                path.EndsWith(".glb", StringComparison.OrdinalIgnoreCase)))
            {
                EditorApplication.delayCall += TryCreateFromImportedModel;
            }
        }
    }

    private static void TryCreateFromImportedModel()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode) return;
        GameObject model = FindSoldierModel();
        if (model == null)
        {
            Debug.LogWarning("WW3: Real soldier GLB is not imported as a GameObject. Check whether the .glb exists in Assets/WW3/Characters/Soldier01/Models and glTFast is installed.");
            return;
        }
        // Open the REAL soldier scene after this update. The older WW3_CharacterTest
        // scene contains only the simple geometry prototype.
        const string versionKey = "WW3_Soldier_Scene_V2";
        if (EditorPrefs.GetBool(versionKey, false) &&
            AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build(model);
        EditorPrefs.SetBool(versionKey, true);
    }

    [MenuItem("WW3/Character/Build Real Soldier Test")]
    public static void BuildSelected()
    {
        GameObject model = Selection.activeObject as GameObject;
        if (model == null)
        {
            model = FindSoldierModel();
        }
        if (model == null || !AssetDatabase.Contains(model))
        {
            EditorUtility.DisplayDialog("WW3 needs a real 3D model",
                "Place your licensed soldier FBX in " + ModelFolder + ". The test scene will then build itself.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build(model);
    }

    private static GameObject FindSoldierModel()
    {
        // glTFast imports GLB as a native Unity prefab-like asset. Do not depend
        // on the t:Model asset search filter (which varies by importer).
        const string preferred = ModelFolder + "/soldier_low_poly_mesh_game_ready_4k_pbr_rigged.glb";
        GameObject known = AssetDatabase.LoadAssetAtPath<GameObject>(preferred);
        if (known != null) return known;

        if (!AssetDatabase.IsValidFolder(ModelFolder)) return null;
        string[] ids = AssetDatabase.FindAssets("", new[] { ModelFolder });
        foreach (string id in ids)
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            if (!path.EndsWith(".glb", StringComparison.OrdinalIgnoreCase)
                && !path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)
                && !path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)) continue;
            GameObject candidate = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (candidate != null) return candidate;
        }
        return null;
    }

    private static void Build(GameObject source)
    {
        EnsureFolder("Assets/WW3");
        EnsureFolder("Assets/WW3/Scenes");
        EnsureFolder("Assets/WW3/Characters");
        EnsureFolder("Assets/WW3/Characters/Soldier01");
        EnsureFolder(ModelFolder);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject player = new GameObject("WW3 Player");
        var cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.28f;
        cc.center = Vector3.up * 0.9f;
        cc.stepOffset = 0.25f;

        GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(source);
        if (visual == null) visual = UnityEngine.Object.Instantiate(source);
        visual.name = "Soldier Visual Rig";
        visual.transform.SetParent(player.transform, false);
        visual.transform.localPosition = Vector3.zero;

        Renderer[] visibleMeshes = visual.GetComponentsInChildren<Renderer>(true);
        if (visibleMeshes.Length == 0)
        {
            Debug.LogError("WW3: Imported soldier has zero renderers! The GLB did not import its mesh. No empty test scene will be saved.");
            UnityEngine.Object.DestroyImmediate(player);
            return;
        }
        Bounds bounds = GetBounds(visual);
        if (bounds.size.y > 0.01f)
        {
            float scale = Mathf.Clamp(1.8f / bounds.size.y, 0.01f, 100f);
            visual.transform.localScale *= scale;
            bounds = GetBounds(visual);
            visual.transform.position += Vector3.up * (-bounds.min.y);
        }

        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Training Floor";
        floor.transform.localScale = Vector3.one * 4f;
        floor.transform.position = Vector3.zero;

        var light = new GameObject("Sun").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.transform.rotation = Quaternion.Euler(42f, -25f, 0f);

        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        var camera = cameraGO.AddComponent<Camera>();
        camera.fieldOfView = 55;
        // Focus on imported mesh bounds rather than guessing its origin.
        bounds = GetBounds(visual);
        Vector3 center = bounds.center;
        float distance = Mathf.Max(3.5f, bounds.size.magnitude * 1.8f);
        cameraGO.transform.position = center + new Vector3(0, Mathf.Max(0.5f, bounds.size.y * .10f), -distance);
        cameraGO.transform.LookAt(center);
        cameraGO.AddComponent<AudioListener>();

        var movement = player.AddComponent<WW3SoldierMovement>();
        movement.followCamera = camera;
        foreach (var renderer in visibleMeshes)
        {
            if (!renderer.gameObject.activeInHierarchy) renderer.gameObject.SetActive(true);
            renderer.enabled = true;
            var skinned = renderer as SkinnedMeshRenderer;
            if (skinned != null) skinned.updateWhenOffscreen = true;
        }
        Debug.Log("WW3: Model has " + visibleMeshes.Length + " renderers, size " + bounds.size);

        EditorSceneManager.SaveScene(scene, ScenePath);
        WriteAudit(source, visual);
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(ScenePath);
        Debug.Log("WW3 real model scene generated: " + ScenePath +
                  ". This scene is controllable with WASD/Shift/Space. Rig report: " + ReportPath);
    }

    private static void WriteAudit(GameObject source, GameObject instance)
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("WW3 Soldier Rig Audit");
        report.AppendLine("Source: " + AssetDatabase.GetAssetPath(source));
        var skinned = instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        var bones = skinned.SelectMany(x => x.bones)
            .Where(x => x != null).Select(x => x.name).Distinct().ToArray();
        report.AppendLine("Skinned meshes: " + skinned.Length);
        report.AppendLine("Unique skinned bones: " + bones.Length);
        Animator animator = instance.GetComponentInChildren<Animator>(true);
        bool humanoid = animator != null && animator.avatar != null &&
                        animator.avatar.isValid && animator.avatar.isHuman;
        report.AppendLine("Validated humanoid Animator Avatar: " + humanoid);
        foreach (string bone in bones.OrderBy(x => x)) report.AppendLine("  " + bone);

        int fingers = bones.Count(x =>
            x.IndexOf("thumb", StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.IndexOf("index", StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.IndexOf("middle", StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.IndexOf("ring", StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.IndexOf("pinky", StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.IndexOf("little", StringComparison.OrdinalIgnoreCase) >= 0);
        report.AppendLine("Finger-related bone names (heuristic): " + fingers);
        report.AppendLine("NOTE: Bone-name heuristics are NOT proof of correct finger skin weights.");
        report.AppendLine("Check bends, topology, clipping, rig mapping and material textures inside Unity.");
        File.WriteAllText(ReportPath, report.ToString());
    }

    private static Bounds GetBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
            return new Bounds(root.transform.position + Vector3.up, Vector3.one * 1.8f);
        Bounds box = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; ++i) box.Encapsulate(renderers[i].bounds);
        return box;
    }

    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;
        int slash = folder.LastIndexOf('/');
        string parent = folder.Substring(0, slash);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, folder.Substring(slash + 1));
    }
}
#endif
