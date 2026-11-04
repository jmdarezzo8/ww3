#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class WW3OneClickCharacterTest
{
    private const string ScenePath = "Assets/WW3/Scenes/WW3_CharacterTest.unity";
    private const string PrefabPath = "Assets/WW3/Characters/Soldier01/Prefabs/Soldier01_Prototype.prefab";

    static WW3OneClickCharacterTest()
    {
        // The first time this script is imported, generate the test without manual Unity setup.
        EditorApplication.delayCall += AutoSetup;
    }

    private static void AutoSetup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        BuildScene();
    }

    [MenuItem("WW3/Open Character Test")]
    public static void OpenTest()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            BuildScene();
        }
        else
        {
            EditorSceneManager.OpenScene(ScenePath);
        }
    }

    [MenuItem("WW3/Rebuild Character Test")]
    public static void Rebuild()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        BuildScene();
    }

    private static void BuildScene()
    {
        Folder("Assets/WW3");
        Folder("Assets/WW3/Scenes");
        Folder("Assets/WW3/Characters");
        Folder("Assets/WW3/Characters/Soldier01");
        Folder("Assets/WW3/Characters/Soldier01/Prefabs");
        Folder("Assets/WW3/Characters/Soldier01/Materials");

        Material fabric = MaterialAt("Fabric", new Color(.23f, .27f, .20f));
        Material vest = MaterialAt("Vest", new Color(.17f, .19f, .14f));
        Material boots = MaterialAt("Boots", new Color(.095f, .083f, .072f));
        Material skin = MaterialAt("Skin", new Color(.64f, .46f, .34f));
        Material dark = MaterialAt("Dark", new Color(.045f, .05f, .05f));

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject soldier = new GameObject("Soldier01 - Geometry Prototype");
        // Geometric stand-in for import verification: NOT a finished realistic character.
        Part(soldier.transform, "Pelvis", PrimitiveType.Cube, new Vector3(0, 1.02f, 0), new Vector3(.46f,.28f,.24f), fabric);
        Part(soldier.transform, "Torso", PrimitiveType.Cube, new Vector3(0, 1.42f, 0), new Vector3(.52f,.57f,.29f), fabric);
        Part(soldier.transform, "Plate carrier", PrimitiveType.Cube, new Vector3(0,1.43f,.18f), new Vector3(.55f,.45f,.12f),vest);
        Part(soldier.transform, "Neck", PrimitiveType.Cylinder, new Vector3(0,1.78f,0), new Vector3(.11f,.09f,.11f),skin);
        Part(soldier.transform, "Head", PrimitiveType.Sphere, new Vector3(0,1.94f,0), new Vector3(.27f,.33f,.26f),skin);
        Part(soldier.transform, "Helmet", PrimitiveType.Sphere, new Vector3(0,2.08f,0), new Vector3(.32f,.18f,.31f),vest);
        Part(soldier.transform, "Helmet rim", PrimitiveType.Cylinder, new Vector3(0,2.047f,0), new Vector3(.35f,.025f,.34f),vest);
        Part(soldier.transform, "Visor", PrimitiveType.Cube, new Vector3(0,1.96f,.15f), new Vector3(.29f,.07f,.045f),dark);
        for (int side = -1; side <= 1; side += 2)
        {
            float s = side;
            Part(soldier.transform, s < 0 ? "Left upper arm" : "Right upper arm", PrimitiveType.Capsule,
                new Vector3(s*.37f,1.48f,0), new Vector3(.19f,.43f,.20f),fabric);
            Part(soldier.transform, s < 0 ? "Left forearm" : "Right forearm", PrimitiveType.Capsule,
                new Vector3(s*.39f,1.13f,.04f), new Vector3(.17f,.36f,.19f),fabric);
            Part(soldier.transform, s < 0 ? "Left glove" : "Right glove", PrimitiveType.Sphere,
                new Vector3(s*.39f,.91f,.05f), new Vector3(.17f,.20f,.19f),dark);
            Part(soldier.transform, s < 0 ? "Left leg" : "Right leg", PrimitiveType.Capsule,
                new Vector3(s*.15f,.65f,0), new Vector3(.24f,.62f,.27f),fabric);
            Part(soldier.transform, s < 0 ? "Left boot" : "Right boot", PrimitiveType.Cube,
                new Vector3(s*.15f,.17f,.08f), new Vector3(.27f,.34f,.43f),boots);
            Part(soldier.transform, s < 0 ? "Left vest pouch" : "Right vest pouch", PrimitiveType.Cube,
                new Vector3(s*.17f,1.31f,.26f),new Vector3(.17f,.19f,.09f),vest);
        }

        PrefabUtility.SaveAsPrefabAsset(soldier, PrefabPath);
        Object.DestroyImmediate(soldier);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        GameObject soldierInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        soldierInstance.transform.position = Vector3.zero;

        var floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floor.name = "Display Stand";
        floor.transform.position = new Vector3(0,-.14f,0);
        floor.transform.localScale = new Vector3(1.3f,.13f,1.3f);
        floor.GetComponent<Renderer>().sharedMaterial = dark;

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0,-.3f,0);
        ground.transform.localScale = Vector3.one * 2.5f;
        ground.GetComponent<Renderer>().sharedMaterial = vest;

        var sun = new GameObject("Key Light").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.35f;
        sun.transform.rotation = Quaternion.Euler(35f,-30f,0);
        var fill = new GameObject("Fill Light").AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = .55f;
        fill.transform.rotation = Quaternion.Euler(25f,160f,0);

        GameObject cameraObject = new GameObject("Main Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.fieldOfView = 48;
        camera.transform.position = new Vector3(3.1f,2.0f,4.6f);
        camera.transform.LookAt(new Vector3(0,1.12f,0));
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<WW3PreviewOrbit>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(ScenePath);
        Debug.Log("WW3 character test generated. Press Play to orbit around the prototype soldier. This is a geometric test placeholder, not a 4K model.");
    }

    private static void Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent,false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.GetComponent<Renderer>().sharedMaterial = material;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    private static Material MaterialAt(string name, Color color)
    {
        string path = "Assets/WW3/Characters/Soldier01/Materials/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var material = new Material(shader);
        material.color = color;
        AssetDatabase.CreateAsset(material,path);
        return material;
    }

    private static void Folder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        int slash = path.LastIndexOf('/');
        string parent = path.Substring(0,slash);
        if (!AssetDatabase.IsValidFolder(parent)) Folder(parent);
        AssetDatabase.CreateFolder(parent,path.Substring(slash+1));
    }
}
#endif
