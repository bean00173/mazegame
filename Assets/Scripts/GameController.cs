using System;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;

    private MazeConstructor constructor;

    void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
    }

    void Start()
    {
        constructor.GenerateNewMaze(rows, cols);
    }
}