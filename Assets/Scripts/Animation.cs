using FishNet.Component.Animating;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Animator _animator;
    private NetworkAnimator _networkAnimator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }

    public void SetMoving(bool value)
    {
        _animator.SetBool("Move", value);
    }
    public void Jump()
    {
        _networkAnimator.SetTrigger("Jump");
    }
}
