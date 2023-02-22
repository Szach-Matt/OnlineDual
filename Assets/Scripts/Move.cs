using FishNet.Object;
using UnityEngine;

public class Move : NetworkBehaviour
{
    public float rotationSpeed = 150f;
    public float moveSpeed = 5;
    private CharacterController _characterController;
    private Animation _animating;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animating = GetComponent<Animation>();
    }

    // [Client(RequireOwnership = true)]
    private void Update()
    {
        if (!base.IsOwner) return;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        transform.Rotate(new Vector3(0f, horizontal * rotationSpeed * Time.deltaTime, 0f));
        Vector3 offset = new Vector3(0f, Physics.gravity.y, vertical) * moveSpeed * Time.deltaTime;
        offset = transform.TransformDirection(offset);
        _characterController.Move(offset);

        bool moving = (horizontal != 0f || vertical != 0f);
        _animating.SetMoving(moving);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animating.Jump();
        }




    }
}
