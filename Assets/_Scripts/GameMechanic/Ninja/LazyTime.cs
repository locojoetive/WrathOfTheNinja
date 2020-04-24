using UnityEngine;
using UnityEngine.SceneManagement;

public class LazyTime : MonoBehaviour {
    private int life = 2;

    Animator anim;
    Rigidbody2D rb;
    public int count = 0;
    public bool lazyTime = false, reallyLazy = false;

	void Start () { rb = GetComponent<Rigidbody2D>(); }
	void Update () {
        lazyTime = Mathf.Abs(rb.velocity.x) < 0.000001f
            && Mathf.Abs(rb.velocity.y) < 0.000001f;
        if (lazyTime) count--;
        else count = 0;
        reallyLazy = count < -1000;
    }

    public void InflictDamage()
    {
        life--;
        if (life == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
