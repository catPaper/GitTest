using UnityEngine;
using System.Collections;

public class Disconnect : Photon.MonoBehaviour {

	public void disconnect()
    {
        PhotonNetwork.Disconnect();
    }
}
