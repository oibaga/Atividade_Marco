using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float speed = 3f;
    [SerializeField] float runSpeed = 5f;

    [Header("Referências")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] CameramanAgentScript cameraman;
    [SerializeField] DialogTrigger dialogTrigger1;
    [SerializeField] private AudioSource stepAudioSource;
    [SerializeField] private Transform audioListener;

    [Header("Teclas")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode actionKey = KeyCode.Space;

    private Vector3 move;
    public Boolean canMove = true;
    public Boolean isGrabbed = false;
    public bool isInspecting { get; private set; } = false;
    InteractableObject objInspecting = null;

    public Queue<int> inventoryIndexes;
    private readonly List<InteractableObject> nearbyObjects = new List<InteractableObject>();
    public InteractableObject closestItem = null;

    private float defaultSpeed = 3f;

    private void Awake()
    {
        Cursor.visible = false;
        inventoryIndexes = new Queue<int>();
        defaultSpeed = speed;
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
        audioListener.position = transform.position;

        if (canMove && !isGrabbed)
        {
            MovePlayer();
            closestItem = GetClosestObject();

            if (Input.GetKeyDown(interactKey) && !isInspecting && closestItem)
            {
                InteractWithClosestObject();
            }
        }
        else
        {
            closestItem = null;

            if (Input.GetKeyDown(interactKey) && isInspecting)
            { 
                FindFirstObjectByType<Object_Interact>().StopInspection();
                objInspecting.PickUp( inventoryIndexes );
                isInspecting = false;

                Destroy( objInspecting.gameObject );
                objInspecting = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown( actionKey ))
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

    private void OnTriggerEnter(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null && !nearbyObjects.Contains(interactable))
        {
            nearbyObjects.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableObject interactable = other.GetComponent<InteractableObject>();
        if (interactable != null && nearbyObjects.Contains(interactable))
        {
            nearbyObjects.Remove(interactable);
        }
    }

    private void InteractWithClosestObject()
    {
            closestItem.Inspect();
            objInspecting = closestItem;

            isInspecting = true;
    }

    private InteractableObject GetClosestObject()
    {
        if (nearbyObjects.Count == 0) return null;

        InteractableObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPos = transform.position;

        foreach (var obj in nearbyObjects)
        {
            if (obj == null) continue;

            float dist = Vector3.Distance(playerPos, obj.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = obj;
            }
        }
        return closest;
    }

    private void Step()
    {
        stepAudioSource.Play();
    }

    public KeyCode GetInteractKey()
    {
        return interactKey;
    }

    public KeyCode GetActionKey()
    {
        return actionKey;
    }

    public void StartChase()
    {
        animator.SetBool("isRunning", true);
        cameraman.StartChase();
        speed = runSpeed;
    }
    public void StopChase()
    {
        animator.SetBool("isRunning", false);
        cameraman.StopChase();
        speed = defaultSpeed;
    }
}