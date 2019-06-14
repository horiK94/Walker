using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private void Awake()
    {
        MazeCreator mazeCreator = new MazeCreator();
        eStageType[,] maze = mazeCreator.CreateMaze(9, 9, new Vector2Int(0, 8), new Vector2Int(8, 0), 0.1f);
    }
}
