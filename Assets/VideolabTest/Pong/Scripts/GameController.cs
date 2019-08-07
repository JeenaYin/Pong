using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController controller;
    //public TextMeshProUGUI Score;
    //public CanvasGroup RightWinCG;
    //public CanvasGroup LeftWinCG;
    public int leftScore = 0;
    public int rightScore = 0;
    public GameObject leftBat;
    public GameObject rightBat;
    public bool crazyMode = false;
    public readonly int crazyMaxBallNum = 8;
    public int curBallNum = 0;

    private GameObject[] Balls;
    public GameObject Ball0;
    public GameObject Ball1;
    public GameObject Ball2;
    public GameObject Ball3;
    public GameObject Ball4;
    public GameObject Ball5;
    public GameObject Ball6;
    public GameObject Ball7;

    private GameObject[] Bumpers;
    public GameObject Bumper0;
    public GameObject Bumper1;
    public GameObject Bumper2;
    public GameObject Bumper3;

    private Vector3[] BumperPos;
    private Vector3[] BumperVel;
    private int numBumper;
    private int maxBumper = 4;
    private float fadeTime = 0.3f;
    private int hitCounter1 = 0;
    private int hitCounter2 = 0;

    private Vector3 crazyLaunchLoc;
    private Vector3 crazyLaunchVel;
    //private int maxScore = 20;
    //private bool gameOver = false;

    private float smoothTime = 0.1f;

    private void Awake()
    {
        if (controller != null && controller != this)
            Destroy(this.gameObject);
        else
        {
            controller = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //LeftWinCG.alpha = 0;
        //RightWinCG.alpha = 0;
        Bumpers = new GameObject[maxBumper];
        BumperPos = new Vector3[maxBumper];
        BumperVel = new Vector3[maxBumper];
        Bumpers[0] = Bumper0;
        Bumpers[1] = Bumper1;
        Bumpers[2] = Bumper2;
        Bumpers[3] = Bumper3;
        for (int i = 0; i < Bumpers.Length; i++)
        {
            Bumpers[i].SetActive(false);
        }
        for (int i = 0; i < maxBumper; i++)
        {
            BumperPos[i] = new Vector3(Random.Range(-7f, 7f), Random.Range(-5f, 5f), 0);
        }
        Balls = new GameObject[crazyMaxBallNum];
        Balls[0] = Ball0;
        Balls[1] = Ball1;
        Balls[2] = Ball2;
        Balls[3] = Ball3;
        Balls[4] = Ball4;
        Balls[5] = Ball5;
        Balls[6] = Ball6;
        Balls[7] = Ball7;
        for (int i = 0; i < Balls.Length; i++)
        {
            Balls[i].SetActive(false);
        }
        SpawnFirstBall();
    }
    // Update is called once per frame
    void Update()
    {
        leftBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        rightBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        if (Input.GetKey(KeyCode.W))
        {
            leftBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, 8f, 0f);
        }
        if(Input.GetKey(KeyCode.S))
        {
            leftBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, -8f, 0f);
        }
        if(Input.GetKey(KeyCode.UpArrow))
        {
            rightBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, 8f, 0f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rightBat.GetComponent<Rigidbody>().velocity = new Vector3(0f, -8f, 0f);
        }

        for (int i = 0; i < maxBumper; i++)
            UpdateEachBumperPos(i);

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        //Score.text = leftScore.ToString() + " : " + rightScore.ToString();
        //curBallNum = NumBalls();
    }

    private void SpawnFirstBall()
    {
        Debug.Log("Spawn first ball");
        for (int i = 0; i < Balls.Length; i++)
        {
            Balls[i].SetActive(false);
        }
        Balls[0].SetActive(true);
        
        Balls[0].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        Balls[0].transform.position = new Vector3(0, 0, 0);
        Balls[0].GetComponent<BallController>().id = 0;
        Balls[0].GetComponent<BallController>().LaunchBall(1, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    }

    private void UpdateEachBumperPos(int idx)
    {
        if (Bumpers[idx] != null)
            Bumpers[idx].transform.position = Vector3.SmoothDamp(Bumpers[idx].transform.position, BumperPos[idx], ref BumperVel[idx], smoothTime);
    }

    private void SpawnNewBumper()
    {
        //if(!gameOver)
        //{
            Bumpers[numBumper].SetActive(true);
            Bumpers[numBumper].transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(-5f, 5f), 0);
            numBumper++;
        //}
    }

    public void BallCollision(Collision collision)
    {
        if (numBumper == 0)
        {
            hitCounter1++;
            if(hitCounter1 > 5)
            {
                int ran = Random.Range(0, 10);
                if (ran > 2)
                {
                    SpawnNewBumper();
                }
            }
        }
        if (numBumper > 0 && collision.gameObject.CompareTag("Bumper"))
        {
            if (numBumper < maxBumper)
            {
                hitCounter2++;
                if(numBumper == 1)
                {
                    SpawnNewBumper();
                }
                else
                {
                    int ran = Random.Range(0, 10);
                    if(numBumper > 2)
                    {
                        if (ran > 5)
                            SpawnNewBumper();
                    }
                    else if(ran > 4)
                    {
                        SpawnNewBumper();
                    }
                }
            }
            else if (numBumper == maxBumper)
            {
                curBallNum = NumBalls();
                int ran = Random.Range(0, 10);
                if (ran > 5  && curBallNum < 3)
                {
                    //Debug.Log("Trigger pattern");
                    for (int i = 0; i < maxBumper; i++)
                    {
                        BumperPos[i] = new Vector3(Random.Range(-7f, 7f), Random.Range(-5f, 5f), 0);
                    }
                }
                else
                {
                    //Debug.Log("Trigger effect");
                    if(!crazyMode)
                    {
                        StartCoroutine("EnterCrazyMode");
                    }
                }
            }
        }
    }

    IEnumerator EnterCrazyMode()
    {
        yield return new WaitForSeconds(0.05f);
        crazyLaunchVel = Balls[0].GetComponent<Rigidbody>().velocity;
        crazyLaunchLoc = Balls[0].GetComponent<Transform>().position;
        for (int i = 1; i < Balls.Length ; i++)
        {
            StartCoroutine("LaunchNewBall", (float)(i * 0.2f));
        }
        crazyMode = true;
    }

    public void BallMinusOne(int winner, int id)
    {
        Balls[id].SetActive(false);

        curBallNum = NumBalls();

        if (curBallNum == 0)
        {
            crazyMode = false;
            SpawnFirstBall();
        }
        //if (!gameOver)
        //{
            
        //    if (winner == 1)
        //        rightScore++;
        //    else
        //        leftScore++;
        //    if (leftScore == maxScore)
        //    {
        //        LeftWin();
        //    }
        //    if (rightScore == maxScore)
        //    {
        //        RightWin();
        //    }
        //}
    }

    IEnumerator LaunchNewBall (float waittime)
    {
        int idx = (int) (waittime / 0.2f);
        Debug.Log("Launch new ball " + idx);
        yield return new WaitForSeconds(waittime);
        Balls[idx].SetActive(true);
        Balls[idx].transform.position = crazyLaunchLoc;
        Balls[idx].GetComponent<BallController>().id = idx;
        Balls[idx].GetComponent<BallController>().LaunchBall(0, crazyLaunchLoc, crazyLaunchVel);
        Debug.Log("current ballnum " + curBallNum);
    }

    //void LeftWin()
    //{
    //    gameOver = true;
    //    StartCoroutine("FadeInCG", LeftWinCG);
    //    for(int i = 0; i < Balls.Length; i++)
    //    {
    //        Balls[i].SetActive(false);
    //    }
    //    for (int i = 0; i < Bumpers.Length; i++)
    //    {
    //        Bumpers[i].SetActive(false);
    //    }
    //}

    //void RightWin()
    //{
    //    gameOver = true;444444444444444444444444444444444444444444444444444444
    //    StartCoroutine("FadeInCG", RightWinCG);
    //    for (int i = 0; i < Balls.Length; i++)
    //    {
    //        Balls[i].SetActive(false);
    //    }
    //    for (int i = 0; i < Bumpers.Length; i++)
    //    {
    //        Bumpers[i].SetActive(false);
    //    }
    //}

    private int NumBalls()
    {
        int num = 0;
        foreach(GameObject ball in Balls)
        {
            if (ball.activeSelf)
                num++;
        }
        return num;
    }

    private IEnumerator FadeOutCG(CanvasGroup cg)
    {
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime / fadeTime;
            yield return new WaitForEndOfFrame();
        }

        cg.alpha = 0;
    }

    private IEnumerator FadeInCG(CanvasGroup cg)
    {
        while (cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime / fadeTime;
            yield return new WaitForEndOfFrame();
        }

        cg.alpha = 1;
    }
}
