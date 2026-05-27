using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class coinManager : MonoBehaviour
{
    public int coinCount;
    public TextMeshProUGUI coinText;

    void Start()
    {
        // Initialize score
        coinCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Display score
         coinText.text = string.Format("Score: {0}", (coinCount));
    }

}
