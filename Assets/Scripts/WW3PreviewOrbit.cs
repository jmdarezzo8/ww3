using UnityEngine;

// Runtime-only camera orbit for the first WW3 prototype scene.
// No input packages required. Press Play and observe the character.
public class WW3PreviewOrbit : MonoBehaviour
{
    public Vector3 target = new Vector3(0f, 1.15f, 0f);
    public float degreesPerSecond = 12f;

    private void Update()
    {
        transform.RotateAround(target, Vector3.up, degreesPerSecond * Time.deltaTime);
        transform.LookAt(target);
    }
}
