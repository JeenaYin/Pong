using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody rb;
    public int id;
    private bool launched;
    float randomRange = 1.5f;
    Vector3 dampGoal;
    Vector3 placeHolder;

    Vector3 crazyLaunchVel;
    Vector3 crazyLaunchLoc;

    public void LaunchBall(float waittime, Vector3 loc, Vector3 vel)
    {
        launched = false;
        rb = GetComponent<Rigidbody>();
        crazyLaunchLoc = loc;
        crazyLaunchVel = vel;
        StartCoroutine("Launch", waittime);
    }

    // Update is called once per frame
    void Update()
    {
        if (launched)
            rb.velocity = SlowDown(rb.velocity);
        if (Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 7f)
        {
            if(transform.position.x < 0)
                GameController.controller.BallMinusOne(1, id);
            else
                GameController.controller.BallMinusOne(0, id);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Vector3 newVector = new Vector3();
        newVector = rb.velocity;

        if (collision.gameObject.CompareTag("Bats") 
            || collision.gameObject.CompareTag("Bumper") 
            || collision.gameObject.CompareTag("Boundaries"))
        {
            if(collision.relativeVelocity.sqrMagnitude < 30f)
            {
                newVector = Vector3.Scale(rb.velocity, new Vector3(2.2f, 2.2f, 0f));
            }
            else if (collision.relativeVelocity.sqrMagnitude < 40f)
            {
                newVector = Vector3.Scale(rb.velocity, new Vector3(2f, 2f, 0f));

            } else if(collision.relativeVelocity.sqrMagnitude < 60f)
            {
                newVector = Vector3.Scale(rb.velocity, new Vector3(1.7f, 1.7f, 0f));
            }
            else if (collision.relativeVelocity.sqrMagnitude < 80f)
            {
                newVector = Vector3.Scale(rb.velocity, new Vector3(1.5f, 1.5f, 0f));
            }
        }
        rb.velocity = Amplify(newVector);
        dampGoal = Vector3.Scale(rb.velocity, new Vector3(0.7f, 0.7f, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameController.controller.BallCollision(collision);
    }
    
    IEnumerator Launch(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        launched = true;

        if(crazyLaunchVel.magnitude != 0)
        {
            rb.velocity = crazyLaunchVel;
        }
        else
        {
            int x = Random.Range(0, 2);
            int y = Random.Range(0, 3);
            Vector3 launchDir = new Vector3
            {
                x = x == 0 ? 7f : -7f,
                y = y == 0 ? 7f : -7f
            };
            launchDir.y = y == 2 ? 0f : launchDir.y;
            launchDir.x = y == 2 ? (launchDir.x * 1.414f) : launchDir.x;
            rb.velocity = launchDir;
        }
        dampGoal = Vector3.Scale(rb.velocity, new Vector3(1f, 1f, 0));
    }

    Vector3 Amplify(Vector3 original)
    {
        Vector3 newVector = original;
        float absX = Mathf.Abs(original.x);
        float absY = Mathf.Abs(original.y);

        if (absX < 2.7f)
        {
            if (absX < 0.5f)
            {
                newVector = new Vector3(Random.Range(-randomRange, randomRange), original.y, 0f);
            }
            else
                newVector = Vector3.Scale(original, new Vector3(1.5f, 1f, 0f));
        }

        if (absY < 1f)
        {
            if (absY < 0.5f)
            {
                newVector = new Vector3(original.x, Random.Range(-randomRange, randomRange), 0f);
            }
            else 
                newVector = Vector3.Scale(original, new Vector3(1.5f, 2f, 0f));
        }
        newVector = ClampMag(newVector, 12, 3);
        return newVector;
    }

    Vector3 SlowDown(Vector3 original)
    {
        Vector3 newVector = original;
        newVector = Vector3.SmoothDamp(newVector, dampGoal, ref placeHolder, 0.8f);
        newVector = Vector3.ClampMagnitude(newVector, 25);
        return newVector;
    }

    public static Vector3 ClampMag(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max) return v.normalized * max;
        else if (sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
}
