using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    public Animator animator;
    private PlayerControls controls;

    private bool isAttacking = false;
    private bool isDancing = false;

    [SerializeField] public AudioSource goblinLeap;
    [SerializeField] public AudioSource music;
    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Attack.performed += ctx => HandleAttack();
        controls.Player.Dance.performed += ctx => ToggleDance();
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void HandleAttack()
    {
        if (!isAttacking)
        {
            animator.SetTrigger("Attack");
            goblinLeap.Play();
            isAttacking = true;
            StartCoroutine(ResetAttack(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("AttackLayer")).length));
        }
        else
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
    }

    private void ToggleDance()
    {
        isDancing = !isDancing;
        animator.SetBool("dancing", isDancing);
        Debug.Log("Dance toggled: " + isDancing);

        if (isDancing)
            music.Play();
        else
            music.Stop();
    }

    private System.Collections.IEnumerator ResetAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }
}
