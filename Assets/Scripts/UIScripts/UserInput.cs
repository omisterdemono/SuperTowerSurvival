using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2 MoveInput { get; private set; }
    public bool AttackInput { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _attackAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _playerInput = GetComponent<PlayerInput>();
        SetupInputs();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }

    private void SetupInputs()
    {
        _moveAction = _playerInput.actions["Move"];
        _attackAction = _playerInput.actions["Attack"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        AttackInput = _attackAction.IsPressed();
    }
}
