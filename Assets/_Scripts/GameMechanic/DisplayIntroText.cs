using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIntroText : MonoBehaviour
{
    private bool hasIntroEnded;
    private void Start()
    {
        TextBoxManager.EnableTextBox(TextImporter.textFileToConversation("intro"));
    }

    private void Update()
    {
        if (TextBoxManager.conversation != null && TextBoxManager.conversation.hasEnded() && !hasIntroEnded)
        {
            hasIntroEnded = true;
            StageManager.hasTextSceneEnded = hasIntroEnded;
        }
    }
}
