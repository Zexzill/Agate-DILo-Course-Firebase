using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    private void Start()
    {
        UserDataManager.load();
        SceneManager.LoadScene(1);
    }
}
