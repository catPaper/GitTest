using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowData : MonoBehaviour {

    [SerializeField]
    private Text dataText;

    void Awake()
    {
        if (PhotonNetwork.playerList.Length != PhotonNetwork.player.ID)
        {
            int endIndex = PhotonNetwork.player.ID;
            PhotonPlayer tmpPlayer;
            for (int i = 0; i < endIndex; i++)
            {
                tmpPlayer = PhotonNetwork.playerList[i];
                PhotonNetwork.playerList[i] = PhotonNetwork.playerList[i + 1];
                PhotonNetwork.playerList[i + 1] = tmpPlayer;
            }
        }
    }

    void Start()
    {
        dataText.text = "";
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            dataText.text += player.name + '\n';
            Debug.Log(player.name +":");
        }
    }
}
