using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueensManipulator : MonoBehaviour
{
  [SerializeField] private GameObject[] queens = new GameObject[8];
  [SerializeField] private GameObject[] boardColumns = new GameObject[8];

  public void UpdateQueensPositions(BoardSetting gene)
  {
    int[] geneticTape = gene.GetGeneticIntTape();
    for (int i = 0; i < 8; i++)
    {
      NewQueenPosition(i, geneticTape[i]);
    }
  }


  void NewQueenPosition(int column, int line)
  {
    Transform lineTransform = boardColumns[column].transform.GetChild(line);
    queens[column].transform.position = lineTransform.position;
  }

  public void OnUpdateQueens(BoardSetting gene)
  {
    UpdateQueensPositions(gene);
  }

  public GameObject[] GetQueens()
  {
    return queens;
  }

  public GameObject[] GetBoard()
  {
    return boardColumns;
  }
}