using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    private Vector3 move;

    [SerializeField] float rayLength = 1.5f; // Distância para detectar chão

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
            // Checa se pode mover para essa direção
            if (CanMove(movement))
            {
                // Rotaciona suavemente para direção do movimento
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10);

                // Move o personagem
                characterController.Move(movement * Time.deltaTime * speed);

                animator.SetBool("isMoving", true);
            }
            else
            {
                // Bloqueia movimento e animação se não pode andar
                animator.SetBool("isMoving", false);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private bool CanMove(Vector3 direction)
    {
        // Origem do raycast na frente do player, na altura do personagem
        Vector3 origin = transform.position + direction.normalized * 0.5f;

        // Raycast para baixo a partir da origem
        Ray ray = new Ray(origin, Vector3.down);

        // Desenha o ray na cena para debug (opcional)
        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);

        if (Physics.Raycast(ray, rayLength))
        {
            // Tem chão para pisar, pode andar
            return true;
        }

        // Não tem chão, bloqueia movimento
        return false;
    }
}
