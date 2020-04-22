using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTextBox : MonoBehaviour
{
    public string conversationId;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && InputManager.action)
        {
            Time.timeScale = 0F;
            Conversation conversation = TextImporter.textFileToConversation(conversationId);
            TextBoxManager.EnableTextBox(conversation);
        }
    }
}
