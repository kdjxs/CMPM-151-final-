using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    //private RespawnScript respawn;

    /*private void Awake()
    {
        respawn = GetComponent<RespawnScript>();
        //respawn = GameObject.FindGameObjectWithTag("RespawnCol").GetComponent<RespawnScript>();
    }
    */
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
