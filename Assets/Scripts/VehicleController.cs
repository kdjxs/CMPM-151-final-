using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class VehicleController : MonoBehaviour
{
    int amtLaps = 0;
    float desired_acceleration = 1;
    float starttime;
    public float impulse;
    public float turnrate;
    public CheckpointController target;
    public TextMeshProUGUI timelbl;
    public TextMeshProUGUI laps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       amtLaps = 0;
       starttime = Time.time;
       laps.text = "Lap: " + amtLaps;
       target = GetComponent<CheckpointController>();
    }

    // Update is called once per frame
    void Update()
    {
        timelbl.text = string.Format("Current time: {0:F2} seconds", (Time.time - starttime));

        GetComponent<Rigidbody>().AddRelativeForce(desired_acceleration * impulse, 0, 0);
        float dx = (Mouse.current.position.x.value - Screen.width / 2) / turnrate;
        if (Mathf.Abs(dx) > 0.01f)
        {
            transform.Rotate(0, dx, 0);
        }
    }
    void OnMove(InputValue action)
    {
        var movement = action.Get<Vector2>();
        desired_acceleration = movement.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LapTrigger"))
        {
            amtLaps++;
            laps.text = "Lap: " + amtLaps;
        }
    }
    }
