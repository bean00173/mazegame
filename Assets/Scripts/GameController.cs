using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;

    public GameObject playerPrefab;
    public GameObject monsterPrefab;

    private MazeConstructor constructor;

    private AiController aIController;

    public GameObject sphereParent;

    private bool con = true;

    void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AiController>();
    }

    void Start()
    {
        NewMaze();
    }
    
    // Moved all Start Methods into separate, Now can be called multiple times, makes restarting on death easier.

    private void NewMaze()
    {
        constructor.GenerateNewMaze(rows, cols, OnTreasureTrigger);

        aIController.Graph = constructor.graph;
        aIController.Player = CreatePlayer();
        aIController.Monster = CreateMonster(OnMonsterTrigger); // Adds in Monster Trigger Function as Param for Creating Monster
        aIController.HallWidth = constructor.hallWidth;
        aIController.StartAI();
    }

    private GameObject CreatePlayer()
    {
        Vector3 playerStartPosition = new Vector3(constructor.hallWidth, 1, constructor.hallWidth);
        GameObject player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
        player.tag = "Generated";

        return player;
    }

    private GameObject CreateMonster(TriggerEventHandler monsterCallback) // Added in Callback Params for Creating Monster
    {
        Vector3 monsterPosition = new Vector3(constructor.goalCol * constructor.hallWidth, 0f, constructor.goalRow * constructor.hallWidth);
        GameObject monster = Instantiate(monsterPrefab, monsterPosition, Quaternion.identity);
        monster.tag = "Generated";

        TriggerEventRouter tc = monster.AddComponent<TriggerEventRouter>(); // Gives Monster New Component TriggerEventRouter
        tc.callback = monsterCallback;

        return monster;
    }

    private void OnMonsterTrigger(GameObject trigger, GameObject other) // A New Trigger Function for when the Monster Hits you, Destroys Guide Path, Maze and Makes New Maze
    {
        Debug.Log("Gotcha!");

        ClearGuidePath();
        constructor.DisposeOldMaze();

        NewMaze();
    }

    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("You Won!");

        ClearGuidePath();

        aIController.StopAI();
    }


    private void Update()
    {
        // Within this update is most of the method to generate a path of spheres

        List<GameObject> spheres = new List<GameObject>();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(con) 
            {

                Transform treasure = GameObject.Find("Treasure").transform;
                Transform player = aIController.Player.transform;

                // For some reason, when finding a path using the Node Positions, you have to flip the x and y (z),

                List<Node> path = aIController.FindPath((int)Mathf.Round((player.position.z) / aIController.HallWidth), (int)Mathf.Round((player.position.x) / aIController.HallWidth), (int)Mathf.Round((treasure.position.x) / aIController.HallWidth), (int)Mathf.Round((treasure.position.z) / aIController.HallWidth));

                Debug.Log("Finding Path");

                if (path != null && path.Count > 1)
                {
                    foreach (Node node in path) // Cycle through new path and place yellow sphere on every node in path
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = new Vector3(node.y * constructor.hallWidth, .5f, node.x * constructor.hallWidth);
                        sphere.GetComponent<MeshRenderer>().material.color = Color.yellow;

                        sphere.tag = "Waypoint";

                        Destroy(sphere.GetComponent<SphereCollider>());
                    }
                }
                
                con = false;
            }
            else // if spheres are already visible, delete them
            {
                ClearGuidePath();
            }
            
            
        }

    }

    private void ClearGuidePath() // Function to Get Rid of Path
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject point in points)
        {
            Destroy(point);
        }

        con = true;
    }
}