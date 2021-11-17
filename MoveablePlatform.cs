using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For 3D Games use this
//[RequireComponent(typeof(BoxCollider))]

//For 2D Games use this
[RequireComponent(typeof(BoxCollider2D))]
public class MoveablePlatform : MonoBehaviour
{
    //The number of points within the path the platform can move in; these points will change the direction the platform moves in based on the next point it needs to move to
    public List<Vector3> numberOfPoints;
    //The different types of movement the platform can move around the paths from the PathEditor script
    [SerializeField]
    private enum MovementType { Ascending, PingPong, StopOnEnd }
    [SerializeField]
    private MovementType movementType;
    //How fast the platform should be moving
    [SerializeField]
    protected float speed;
    //Determines if the platform has reached the end of its current point and needs to keep moving to the next
    private bool needsToFindNextPoint = true;
    //Determines if the platform is currently moving, meaning it hasn't reached the destination of a point
    private bool moving;
    //Determines if the platform is a PingPong type, if it is going backwards in the iteration now
    private bool pingPongGoingDown;
    //The point in the path the platform is on; by default all paths should start at numberOfPahts iteration value 0, so we hardcode this value here
    private int currentPoint = 0;
    //Finds out based on the currentPoint value what the next point in the path should be
    private int nextPoint;

    public bool placePlatformOnFirstPoint;


    private void Start()
    {
        transform.position = numberOfPoints[currentPoint];
    }

    private void FixedUpdate()
    {
        FindThePoint();
        MoveToPosition();

    }
    protected virtual void FindThePoint()
    {
        //A different for loop that we usually write, this for loop starts the iteration at the currentPoint value; typically we use for loops to start at 0
        for (int i = currentPoint; i < numberOfPoints.Count; i++)
        {
            //If the platform is at the end of a path, it checks to see if the needsToMove bool is true still
            if (needsToFindNextPoint)
            {
                //If needsToMove is still true, we quickly set it to false so we can calculate the next path the platform needs to move in
                needsToFindNextPoint = false;
                //Changes the currentPoint iteration value to the most appropriate value based on what platformType it is
                if (!pingPongGoingDown)
                {
                    currentPoint = i;
                }
                else
                {
                    currentPoint = i - 2;
                }
                //If the platform is Ascending, it will constantly move to the next path ahead of it, and then reset back to the begining once it reaches the end
                if (movementType == MovementType.Ascending)
                {
                    nextPoint = i + 1;
                    //This logic helps ensure the platform goes right back to the begining again in the most direct path if it reaches the end of the numberOfPaths list
                    if (nextPoint == numberOfPoints.Count)
                    {
                        nextPoint = 0;
                    }
                }
                //If the platform is PingPong, once it reaches the end it will go in reverse and pass through each point in the numberOfPaths list
                if (movementType == MovementType.PingPong)
                {
                    //If the path is still going to the final iteration in the numberOfPaths list, the i value for the nextPath will be added
                    if (!pingPongGoingDown)
                    {
                        nextPoint = i + 1;
                        //If the path reaches the end, we set the next path to the previous point by subtracting it by 2, and then set the pingPongGoingDown bool to true
                        if (nextPoint == numberOfPoints.Count)
                        {
                            nextPoint = i - 2;
                            currentPoint--;
                            pingPongGoingDown = true;
                        }
                    }
                    //If the pingPongGoingDown bool is true, rather than adding one to the i value at the end of each point, it subtracts one
                    if (pingPongGoingDown)
                    {
                        nextPoint = i - 1;
                        //If it gets back to the begining of the numberOfPaths list, it sets everything up so the platform will start going forward through that list again
                        if (nextPoint == 0)
                        {
                            nextPoint = 0;
                            currentPoint = -1;
                            pingPongGoingDown = false;
                        }
                    }
                }
                //If the platform is simply supposed to stop at the end of the path, it doesn't iterate anymore and stops once it reaches the end of the numberOfPaths list
                if (movementType == MovementType.StopOnEnd)
                {
                    nextPoint = i + 1;
                    if (nextPoint == numberOfPoints.Count)
                    {
                        return;
                    }
                }
                //Makes sure the platform is constantly moving as long as needsToMove is true
                moving = true;
            }
        }
    }

    //This method will manage propper movement between the different paths for all three different types, it uses the nextPath value to calculate where it should move the platform towards next
    protected virtual void MoveToPosition()
    {
        if (moving)
        {
            if (transform.position == numberOfPoints[nextPoint])
            {
                moving = false;
                needsToFindNextPoint = true;
                currentPoint++;
            }
            if (transform.position == numberOfPoints[nextPoint] && currentPoint == numberOfPoints.Count)
            {
                currentPoint = 0;
            }
            transform.position = Vector3.MoveTowards(transform.position, numberOfPoints[nextPoint], speed * Time.deltaTime);
        }
    }

    //Use this for a 2D game
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.GetComponent<Collider2D>().bounds.min.y > GetComponent<Collider2D>().bounds.center.y)
        {
            collision.transform.parent = transform;
        }
    }


    //Use this for a 2D game
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.parent = null;
        }
    }

    //Use this for a 3D game
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.GetComponent<Collider>().bounds.min.y > GetComponent<Collider>().bounds.center.y)
        {
            collision.transform.parent = transform;
        }
    }
    

    //Use this for a 3D game
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.transform.parent = null;
        }
    }
    */


    protected virtual void OnDrawGizmos()
    {
        if(placePlatformOnFirstPoint && !Application.isPlaying)
        {
            transform.position = numberOfPoints[0];
        }
    }
}