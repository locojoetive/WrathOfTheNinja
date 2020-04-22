using UnityEngine;

public class OpenWindow : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StageManager.InitializeNextStage();
        }
    }
}
