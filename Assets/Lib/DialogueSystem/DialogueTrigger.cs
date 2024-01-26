using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue() 
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        if (gameManager != null) {
            gameManager.inDialogue = true;
        }
    }
}
