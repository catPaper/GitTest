using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {
    

	public void LoadScene(string loadSceneName)
    {
        if (loadSceneName != null) {
            SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
        }
    }

    public void LoadSceneAllMember(string loadSceneName)
    {
        GetComponent<PhotonView>().RPC("SyncLoadScene", PhotonTargets.All,loadSceneName);
    }

    [PunRPC]
    private void SyncLoadScene(string loadSceneName)
    {
        if (loadSceneName != null)
        {
            SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
        }
    }
}
