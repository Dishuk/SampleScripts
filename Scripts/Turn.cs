using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    public static Turn instance;
    public Checker checker;
    public GridCell[,] grid;
    public List<GameObject> availableTurns;
    public GameObject availableTurnsGO;

    public bool attackersTurn;

    public List<GridCell> availableTurnsCoords;

    public Checker.ConflictSide currentTurn;

    public bool cameraDrag;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        grid = GameController.instance.grid;
        for (int i = 0; i < (grid.GetLength(0)-1) * 2 ; i++)
        {
            GameObject newGO = Instantiate(availableTurnsGO, Vector3.one, Quaternion.identity, transform);
            newGO.SetActive(false);
            availableTurns.Add(newGO);
        }
    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown (0) && cameraDrag == false)
        {
            Checker choosenChecker = ChooseChecker();

                if (choosenChecker != null)
                {
                    if (checker == choosenChecker)
                    {
                        checker = null;
                        ClearAvailalbeTurns();
                    }
                    else
                    {
                        StartTurn(choosenChecker);
                    }
                }
                else
                {
                    FinishTurn();
                }
        }*/
        

        if (Input.touchCount != 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended && cameraDrag == false)
            {
                Checker choosenChecker = ChooseChecker();

                if (choosenChecker != null)
                {
                    if (checker == choosenChecker)
                    {
                        checker = null;
                        ClearAvailalbeTurns();
                    }
                    else
                    {
                        StartTurn(choosenChecker);
                    }
                }
                else
                {
                    FinishTurn();
                }
            }
        }
    }

    Checker ChooseChecker()
    {
       int posX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x);
       int posY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y);

        //int posX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
        //int posY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        
        if (posX >= 0 && posX <= grid.GetLength(0) && posY >= 0 && posY <= grid.GetLength(1))
        {
            if (grid[posX, posY].isEmpty == false && grid[posX, posY].checker.conflictSide != currentTurn)
            {
                return null;
            }
            return grid[posX, posY].checker;
        }
        return null;
        

        /*
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (hit == true)
        {
            if (!hit.transform.gameObject.CompareTag("Checker") || hit.transform.GetComponent<Checker>().conflictSide != currentTurn)
            {
                return null;

            }
            return hit.transform.GetComponent<Checker>();
        }
        return null;*/
    }

    void StartTurn(Checker chooseChecker)
    {
        checker = chooseChecker;
        ClearAvailalbeTurns();
        ShowAvailableTurns();
    }

    void FinishTurn() {
        int posX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x);
        int posY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y);

        //int posX = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
        //int posY = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        if (posX >= 0 && posX <= grid.GetLength(0) && posY >= 0 && posY <= grid.GetLength(1))
        {
            if (grid[posX, posY].isAvailable == true)
            {
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isEmpty = true;
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].checker = null;

                checker.targetPosition = new Vector3(posX, posY, 0);
                StartCoroutine(checker.MoveChess());

                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isEmpty = false;
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].checker = checker.GetComponent<Checker>();

                if (currentTurn == Checker.ConflictSide.Attackers)
                {
                    currentTurn = Checker.ConflictSide.Deffenders;
                }
                else
                {
                    currentTurn = Checker.ConflictSide.Attackers;
                }

                CheckForFinishGame();
                CheckForDamage();
                ClearAvailalbeTurns();
                checker = null;
            }
        }

        /*

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
        if (hit == true)
        {
            if (hit.transform.gameObject.CompareTag("AvailableTurn"))
            {
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isEmpty = true;
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].checker = null;

                checker.targetPosition = hit.transform.position;
                StartCoroutine (checker.MoveChess());
               
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isEmpty = false;
                grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].checker = checker.GetComponent<Checker>();

                CheckForFinishGame();
                CheckForDamage();
                DeselectChecker();

                if (currentTurn == Checker.ConflictSide.Attackers)
                {
                    currentTurn = Checker.ConflictSide.Deffenders;
                }
                else {
                    currentTurn = Checker.ConflictSide.Attackers;
                }
            }
        }*/
    }


    void CheckForFinishGame() {
        if (checker.GetComponent<Checker>().isKing == true)
        {
            if (grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isExit == true)
            {
                Debug.Log("Game Finished");
            }
        }
    }

    void CheckForDamage()
    {
        int posX = Mathf.RoundToInt(checker.targetPosition.x);
        int posY = Mathf.RoundToInt(checker.targetPosition.y);

        if (posX + 2 < grid.GetLength(0))
        {
            if (grid[posX + 1, posY].isEmpty == false && grid[posX + 2, posY].isEmpty == false)
            {
                if (grid[posX + 1, posY].checker.conflictSide != grid[posX, posY].checker.conflictSide && grid[posX + 2, posY].checker.conflictSide == grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX + 1, posY].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX + 1, posY);
                    }
                    else
                    {
                        KillChecker(posX + 1, posY);
                    }
                }
                
            }
            else if (grid[posX + 1, posY].isEmpty == false && grid[posX + 2, posY].isCastle == true && grid[posX + 2, posY].isEmpty == true || grid[posX + 1, posY].isEmpty == false && grid[posX + 2, posY].isExit == true)
            {
                if (grid[posX + 1, posY].checker.conflictSide != grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX + 1, posY].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX + 1, posY);
                    }
                    else
                    {
                        KillChecker(posX + 1, posY);
                    }
                }
            }
        }

        if (posX - 2 >= 0)
        {
            if (grid[posX - 1, posY].isEmpty == false && grid[posX - 2, posY].isEmpty == false)
            {
                if (grid[posX - 1, posY].checker.conflictSide != grid[posX, posY].checker.conflictSide && grid[posX - 2, posY].checker.conflictSide == grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX - 1, posY].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX - 1, posY);
                    }
                    else
                    {
                        KillChecker(posX - 1, posY);
                    }
                }
                
            }
            else if (grid[posX - 1, posY].isEmpty == false && grid[posX - 2, posY].isCastle == true && grid[posX - 2, posY].isEmpty == true || grid[posX - 1, posY].isEmpty == false && grid[posX - 2, posY].isExit == true)
            {
                if (grid[posX - 1, posY].checker.conflictSide != grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX - 1, posY].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX - 1, posY);
                    }
                    else
                    {
                        KillChecker(posX - 1, posY);
                    }
                }
            }
        }

        if (posY + 2 < grid.GetLength(1))
        {
            if (grid[posX, posY + 1].isEmpty == false && grid[posX, posY + 2].isEmpty == false)
            {
                if (grid[posX, posY + 1].checker.conflictSide != grid[posX, posY].checker.conflictSide && grid[posX, posY + 2].checker.conflictSide == grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX , posY + 1].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX , posY + 1);
                    }
                    else
                    {
                        KillChecker(posX, posY + 1);
                    }
                }
            }
            else if (grid[posX, posY + 1].isEmpty == false && grid[posX, posY + 2].isCastle == true && grid[posX , posY+2].isEmpty == true || grid[posX, posY + 1].isEmpty == false && grid[posX, posY + 2].isExit == true)
            {
                if (grid[posX, posY + 1].checker.conflictSide != grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX, posY + 1].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX, posY + 1);
                    }
                    else
                    {
                        KillChecker(posX, posY + 1);
                    }
                }
            }
        }

        if (posY - 2 >= 0)
        {
            if (grid[posX, posY - 1].isEmpty == false && grid[posX, posY - 2].isEmpty == false)
            {
                if (grid[posX, posY - 1].checker.conflictSide != grid[posX, posY].checker.conflictSide && grid[posX, posY - 2].checker.conflictSide == grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX, posY - 1].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX , posY-1);
                    }
                    else
                    {
                        KillChecker(posX, posY - 1);
                    }
                }
            }
            else if (grid[posX, posY - 1].isEmpty == false && grid[posX, posY - 2].isCastle == true && grid[posX, posY - 2].isEmpty == true || grid[posX, posY - 1].isEmpty == false && grid[posX, posY - 2].isExit == true)
            {
                if (grid[posX, posY - 1].checker.conflictSide != grid[posX, posY].checker.conflictSide)
                {
                    if (grid[posX, posY - 1].checker.isKing == true)
                    {
                        CheckForKingDefeat(posX, posY - 1);
                    }
                    else
                    {
                        KillChecker(posX, posY - 1);
                    }
                }
            }
        }
    }

    void CheckForKingDefeat(int positionX, int positionY) {
        if (positionX + 1 > grid.GetLength(0) || grid[positionX+1, positionY].isEmpty == false && grid[positionX+1, positionY].checker.conflictSide == Checker.ConflictSide.Attackers || grid[positionX + 1, positionY].isCastle == true)
        {
            if (positionX - 1 < 0 ||  grid[positionX - 1, positionY].isEmpty == false && grid[positionX - 1, positionY].checker.conflictSide == Checker.ConflictSide.Attackers || grid[positionX - 1, positionY].isCastle == true)
            {
                if (positionY + 1 > grid.GetLength(1) ||  grid[positionX , positionY+1].isEmpty == false && grid[positionX , positionY + 1].checker.conflictSide == Checker.ConflictSide.Attackers ||  grid[positionX , positionY+1].isCastle == true)
                {
                    if (positionY - 1 < 0 ||  grid[positionX , positionY-1].isEmpty == false && grid[positionX , positionY-1].checker.conflictSide == Checker.ConflictSide.Attackers ||  grid[positionX, positionY-1].isCastle == true)
                    {
                        KillChecker(positionX, positionY);
                        Debug.Log("DEFEAT. Konung is dead...");
                    }
                }
            }
        }
    }

    void KillChecker(int positionX, int positionY ) {
        
        grid[positionX, positionY].checker.targetPosition = Vector3.zero;
        grid[positionX, positionY].checker.gameObject.SetActive(false);
        grid[positionX, positionY].checker = null;
        grid[positionX, positionY].isEmpty = true;
    }

    void ClearAvailalbeTurns() {
        for (int i = 0; i < availableTurnsCoords.Count; i++)
        {
            availableTurns[i].SetActive(false);
            grid[Mathf.RoundToInt( availableTurns[i].transform.position.x), Mathf.RoundToInt( availableTurns[i].transform.position.y)].isAvailable = false;
        }
        availableTurnsCoords.Clear();
    }

    void ShowAvailableTurns() {
        ClearAvailalbeTurns();
        //Debug.Log(checker.name + "     " + Mathf.RoundToInt(checker.targetPosition.x)+ "       " + Mathf.RoundToInt(checker.targetPosition.y));
        grid[Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)].isEmpty = false;

        for (int x = Mathf.RoundToInt(checker.targetPosition.x); x < grid.GetLength(0); x++)
        {
            if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isEmpty == false|| checker.isKing == false && grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isExit == true )
            {
                if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].coordinates == new Vector2(Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isCastle == true && checker.isKing == false)
                {
                    x++;
                }
                availableTurnsCoords.Add(grid[x, Mathf.RoundToInt(checker.targetPosition.y)]);
                grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isAvailable = true;
            }
        }

        for (int x = Mathf.RoundToInt(checker.targetPosition.x); x >= 0; x--)
        {
            if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isEmpty == false || checker.isKing == false && grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isExit == true ) 
            {
                if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].coordinates == new Vector2(Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isCastle == true && checker.isKing == false)
                {
                    x--;
                }
                availableTurnsCoords.Add(grid[x, Mathf.RoundToInt(checker.targetPosition.y)]);
                grid[x, Mathf.RoundToInt(checker.targetPosition.y)].isAvailable = true;
            }
        }

        for (int y = Mathf.RoundToInt(checker.targetPosition.y); y < grid.GetLength(1); y++)
        {
            if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].isEmpty == false || checker.isKing == false && grid[Mathf.RoundToInt(checker.targetPosition.x), y].isExit == true ) 
            {
                if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].coordinates == new Vector2(Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].isCastle == true && checker.isKing == false)
                {
                    y++;
                }
                availableTurnsCoords.Add(grid[Mathf.RoundToInt(checker.targetPosition.x), y]);
                grid[Mathf.RoundToInt(checker.targetPosition.x), y].isAvailable = true;
            }
        }

        for (int y = Mathf.RoundToInt(checker.targetPosition.y); y >= 0; y--)
        {
            if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].isEmpty == false || checker.isKing == false && grid[Mathf.RoundToInt(checker.targetPosition.x), y].isExit == true)
            {
                if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].coordinates == new Vector2(Mathf.RoundToInt(checker.targetPosition.x), Mathf.RoundToInt(checker.targetPosition.y)))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (grid[Mathf.RoundToInt(checker.targetPosition.x), y].isCastle == true && checker.isKing == false)
                {
                    y--;
                }
                availableTurnsCoords.Add(grid[Mathf.RoundToInt(checker.targetPosition.x), y]);
                grid[Mathf.RoundToInt(checker.targetPosition.x), y].isAvailable = true;
            }
        }

        if (availableTurnsCoords.Count != 0)
        {
            for (int i = 0; i < availableTurnsCoords.Count; i++)
            {
                availableTurns[i].transform.position = availableTurnsCoords[i].coordinates;
                availableTurns[i].SetActive(true);
            }
        }
        else {
            ClearAvailalbeTurns();
            checker = null;
        }
    }
}
