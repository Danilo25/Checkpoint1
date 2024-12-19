using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovemment : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float mouseSensitivity = 100f; 
    public Transform cameraTransform; 

    private float xRotation = 0f; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("O Rigidbody não foi encontrado no Player. Por favor, adicione um Rigidbody.");
        }

        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 


        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical"); 

        Vector3 direction = transform.forward * vertical + transform.right * horizontal;

        Vector3 move = direction.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
