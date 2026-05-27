using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using extOSC;



public class PlayerController : MonoBehaviour
{
    float desired_acceleration = 0;
    float starttime;
    public float impulse;
    float steering;
    public TextMeshProUGUI timelbl;
    public TextMeshProUGUI laps;
    public coinManager cm;
    public GameObject Player;
    public OSCTransmitter transmitter;
    public GameObject RespawnPoint;
    public OSCController oscController;
    [SerializeField] InputActionReference resetAction;

    // debounce last sent value to avoid flooding PD
    private float _lastSentMove = -1f;
    private const float kSendThreshold = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starttime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Display current time
        timelbl.text = string.Format("Current time: {0:F2}", (Time.time - starttime));

        // Add force to player
        GetComponent<Rigidbody>().AddRelativeForce(desired_acceleration * impulse, 0, 0);
        // Camera turn with A and D
        transform.Rotate(0, steering * 100f * Time.deltaTime, 0);

    }
    // When the player uses W and S
    void OnMove(InputValue action)
    {
        var movement = action.Get<Vector2>();
        desired_acceleration = movement.y;
        steering = movement.x;

        float moveIntensity = movement.magnitude;

        if (Mathf.Abs(moveIntensity - _lastSentMove) > kSendThreshold)
        {
            if (oscController != null)
            {
                oscController.SendPlayerMoving(moveIntensity);
            }
            else
            {
                // fallback: send simple transmitter message if needed
                var message = new OSCMessage("/unity/move");
                message.AddValue(OSCValue.Float(moveIntensity));
                transmitter?.Send(message);
            }

            _lastSentMove = moveIntensity;
        }
    }

    // When R button is pressed
    void OnRestart(InputAction.CallbackContext action)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //RespawnatCheckpoint();
    }

    public void RespawnatCheckpoint()
    {
        Player.transform.position = RespawnPoint.transform.position;
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, 0);
        //starttime = Time.time;
    }

    // Player touches coin
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CoinTrigger"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
            oscController.SendCoinPickup();
        }
    }

    // R to restart
    private void OnEnable()
    {
        resetAction.action.performed += OnRestart;
        resetAction.action.Enable();
    }
    private void OnDisable()
    {
        resetAction.action.performed -= OnRestart;
        resetAction.action.Disable();
    }
}
