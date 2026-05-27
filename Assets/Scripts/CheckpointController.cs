using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CheckpointController : MonoBehaviour
{
    int amtLaps = 0;
    public MeshRenderer left;
    public MeshRenderer right;
    public CheckpointController next;
    public CheckpointController target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target.left.materials[0].color = Color.red;
        target.right.materials[0].color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        VehicleController vehicle = other.gameObject.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            next.left.materials[0].color = Color.red;
            next.right.materials[0].color = Color.red;
            left.materials[0].color = Color.white;
            right.materials[0].color = Color.white;
        }

    }
}
