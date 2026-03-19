using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Level2Manager.Instance != null)
        {
            Level2Manager.Instance.SaveRoom2Checkpoint();
        }
    }
}