using UnityEngine;

public class zenmai : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 axis = Vector3.up;
    [SerializeField]
    private float rate;
    [SerializeField]
    private AnimationCurve curve;
    [SerializeField] 
    private Transform CarTransform;
    private Transform child;
    private GameObject PartsPos;
    [SerializeField]
    private CarEntity car;
  
    private Rigidbody Rigidbody => car.Rigidbody;

    public interface IGameUIComponent
    {
        void Init(CarEntity entity);
    }


    private void Start()
    {
        curve.preWrapMode = WrapMode.Loop;
        curve.postWrapMode = WrapMode.Loop;
        CarTransform = transform;
        PartsPos = transform.Find("PartsPos").gameObject;
        child = PartsPos.transform.Find("MainSpringPos").gameObject.GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        
       if(Rigidbody.linearVelocity.magnitude * 3.6f > 1)
        {
            zenmai_rotate();
        }

        
    }
    public void zenmai_rotate()
    {
        child.transform.localRotation = Quaternion.AngleAxis(curve.Evaluate(Time.time * 1) * 360, axis);
    }
}
