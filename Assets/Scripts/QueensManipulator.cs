using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueensManipulator : MonoBehaviour
{
    [SerializeField] private GeneticManager genetic;
    [SerializeField] private GameObject[] queens = new GameObject[8];
    [SerializeField] private GameObject[] boardColumns = new GameObject[8];
    
    public void UpdateQueensPositions() {
        int[] geneticTape = genetic.GetGeneticTape();
        for (int i = 0; i<8; i++)
        {
            NewQueenPosition(i, geneticTape[i]);
        }
    }

    
    void NewQueenPosition(int column, int line){
        Transform lineTransform = boardColumns[column].transform.GetChild(line);
        queens[column].transform.position = lineTransform.position;
    }

    public void OnUpdateQueens()
    {
        genetic.RandomizeGeneticTape();
        UpdateQueensPositions();
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