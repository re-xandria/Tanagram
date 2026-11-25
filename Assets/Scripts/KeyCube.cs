using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class KeyCube : MonoBehaviour
{
    // public List<Key> keys;
    public int direction = 0;
    public GameObject cube;

    public float turnSpeed = 360f; // Degrees per second
    private Quaternion targetRotation;



    void Start()
    {
        targetRotation = transform.rotation;

        InputAction rotate = InputSystem.actions.FindAction("RotateCube");
        rotate.performed += button => RotateCube(button);
    }

    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    // we wont physicially rotate, we'll animatefgfv
    public void RotateCube(InputAction.CallbackContext btn)
    {
        KeyControl key = btn.control as KeyControl;

        if (key.keyCode == Key.LeftArrow && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            targetRotation = Quaternion.Euler(0, 90, 0) * targetRotation;
        }
        else if (key.keyCode == Key.RightArrow && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            targetRotation = Quaternion.Euler(0, -90, 0) * targetRotation;
        }
        else if (key.keyCode == Key.UpArrow && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            targetRotation = Quaternion.Euler(-90, 0, 0) * targetRotation;
        }
        else if (key.keyCode == Key.DownArrow && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            targetRotation = Quaternion.Euler(90, 0, 0) * targetRotation;
        }
    }

    // public void TriggerKeyPress(keys key) {}

}
