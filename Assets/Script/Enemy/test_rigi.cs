using UnityEngine;
using UnityEngine.AI;

public class test_rigi : MonoBehaviour

{
    [SerializeField]
    private Transform[] Checkpoint;
    [SerializeField]
    EnemyController enemyController;
    [SerializeField]
    float speed;                //デバッグ用に前進させるときの速度

    [SerializeField]
    Vector3 Next_Corner;     //次のコーナー
    [SerializeField]
    private NavMeshAgent agent; //自分をNavMeshAgentに指定する
    [SerializeField]
    private Transform target;   //次の目的地
    [SerializeField]
    private Vector3 target_pos; //次の目的地の座標
    [SerializeField]
    bool NexCornerRight = false;//次のカーブが右方向か否か

    [SerializeField]
    float angle_below = 15;     //次の目的地に対してどれだけずらすかの最低値
    [SerializeField]
    float angle_above = 20;     //次の目的地に対してどれだけずらすかの最大値
    [SerializeField]
    float Inside_score = 100;   //どれだけインコースで走るか(デフォルト100)
    [SerializeField]
    float cpulevel = 0;
    float cpulevel_random = 0;
    [SerializeField]
    float Inside_score_Random = 0; //どれだけランダム性を持たせるか(+-n)
    float Inside_random = 0;
    [SerializeField]
    float angle_Random = 0;
    float Angle_Random = 0;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target_pos = target.transform.position;
        cpulevel_random = Random.Range(-cpulevel, cpulevel);
        Inside_random = cpulevel_random * 10;
        Angle_Random = Mathf.Abs(cpulevel_random);
    }
    private void FixedUpdate()
    {

    }
    private void Update()
    {
        //物理で動かすときは以下をFixedUpdate()に移動
        if (enemyController.canControl)
        {
            agent.SetDestination(target_pos);
            if (agent.path.corners.Length > 1)
            {
                Next_Corner = agent.path.corners[1];
                n_rotate();
            }
        }
        //前進するコードをここに
        //------------------------------
        //transform.position += transform.forward * speed * Time.deltaTime;
        //------------------------------
    }
    private void OnTriggerEnter(Collider other)
    {
        //次のカーブが右か左かを取得
        if (other.TryGetComponent<NextCorner>(out NextCorner component))
        {
            NextCornerData next = component.GetNextCorner;
            if (next == NextCornerData.Right)
            {
                NexCornerRight = true;
            }
            else if (next == NextCornerData.Left)
            {
                NexCornerRight = false;
            }
        }
        Arriving_Checkpoint(other);
        //UnityEngine.Debug.Log("ゴール");
    }
    void n_rotate()
    {
        string test = "";
        bool RightRotation = false;     //右旋回中か
        bool LeftRotation = false;      //左旋回中か
        float Distance = Vector3.Distance(transform.position, Next_Corner);

        float new_angle_below = (angle_below + Angle_Random) / (Distance / (Inside_score + Inside_random));
        float new_angle_above = (angle_above + Angle_Random) / (Distance / (Inside_score + Inside_random));
        //次が右カーブなら  コーナーが右側15～20度に来るように調整
        if (NexCornerRight)
        {
            test += "次のカーブは右：";

            //次のコーナーが左側にあるなら
            if (Angle_Judgment.Left_RightJudgment(gameObject, Next_Corner) == Direction.Left)
            {
                test += "コーナーが左:";
                LeftRotation = true;
            }
            //次のコーナーが右側にあるなら
            else if (Angle_Judgment.Left_RightJudgment(gameObject, Next_Corner) == Direction.Right)
            {
                test += "コーナーが右:";
                if (Angle_Judgment.Front_Judgment(gameObject, Next_Corner, new_angle_below))
                {
                    test += angle_below + "以下";
                    LeftRotation = true;
                }
                else if (!Angle_Judgment.Front_Judgment(gameObject, Next_Corner, new_angle_above))
                {
                    test += angle_above + "以上";
                    RightRotation = true;
                }
            }
        }
        else
        {
            test += "次のカーブは左：";
            //Debug.DrawLine(gameObject.transform.position, Next_Corner.transform.position);
            //Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + transform.forward*5);
            Debug.Log(Angle_Judgment.Left_RightJudgment(gameObject, Next_Corner));
            //次のコーナーが右側にあるなら
            if (Angle_Judgment.Left_RightJudgment(gameObject, Next_Corner) == Direction.Right)
            {
                test += "コーナーが右:";
                RightRotation = true;
            }
            //次のコーナーが左側にあるなら
            else if (Angle_Judgment.Left_RightJudgment(gameObject, Next_Corner) == Direction.Left)
            {
                test += "コーナーが左:";
                if (Angle_Judgment.Front_Judgment(gameObject, Next_Corner, new_angle_below))
                {
                    test += angle_below + "以下";
                    RightRotation = true;
                }
                else if (!Angle_Judgment.Front_Judgment(gameObject, Next_Corner, new_angle_above))
                {
                    test += angle_above + "以上";
                    LeftRotation = true;
                }
            }
        }
        //Debug.Log(test);

        //旋回する
        if (RightRotation)
        {
            //右に曲がるコードをここに
            //------------------------------
            gameObject.transform.Rotate(0, 0.4f, 0);
            //------------------------------
        }
        else if (LeftRotation)
        {
            //右に曲がるコードをここに
            //------------------------------
            gameObject.transform.Rotate(0, -0.4f, 0);
            //------------------------------
        }
    }

    void Arriving_Checkpoint(Collider other)
    {
        //Debug.Log("goal");
        if (other.transform == target)
        {
            //Debug.Log("next");
            for (int i = 0; i < Checkpoint.Length; i++)
            {
                if (Checkpoint[i] == other.transform)
                {
                    if (i >= Checkpoint.Length - 1)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                    target = Checkpoint[i];
                    target_pos = target.transform.position;
                    //Target_RandomNumber(target);
                    return;
                }
            }
        }
    }
}
