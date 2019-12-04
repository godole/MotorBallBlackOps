using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitstop : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {

    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterBase>();

        if (character == null)
            return;

        if (character.HasBall)
            return;

        if (!character.photonView.IsMine)
            return;
        
        character.EnterPitstop();

        UIController.getInstance.PlayPanel.gameObject.SetActive(false);
        UIController.getInstance.PitstopPanel.gameObject.SetActive(true);

        GameSceneManager.getInstance.Player = character.gameObject;
    }
}
