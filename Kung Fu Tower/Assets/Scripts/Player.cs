using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNumber;

    public Animator bodyAnimator;

    public ParticleSystem brainParticles;

    public float movementSpeed, rotationSpeed;

    public float health;

    private MazeCell currentCell;

    private MazeDirection currentDirection;

    private Vector3 startMovement, endMovement;

    private Quaternion startRotation, endRotation;

    private float movementProgress = -1, rotationProgress = -1;

    public AudioClip deathNoise;

    private void Start()
    {
        var brain = brainParticles.main;
        brain.startColor = Random.ColorHSV();
        foreach (PlayerAttack arm in gameObject.GetComponentsInChildren<PlayerAttack>()) arm.playerNumber = playerNumber;
    }

    public void DecrementHealth()
    {
        health = Mathf.Max(0, health - 1);
        gameObject.GetComponentInChildren<Animator>().SetTrigger("damagedTrigger");
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void SetLocation(MazeCell cell)
    {
        if (currentCell != null)
        {
            currentCell.OnPlayerExited();
        }
        currentCell = cell;
        transform.localPosition = cell.transform.localPosition;
        currentCell.OnPlayerEntered();
    }

    private void StartMoving(MazeDirection direction, string trigger)
    {
        MazeCellEdge edge = currentCell.GetEdge(direction);
        if (edge is MazePassage)
        {
            if (!(edge.otherCell.playerInCell || edge.otherCell.enemyInCell))
            {
                if (currentCell != null)
                {
                    currentCell.OnPlayerExited();
                }
                currentCell = edge.otherCell;

                startMovement = transform.localPosition;
                endMovement = edge.otherCell.transform.localPosition;
                movementProgress = 0f;
                currentCell.OnPlayerEntered();
            }
            else
            {
                bodyAnimator.SetTrigger(trigger);
                if (trigger.Equals("forwardBackTrigger"))
                {
                    if (edge.otherCell.playerInCell)
                    {
                        foreach (PlayerAttack arm in gameObject.GetComponentsInChildren<PlayerAttack>())
                        {
                            arm.GetArmAnimator().SetTrigger("avoidTrigger");
                        }
                    }
                    else if (edge.otherCell.enemyInCell)
                    {
                        string armString = "";
                        int whichArm = Random.Range(0, 2);
                        if (whichArm == 0) armString = "left";
                        else armString = "right";
                        foreach (PlayerAttack arm in gameObject.GetComponentsInChildren<PlayerAttack>())
                        {
                            if (arm.whichArm.Equals(armString) && !arm.otherArm.GetArmAnimator().GetBool("swipeTrigger")) arm.GetArmAnimator().SetTrigger("swipeTrigger");
                        }
                    }
                }
            }
        }
    }

    private void StartRotating(MazeDirection direction)
    {
        startRotation = transform.localRotation;
        endRotation = direction.ToRotation();
        rotationProgress = 0f;
        currentDirection = direction;
    }

    private void Update()
    {
        if (health <= 0)
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("deathTrigger");
            gameObject.GetComponent<AudioSource>().clip = deathNoise;
            if (!gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Play();
        }

        if (movementProgress >= 1 || movementProgress < 0)
        {
            if (playerNumber == 1)
            {
                bool playerInNextCell = false;
                bool enemyInNextCell = false;
                if (currentCell.GetEdge(currentDirection).otherCell != null)
                {
                    playerInNextCell = currentCell.GetEdge(currentDirection).otherCell.playerInCell;
                    enemyInNextCell = currentCell.GetEdge(currentDirection).otherCell.playerInCell;
                }
                if (Input.GetKey(KeyCode.W) || ((playerInNextCell || enemyInNextCell) && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))))
                {
                    StartMoving(currentDirection, "forwardBackTrigger");
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    StartMoving(currentDirection.GetOpposite(), "backForwardTrigger");
                }
            }
            else if (playerNumber == 2)
            {
                bool playerInNextCell = false;
                bool enemyInNextCell = false;
                if (currentCell.GetEdge(currentDirection).otherCell != null)
                {
                    playerInNextCell = currentCell.GetEdge(currentDirection).otherCell.playerInCell;
                    enemyInNextCell = currentCell.GetEdge(currentDirection).otherCell.playerInCell;
                }
                if (Input.GetKey(KeyCode.UpArrow) || ((playerInNextCell || enemyInNextCell) && (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.PageUp))))
                {
                    StartMoving(currentDirection, "forwardBackTrigger");
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    StartMoving(currentDirection.GetOpposite(), "bacForwardTrigger");
                }
            }
        }
        else
        {
            movementProgress += Time.deltaTime * movementSpeed;
            transform.localPosition = Vector3.Lerp(startMovement, endMovement, movementProgress);
        }

        if (rotationProgress >= 1 || rotationProgress < 0)
        {
            if (playerNumber == 1)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    StartRotating(currentDirection.GetNextCounterclockwise());
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    StartRotating(currentDirection.GetNextClockwise());
                }
            }
            else if (playerNumber == 2)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    StartRotating(currentDirection.GetNextCounterclockwise());
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    StartRotating(currentDirection.GetNextClockwise());
                }
            }
        }
        else
        {
            rotationProgress += Time.deltaTime * rotationSpeed;
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
        }
    }

    public MazeCell GetCurrentCell()
    {
        return currentCell;
    }
}
