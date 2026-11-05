using UnityEngine;

public class boss_idle_follow : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    float enterTime;
    public float speed = 5f;
    public float attackRange = 3f;
    public float attackDelay = 1f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        enterTime = Time.time;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<enemy>().LookAtPlayer();

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
        rb.MovePosition(newPos);

        bool inRange = Vector2.Distance(player.position, rb.position) <= attackRange;
        bool delayPassed = Time.time >= enterTime + attackDelay;
        Debug.Log("inRange: " + inRange + " delayPassed: " + delayPassed + " timeInIdle: " + (Time.time - enterTime));

        if (inRange && delayPassed)
        {
            animator.SetTrigger("attack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack");
    }
}