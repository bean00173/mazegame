using System;
using System.Collections.Generic;
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

    private int index;

    void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AiController>();
    }

    void Start()
    {
        constructor.GenerateNewMaze(rows, cols, OnTreasureTrigger);

        aIController.Graph = constructor.graph;
        aIController.Player = CreatePlayer();
        aIController.Monster = CreateMonster();
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

    private GameObject CreateMonster()
    {
        Vector3 monsterPosition = new Vector3(constructor.goalCol * constructor.hallWidth, 0f, constructor.goalRow * constructor.hallWidth);
        GameObject monster = Instantiate(monsterPrefab, monsterPosition, Quaternion.identity);
        monster.tag = "Generated";

        return monster;
    }

    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("You Won!");
        aIController.StopAI();
    }


    private void Update()
    {

        List<GameObject> spheres = new List<GameObject>();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(index == 0)
            {
                index += 1;
            }
            else
            {
                index = 0;

                foreach (GameObject go in spheres)
                {
                    Destroy(go);
                }
            }
            
            Transform treasure = GameObject.Find("Treasure").transform;
            Transform player = aIController.Player.transform;

            List<Node> path = aIController.FindPath((int)Mathf.Round((player.position.x)/aIController.HallWidth), (int)Mathf.Round((player.position.z)/aIController.HallWidth), (int)Mathf.Round((treasure.position.x) / aIController.HallWidth), (int)Mathf.Round((treasure.position.z) / aIController.HallWidth));

            Debug.Log("Finding Path");

            if(path!= null && path.Count > 1)
            {
                foreach (Node node in path)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    spheres.Add(sphere.gameObject);
                    sphere.transform.position = new Vector3(node.y * constructor.hallWidth, .5f, node.x * constructor.hallWidth);
                    sphere.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    Destroy(sphere.GetComponent<SphereCollider>());
                }
            }
        }
    }
}