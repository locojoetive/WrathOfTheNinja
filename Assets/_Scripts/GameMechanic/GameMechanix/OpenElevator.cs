using UnityEngine;
using UnityEngine.SceneManagement;


public class OpenElevator : MonoBehaviour {
    Animator animator;
    bool doorOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("opened", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("opened", false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (InputManager.up)
        {
          StageManager.InitializeNextStage();
        }
    }

    public void OnAnimationEnded(bool doorOpened)
    {
        this.doorOpened = doorOpened;
    }
}
