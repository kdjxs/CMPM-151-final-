using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    // Player collides with checkpoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Set players respawn point as the checkpoint
            other.GetComponent<PlayerController>().RespawnPoint = this.gameObject;
            //respawn.RespawnPoint = this.gameObject;
        }
    }
}
