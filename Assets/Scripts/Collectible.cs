using UnityEngine;
using TMPro;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    public string itemName = "Item";
    public GameObject textObject; // Drag your 3D Text object here

    private TextMeshPro tmpComponent;

    void Start()
    {
        // Get the actual text component and format it
        tmpComponent = textObject.GetComponent<TextMeshPro>();
        tmpComponent.text = "Press Q to collect\n" + "<color=yellow>" + itemName + "</color>";
        
        // Ensure it's hidden at the start
        textObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(false);
        }
    }

    void Update()
    {
        // Bonus: Make the text always face the player so it's readable
        if (textObject.activeSelf)
        {
            textObject.transform.LookAt(textObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                      Camera.main.transform.rotation * Vector3.up);
        }
    }
}