using UnityEngine;

public class MazeDoor : MazePassage
{
    public Transform hinge;

    public float rotationSpeed;

    private Quaternion startRotation, endRotation;

    private float rotationProgress = -1;

    private static Quaternion
        normalRotation = Quaternion.Euler(0f, -90f, 0f),
        mirroredRotation = Quaternion.Euler(0f, 90f, 0f);

    private bool isMirrored;

    public override void Initialize(MazeCell primary, MazeCell other, MazeDirection direction)
    {
        base.Initialize(primary, other, direction);
        if (OtherSideOfDoor != null)
        {
            isMirrored = true;
            hinge.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 p = hinge.localPosition;
            p.x = -p.x;
            hinge.localPosition = p;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != hinge)
            {
                child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
            }
        }
    }

    private MazeDoor OtherSideOfDoor
    {
        get
        {
            return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
        }
    }

    public override void OnPlayerEntered()
    {
        if (hinge.localRotation == Quaternion.identity)
        {
            startRotation = hinge.localRotation;
            endRotation = isMirrored ? mirroredRotation : normalRotation;
            rotationProgress = 0;
        }
        else
        {
            OtherSideOfDoor.hinge.localRotation = hinge.localRotation = isMirrored ? mirroredRotation : normalRotation;
        }
        OtherSideOfDoor.cell.room.Show();
    }

    public override void OnPlayerExited()
    {
        //startRotation = hinge.localRotation;
        //endRotation = Quaternion.identity;
        //rotationProgress = 0;
        //OtherSideOfDoor.cell.room.Hide();
    }

    private void Update()
    {
        if (rotationProgress < 1 && rotationProgress >= 0)
        {
            rotationProgress += Time.deltaTime * rotationSpeed;
            OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
        }
    }
}
