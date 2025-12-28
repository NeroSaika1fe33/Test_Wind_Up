using UnityEngine;

public class CarInput : CarComponent
{
    public bool movable = false;

    public float horizontal;
    public float vertical;

    //ステータス
    public float turnspeed = 2f;
    public float speed = 800f;
    public short Max_speed = 10;

    private Transform tf;
    private Animator anim;
    private Vector3 velocity;
    public void start()
    {
        movable = true;
    }
    float get_speed()
    {
        Vector3 currentVelocity = car.Rigidbody.linearVelocity;
        return currentVelocity.magnitude;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tf = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    { 
        float move = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        if (movable)
        {
            //前後移動
            if (get_speed() < Max_speed)
            {
                car.Rigidbody.AddForce(transform.forward * move * speed * Time.fixedDeltaTime);
            }
            //左右移動
            transform.Rotate(0, steer * turnspeed, 0);
        }
    }
}