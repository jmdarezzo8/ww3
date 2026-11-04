// WW3 - Soldier character preview setup (Unity 6).
// Select a licensed soldier FBX/model prefab in the Project panel, then choose WW3 > Character > Build Soldier Preview.
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WW3SoldierPreviewBuilder
{
    private const string Root = "Assets/WW3";
    private const string SceneFolder = Root + "/Scenes";
    private const string PrefabFolder = Root + "/Characters/Soldier01/Prefabs";

    [MenuItem("WW3/Character/Build Soldier Preview")]
    public static void Build()
    {
        GameObject source = Selection.activeObject as GameObject;
        if (source == null || !AssetDatabase.Contains(source))
        {
            EditorUtility.DisplayDialog("Select a soldier model",
                "First import a legally licensed soldier FBX or model prefab into Assets. Then select it in the Project panel and run WW3 > Character > Build Soldier Preview.", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(source);
        if (!path.StartsWith("Assets/"))
        {
            Debug.LogError("WW3: Select a model from this project's Assets folder.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        EnsureFolder(Root);
        EnsureFolder(SceneFolder);
        EnsureFolder(Root + "/Characters");
        EnsureFolder(Root + "/Characters/Soldier01");
        EnsureFolder(PrefabFolder);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var soldier = (GameObject)PrefabUtility.InstantiatePrefab(source);
        if (soldier == null) soldier = Object.Instantiate(source);
        soldier.name = "Soldier01";
        soldier.transform.position = Vector3.zero;
        soldier.transform.rotation = Quaternion.identity;

        Bounds bounds = CalculateBounds(soldier);
        // Align lowest visible point with the platform top (Y=0).
        soldier.transform.position += Vector3.up * (-bounds.min.y);
        bounds = CalculateBounds(soldier);

        var platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        platform.name = "Character Display Platform";
        platform.transform.position = new Vector3(bounds.center.x, -0.12f, bounds.center.z);
        platform.transform.localScale = new Vector3(2f, 0.12f, 2f);

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0, -0.24f, 0);
        ground.transform.localScale = Vector3.one * 3;

        var key = new GameObject("Key Light").AddComponent<Light>();
        key.type = LightType.Directional;
        key.intensity = 1.3f;
        key.transform.rotation = Quaternion.Euler(42f, -30f, 0);

        var fill = new GameObject("Fill Light").AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.45f;
        fill.transform.rotation = Quaternion.Euler(25f, 145f, 0);

        var cameraObject = new GameObject("Character Preview Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.fieldOfView = 45f;
        float height = Mathf.Max(1.5f, bounds.size.y);
        Vector3 target = bounds.center + Vector3.up * (height * 0.04f);
        camera.transform.position = target + new Vector3(height * 0.8f, height * 0.2f, height * 2.2f);
        camera.transform.LookAt(target);
        camera.clearFlags = CameraClearFlags.Skybox;
        cameraObject.AddComponent<AudioListener>();

        string prefabPath = PrefabFolder + "/Soldier01.prefab";
        // Save a standalone working prefab while keeping its source model intact.
        PrefabUtility.SaveAsPrefabAssetAndConnect(soldier, prefabPath, InteractionMode.AutomatedAction);

        string scenePath = SceneFolder + "/WW3_SoldierPreview.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = soldier;
        Debug.Log("WW3: Soldier preview created at " + scenePath + ". Press Play to inspect it.");
        EditorUtility.DisplayDialog("WW3 Soldier Preview Ready",
            "Scene created: " + scenePath + "\nPrefab created: " + prefabPath + "\nPress Play to inspect your character.", "OK");
    }

    private static void EnsureFolder(string folder)
    {
        if (AssetDatabase.IsValidFolder(folder)) return;
        int split = folder.LastIndexOf('/');
        string parent = folder.Substring(0, split);
        string name = folder.Substring(split + 1);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(root.transform.position + Vector3.up, new Vector3(0.6f, 1.8f, 0.4f));
        Bounds result = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; ++i) result.Encapsulate(renderers[i].bounds);
        return result;
    }
}
#endif
