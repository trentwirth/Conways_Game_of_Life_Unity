using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControlGameBoard : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int boardSize = 10;
    [SerializeField] private float cellSpacing = 1.1f;
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Image playPauseImage;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private CellRules[,] cells;
    private bool isPlaying = false;

    private void Start()
    {
        cells = new CellRules[boardSize, boardSize];
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                Vector3 cellPosition = new Vector3(i * cellSpacing, j * cellSpacing, 0);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cells[i, j] = cell.GetComponent<CellRules>();
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                CellRules cell = hit.collider.GetComponent<CellRules>();
                if (cell != null)
                {
                    cell.ToggleState();
                }
            }
        }

        if (isPlaying)
        {
            updateInterval -= Time.deltaTime;
            if (updateInterval <= 0)
            {
                UpdateBoard();
                updateInterval = 0.5f;
            }
        }
    }

    public void TogglePlayPause()
    {
        isPlaying = !isPlaying;
        playPauseImage.sprite = isPlaying ? pauseSprite : playSprite;
        Debug.Log("Play: " + isPlaying);
    }

    private void UpdateBoard()
    {
        bool[,] newStates = new bool[boardSize, boardSize];

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                int aliveNeighbors = CountAliveNeighbors(i, j);
                bool currentState = cells[i, j].IsAlive;
                bool newState = false;

                if (currentState && (aliveNeighbors == 2 || aliveNeighbors == 3))
                {
                    newState = true;

                }
                else if (!currentState && aliveNeighbors == 3)
                {
                    newState = true;

                    // Find two parent cells for color inheritance.
                    List<CellRules> aliveNeighborCells = new List<CellRules>();
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;

                            int newX = i + x;
                            int newY = j + y;

                            if (newX >= 0 && newX < boardSize && newY >= 0 && newY < boardSize && cells[newX, newY].IsAlive)
                            {
                                aliveNeighborCells.Add(cells[newX, newY]);
                            }
                        }
                    }

                    // Choose two random parent cells for color inheritance.
                    if (aliveNeighborCells.Count >= 2)
                    {
                        CellRules parentCell1 = aliveNeighborCells[Random.Range(0, aliveNeighborCells.Count)];
                        CellRules parentCell2;
                        do
                        {
                            parentCell2 = aliveNeighborCells[Random.Range(0, aliveNeighborCells.Count)];
                        } while (parentCell1 == parentCell2);

                        cells[i, j].InheritColor(parentCell1.ColorInheritance, parentCell2.ColorInheritance);
                    }
                }

                newStates[i, j] = newState;
            }
        }

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                cells[i, j].SetState(newStates[i, j]);
            }
        }
    }

private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int newX = x + i;
                int newY = y + j;

                if (newX >= 0 && newX < boardSize && newY >= 0 && newY < boardSize && cells[newX, newY].IsAlive)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Connect to UI button.
    public void KillAllCells()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                cells[i, j].SetState(false);
            }
        }
    }

}

