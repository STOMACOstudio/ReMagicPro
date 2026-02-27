using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    public string itemName = "Item";
    public string starterColor = "Red";
    public GameObject textObject; // Drag your 3D Text object here
    public bool loadSceneAfterCollect = true;
    public string nextSceneName = "MapScene";
    public bool destroyOnCollect = true;

    private TextMeshPro tmpComponent;
    private bool playerInRange;
    private bool hasBeenCollected;

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
            playerInRange = true;
            textObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            textObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasBeenCollected && playerInRange && Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            CollectReward();
            return;
        }

        // Bonus: Make the text always face the player so it's readable
        if (textObject.activeSelf)
        {
            textObject.transform.LookAt(textObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                      Camera.main.transform.rotation * Vector3.up);
        }
    }

    private void CollectReward()
    {
        hasBeenCollected = true;
        playerInRange = false;

        PlayerPrefs.SetString("PlayerColors", starterColor);
        DeckHolder.SelectedDeck = DeckDatabase.BuildPlayerStarterDeck(starterColor);
        PlayerPrefs.Save();

        if (textObject != null)
            textObject.SetActive(false);

        if (destroyOnCollect)
            Destroy(gameObject);

        if (loadSceneAfterCollect && !string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
