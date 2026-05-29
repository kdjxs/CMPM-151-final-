using System;
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
    [SerializeField] private float groundCheckDistance = 8f;
    [SerializeField] private bool logGroundMaterialChanges = true;

    // debounce last sent value to avoid flooding PD
    private float _lastSentMove = -1f;
    private const float kSendThreshold = 0.01f;
    private const int HardMaterialId = 0;
    private const int SandMaterialId = 1;
    private const int MuteMaterialId = 2;
    private const float kGroundContactGraceTime = 0.2f;
    private int _lastSentMaterial = -1;
    private Collider _currentGroundCollider;
    private float _lastGroundContactTime = -1f;

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

        CheckGroundMaterial();

    }

    private void CheckGroundMaterial()
    {
        int materialId = TryGetGroundMaterialId(out int detectedMaterialId) ? detectedMaterialId : MuteMaterialId;

        if (materialId == _lastSentMaterial)
        {
            return;
        }

        if (oscController != null)
        {
            oscController.SendMaterial(materialId);
        }
        else
        {
            var message = new OSCMessage("/unity/material");
            message.AddValue(OSCValue.Int(materialId));
            transmitter?.Send(message);
        }

        _lastSentMaterial = materialId;

        if (logGroundMaterialChanges)
        {
            Debug.Log($"Ground sound material: {materialId}");
        }
    }

    private bool TryGetGroundMaterialId(out int materialId)
    {
        materialId = MuteMaterialId;
        Collider groundCollider = GetRecentGroundContact();

        if (groundCollider == null)
        {
            groundCollider = RaycastForGround();
        }

        if (groundCollider == null)
        {
            return false;
        }

        materialId = GetMaterialId(groundCollider);
        return true;
    }

    private Collider GetRecentGroundContact()
    {
        if (_currentGroundCollider != null && Time.time - _lastGroundContactTime <= kGroundContactGraceTime)
        {
            return _currentGroundCollider;
        }

        return null;
    }

    private Collider RaycastForGround()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, groundCheckDistance, ~0, QueryTriggerInteraction.Ignore);

        Collider groundCollider = null;
        float closestDistance = float.PositiveInfinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                groundCollider = hit.collider;
            }
        }

        return groundCollider;
    }

    private static int GetMaterialId(Collider groundCollider)
    {
        if (HasSurfaceKeyword(groundCollider, "Sand"))
        {
            return SandMaterialId;
        }

        return HardMaterialId;
    }

    private static bool HasSurfaceKeyword(Collider groundCollider, string keyword)
    {
        if (ContainsKeyword(groundCollider.sharedMaterial != null ? groundCollider.sharedMaterial.name : string.Empty, keyword))
        {
            return true;
        }

        for (Transform current = groundCollider.transform; current != null; current = current.parent)
        {
            if (ContainsKeyword(current.gameObject.name, keyword))
            {
                return true;
            }
        }

        Renderer renderer = groundCollider.GetComponent<Renderer>();
        if (renderer == null)
        {
            return false;
        }

        foreach (Material material in renderer.sharedMaterials)
        {
            if (material != null && ContainsKeyword(material.name, keyword))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsKeyword(string value, string keyword)
    {
        return value.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            if (contact.normal.y > 0.25f)
            {
                _currentGroundCollider = collision.collider;
                _lastGroundContactTime = Time.time;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == _currentGroundCollider)
        {
            _currentGroundCollider = null;
        }
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
    void OnRestart(InputValue action)
    {
        if (action.isPressed)
        {
            RestartScene();
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

}
