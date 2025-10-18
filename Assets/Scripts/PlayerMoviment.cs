using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMoviment : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] DialogTrigger dialogTrigger1;
    [SerializeField] private AudioSource stepAudioSource;
    private Vector3 move;
    public Boolean canMove = true;

    private void Awake()
    {
        Cursor.visible = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        dialogTrigger1.TriggerDialog();
    }

    private void Update()
    {
        if (canMove)
        {
            MovePlayer();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindFirstObjectByType<DialogManager>().ShowNextSentence();
        }
    }
    private void MovePlayer()
    {
        Vector3 movement = new Vector3(move.y * -1, 0, move.x);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10);
        }

        animator.SetBool("isMoving", movement != Vector3.zero);
        characterController.Move(movement * Time.deltaTime * speed);
    }

    public void BlockMoviment()
    {
        canMove = false;
        animator.SetBool("isMoving", false);
    }
    public void UnlockMoviment()
    {
        canMove = true;
    }

    private void Step()
    {
        stepAudioSource.Play();
    }
}