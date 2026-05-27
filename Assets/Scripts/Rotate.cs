using UnityEngine;

public class Rotate : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // Rotate coin animation
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }
}
