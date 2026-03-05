using UnityEngine;

public class Document : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectDocument();
            Destroy(gameObject);
        }
    }
}