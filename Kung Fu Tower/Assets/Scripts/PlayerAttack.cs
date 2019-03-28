using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int playerNumber;

    public string whichArm;

    public PlayerAttack otherArm;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNumber == 1)
        {
            if (whichArm.Equals("left") && Input.GetKeyDown(KeyCode.Q))
            {
                animator.SetTrigger("swipeTrigger");
            }
            if (whichArm.Equals("right") && Input.GetKeyDown(KeyCode.E))
            {
                animator.SetTrigger("swipeTrigger");
            }
        }
        else if (playerNumber == 2)
        {
            if (whichArm.Equals("left") && Input.GetKeyDown(KeyCode.Home))
            {
                animator.SetTrigger("swipeTrigger");
            }
            if (whichArm.Equals("right") && Input.GetKeyDown(KeyCode.PageUp))
            {
                animator.SetTrigger("swipeTrigger");
            }
        }
    }

    public Animator GetArmAnimator()
    {
        return animator;
    }
}
