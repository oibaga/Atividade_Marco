using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    private Vector3 move;


    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0, move.y);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10);
        }

        animator.SetBool("isMoving", movement != Vector3.zero);
        characterController.Move(movement * Time.deltaTime * speed);
    }
}