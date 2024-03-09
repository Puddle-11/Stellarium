using UnityEngine;


public class CameraRotate : MonoBehaviour
{
    private Vector3 defaultPos;
    private Quaternion defaultRot;

    [SerializeField] private Camera cam;
    public bool isControlable;
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float Speed = 50.0f;
    [SerializeField] private float smoothTime = 2f;
    private float rotationYAxis = 0.0f;
    private float rotationXAxis = 0.0f;
    private float velocityX = 0.0f;
    private float velocityY = 0.0f;
    private Vector2 LastMousePos;
    private Vector2 MousePos;
 
    private float lerpTimer;
   [SerializeField] private float changeSpeed;

    public void SetControllable(bool value)
    {
       
            isControlable = value;
       
    }

    // Use this for initialization
    void Start()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        Vector3 angles = transform.eulerAngles;

        rotationYAxis = (rotationYAxis == 0) ? angles.y : rotationYAxis;
        rotationXAxis = angles.x;
    }
    private void Update()
    {

     
    }
    private void FixedUpdate()
    {
        if (isControlable)
        {

        MousePos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
       
                    //------------------------------------------------------------------------------
                    //UNLOCKED
                    velocityX += Speed * Input.GetAxis("Mouse X") * 0.02f;
                    //------------------------------------------------------------------------------
                    
        }
        LastMousePos = MousePos;
        rotate();
        if (Input.GetMouseButton(0))
        {
            velocityY += Speed * Input.GetAxis("Mouse Y") * 0.02f;
        }
        }
        else
        {
            transform.position = defaultPos;
            transform.rotation = defaultRot;
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