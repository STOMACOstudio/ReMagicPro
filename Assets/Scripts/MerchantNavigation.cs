using UnityEngine;
using UnityEngine.SceneManagement;

public class MerchantNavigation : MonoBehaviour
{
    public void GoToMerchant()
    {
        SceneManager.LoadScene("MerchantScene");
    }

    public void ExitMerchant()
    {
        SceneManager.LoadScene("MapScene");
    }
}
