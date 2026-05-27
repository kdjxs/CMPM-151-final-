using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public PlayerController Player;
    public GameObject RespawnPoint;

    // Spawn player at respawn point
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player = other.GetComponent<PlayerController>();
            Player.RespawnatCheckpoint();
            //Player.transform.position = RespawnPoint.transform.position;
        }
    }
}
