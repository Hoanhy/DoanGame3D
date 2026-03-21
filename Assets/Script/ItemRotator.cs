using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    // Tốc độ xoay (độ trên giây)
    public float rotationSpeed = 90f;

    void Update()
    {
        // Xoay vật thể quanh trục Y (trục dọc)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}