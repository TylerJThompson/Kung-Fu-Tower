using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public IntVector2 coordinates;

    public MazeRoom room;

    public bool playerInCell = false, enemyInCell = false;

    public Enemy cellEnemy = null;

    private int initializedEdgeCount;

    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    public void Initialize(MazeRoom room)
    {
        room.Add(this);
        transform.GetChild(0).GetComponent<Renderer>().material = room.settings.floorMaterial;
    }

    public bool IsFullyInitialized
    {
        get
        {
            return initializedEdgeCount == MazeDirections.Count;
        }
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                    {
                        return (MazeDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    public void OnPlayerEntered()
    {
        playerInCell = true;
        room.Show();
        for (int i = 0; i < edges.Length; i++)
        {
            edges[i].OnPlayerEntered();
        }
    }

    public void OnPlayerExited()
    {
        playerInCell = false;
        room.Hide();
        for (int i = 0; i < edges.Length; i++)
        {
            edges[i].OnPlayerExited();
        }
    }

    public void OnEnemyEntered(Enemy enemy)
    {
        enemyInCell = true;
        cellEnemy = enemy;
    }

    public void OnEnemyExited()
    {
        enemyInCell = false;
        cellEnemy = null;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (cellEnemy != null) cellEnemy.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (cellEnemy != null) cellEnemy.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
