using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerFPSController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float interactRange = 1f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] GameObject interactionPrompt;

    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;

    private Vector2 inputMove;
    private Vector2 inputLook;

    private float verticalLookRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        interactAction = playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        moveAction.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => inputMove = Vector2.zero;

        lookAction.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => inputLook = Vector2.zero;

        interactAction.performed += _ => TryInteract();
    }

    private void OnDisable()
    {
        moveAction.performed -= ctx => inputMove = ctx.ReadValue<Vector2>();
        moveAction.canceled -= ctx => inputMove = Vector2.zero;

        lookAction.performed -= ctx => inputLook = ctx.ReadValue<Vector2>();
        lookAction.canceled -= ctx => inputLook = Vector2.zero;

        interactAction.performed -= _ => TryInteract();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CheckForInteractable();

        if (MenuOpen()) return;

        float mouseX = inputLook.x * mouseSensitivity;
        float mouseY = inputLook.y * mouseSensitivity;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);

        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void TryInteract()
    {
        if (MenuOpen()) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private bool MenuOpen()
    {
        NoteBookManager notebook = FindFirstObjectByType<NoteBookManager>();
        ClueBoardManager clueBoard = FindFirstObjectByType<ClueBoardManager>();
        DialogueManager dialogue = FindFirstObjectByType<DialogueManager>();
        BodyInspector bodyInspector = FindFirstObjectByType<BodyInspector>();

        bool notebookOpen = notebook != null && notebook.IsNotebookOpen;
        bool clueBoardOpen = clueBoard != null && clueBoard.IsBoardOpen;
        bool dialogueOpen = dialogue != null && dialogue.IsDialogueOpen;
        bool bodyInspectorOpen = bodyInspector != null && bodyInspector.IsInspectorOpen;

        return notebookOpen || clueBoardOpen || dialogueOpen || bodyInspectorOpen;
    }

    private void CheckForInteractable()
    {
        if(MenuOpen())
        {
            interactionPrompt.SetActive(false);
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            if(hit.collider.TryGetComponent<IInteractable>(out var _))
            {
                interactionPrompt.SetActive(true);
                return;
            }
        }

        interactionPrompt.SetActive(false);
    }
}
