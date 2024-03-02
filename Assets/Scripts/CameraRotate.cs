using UnityEngine;


public class CameraRotate : MonoBehaviour
{
    public Camera cam;
    public bool isControlable;
    public Transform target;
    public float distance = 5.0f;
    public float Speed = 50.0f;



    public float smoothTime = 2f;

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    float velocityX = 0.0f;
    float velocityY = 0.0f;

    private Vector2 LastMousePos;
    private Vector2 MousePos;




    public void SetControllable(bool value)
    {
        isControlable = value;
    }

    // Use this for initialization
    void Start()
    {

        Vector3 angles = transform.eulerAngles;

        rotationYAxis = (rotationYAxis == 0) ? angles.y : rotationYAxis;
        rotationXAxis = angles.x;
    }
    private void FixedUpdate()
    {
        MousePos = Input.mousePosition;
        if (Input.GetMouseButton(0) && isControlable)
        {
       
                    //------------------------------------------------------------------------------
                    //UNLOCKED
                    velocityX += Speed * Input.GetAxis("Mouse X") * 0.02f;
                    //------------------------------------------------------------------------------
                    
        }
        LastMousePos = MousePos;
        rotate();
        if (Input.GetMouseButton(0) && isControlable)
        {
            velocityY += Speed * Input.GetAxis("Mouse Y") * 0.02f;
        }
    }

    public void rotate()
    {
        if (target)
        {

            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
        

            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;

            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

        }

    }


    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}