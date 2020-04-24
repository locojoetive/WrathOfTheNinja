using UnityEngine.SceneManagement;
using UnityEngine;

public class Reload : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<NinjaStatesAnimationSound>().KillNinja();
        }
    }
}
