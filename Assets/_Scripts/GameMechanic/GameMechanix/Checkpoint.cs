using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int newCheckpoint = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StageManager.changeCurrentCheckpoint(newCheckpoint);
    }
}
