using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    public Pooler bulletPool;

    private MazeCell currentCell;

    private MazeDirection currentDirection;

    private Player lockedOn = null;

    private Vector3 startMovement, endMovement;

    private Quaternion startRotation, endRotation;

    private float movementProgress = - 1, rotationProgress = -1;

    public GameObject eye;

    public float idleSpeed;

    private float lastShot = 0f;

    public float timeBetweenShots;

    public GameObject gunBarrel;

    public AudioClip deathNoise;

    public void SetLocation(MazeCell cell)
    {
        if (currentCell != null)
        {
            currentCell.OnEnemyExited();
        }
        currentCell = cell;
        transform.localPosition = cell.transform.localPosition;
        currentCell.OnEnemyEntered(this);
    }

    private void StartMoving(MazeDirection direction)
    {
        MazeCellEdge edge = currentCell.GetEdge(direction);
        if (edge is MazePassage)
        {
            if (!(edge.otherCell.playerInCell || edge.otherCell.enemyInCell))
            {
                if (currentCell != null)
                {
                    currentCell.OnEnemyExited();
                }
                currentCell = edge.otherCell;

                startMovement = transform.localPosition;
                endMovement = edge.otherCell.transform.localPosition;
                movementProgress = 0f;
                currentCell.OnEnemyEntered(this);
            }
        }
    }

    private void IdleTurn(MazeDirection direction)
    {
        startRotation = transform.localRotation;
        endRotation = direction.ToRotation();
        rotationProgress = 0f;
        currentDirection = direction;
    }

    public void DecrementHealth()
    {
        health--;
        lastShot = Time.fixedTime;
        gameObject.GetComponentInChildren<Animator>().SetTrigger("damagedTrigger");
        if (health > 0) gameObject.GetComponent<AudioSource>().Play();
        else
        {
            gameObject.GetComponent<AudioSource>().clip = deathNoise;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    public MazeCell GetCurrentCell()
    {
        return currentCell;
    }

    private void Update()
    {
        if (health <= 0)
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("deathTrigger");
        }

        if ((movementProgress >= 1 || movementProgress < 0) && lockedOn != null)
        {
            int dir = Random.Range(0, 2);
            if (dir == 0)
            {
                if (lockedOn.transform.position.x - transform.position.x > 2) StartMoving(MazeDirection.East);
                else if (transform.position.x - lockedOn.transform.position.x > 2) StartMoving(MazeDirection.West);
            }
            else
            {
                if (lockedOn.transform.position.z - transform.position.z > 2) StartMoving(MazeDirection.North);
                else if (transform.position.z - lockedOn.transform.position.z > 2) StartMoving(MazeDirection.South);
            }
        }
        else if (movementProgress != -1)
        {
            movementProgress += Time.deltaTime * idleSpeed;
            transform.localPosition = Vector3.Lerp(startMovement, endMovement, movementProgress);
        }

        RaycastHit hit;
        Ray ray = new Ray(eye.transform.position, transform.rotation * Vector3.forward);
        if (Physics.Raycast(ray, out hit))
        {
            Player hitObject = hit.transform.gameObject.GetComponentInParent<Player>();
            if (hitObject != null)
            {
                lockedOn = hitObject;
                rotationProgress = -1;
            }
            else
            {
                lockedOn = null;
                if ((rotationProgress >= 1 || rotationProgress < 0) && transform.localRotation != currentDirection.ToRotation()) IdleTurn(currentDirection.GetNextClockwise());
            }
        }

        if (lockedOn == null)
        {
            if ((rotationProgress >= 1 || rotationProgress < 0) && lockedOn == null)
            {
                IdleTurn(currentDirection.GetNextClockwise());
            }
            else
            {
                rotationProgress += Time.deltaTime * idleSpeed;
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
            }
        }
        else
        {
            transform.LookAt(lockedOn.transform);
            if (transform.localRotation == MazeDirection.East.ToRotation()) currentDirection = MazeDirection.East;
            else if (transform.localRotation == MazeDirection.North.ToRotation()) currentDirection = MazeDirection.North;
            else if (transform.localRotation == MazeDirection.South.ToRotation()) currentDirection = MazeDirection.South;
            else if (transform.localRotation == MazeDirection.West.ToRotation()) currentDirection = MazeDirection.West;
            if (Time.fixedTime - lastShot >= timeBetweenShots)
            {
                GameObject bullet = bulletPool.getPooledObject();
                if (bullet != null)
                {
                    bullet.transform.position = gunBarrel.transform.position;
                    bullet.transform.rotation = gunBarrel.transform.rotation;
                    bullet.gameObject.SetActive(true);
                    lastShot = Time.fixedTime;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player")) DecrementHealth();
    }
}
