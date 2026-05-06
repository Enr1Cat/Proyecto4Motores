using UnityEngine;

public class ShouldPersonAnimations : MonoBehaviour
{
    private static readonly int XSpeed = Animator.StringToHash("SpeedX");
    private static readonly int YSpeed = Animator.StringToHash("SpeedY");
    private ShoulderPersonController _thirdPersonController;
    private Animator anim;
    private void Awake()
    {
        _thirdPersonController = GetComponentInParent<ShoulderPersonController>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _thirdPersonController.OnMoving += UpdateAnimation;
    }

    private void OnDisable()
    {
        _thirdPersonController.OnMoving -= UpdateAnimation;
    }

    private void UpdateAnimation(float xPercentage, float yPercentage)
    {
        anim.SetFloat(XSpeed, xPercentage);
        anim.SetFloat(YSpeed, yPercentage);
    }
}
