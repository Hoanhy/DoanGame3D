using UnityEngine;

public class BookFloatAnim : MonoBehaviour
{
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;
    public float swayAngle = 10f;
    public float swaySpeed = 2f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Lơ lửng lên xuống
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, yOffset, 0);

        // Lắc lư
        float zRot = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
        transform.localRotation = Quaternion.Euler(0, 0, zRot);
    }
}
