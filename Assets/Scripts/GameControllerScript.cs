﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour 
{
	private float screenBoundariesX = 8.5f, screenBoundariesY = 4.5f; //Defines the Boundary to be used on all scripts.
    public float ScreenBoundariesX //Makes the values accesible to other scripts.
    {
        get
        {
            return screenBoundariesX;
        }
        set
        {
            //screenBoundariesX = value; //Disabled because we need to protect the screen boundaries, making it read-only.
        }
    }
    public float ScreenBoundariesY //Makes the values accesible to other scripts.
    {
        get
        {
            return screenBoundariesY;
        }
        set
        {
            //screenBoundariesY = value; //Disabled because we need to protect the screen boundaries, making it read-only.
        }
    }
	private int[] playersScores;
	public int[] PlayersScores
	{
		get
		{
			return playersScores;
		}
		set
		{
			playersScores = value;
		}
	}
	private int score;
	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}
	private bool gameOver = false;
	public bool GameOver
	{
		get
		{
			return gameOver;
		}
	}
	public GameObject[] players;
	public GameObject[] Players
	{
		get
		{
			return players;
		}
		set
		{
			players = value;
		}
	}
    public Transform bossPosition;
	private float[] playersHealth;
	public float[] PlayersHealth
	{
		get
		{
			return playersHealth;
		}
		set
		{
			playersHealth = value;
		}
	}
    private float bossHealth;
    public float BossHealth
    {
        get
        {
            return bossHealth;
        }
        set
        {
            bossHealth = value;
        }
    }


    public float defaultBGScrollSpeed = -1.0f;
    public float defaultFGScrollSpeed = -3.0f;
    public float bgScrollSpeed;
    public float fgScrollSpeed;
  
	public GameObject[] FindAllPlayers()
	{
		PlayerScript[] playerScripts = GameObject.FindObjectsOfType<PlayerScript> ();
		if (playerScripts.Length == 0)
		{
			return null;
		}
		GameObject[] players = new GameObject[playerScripts.Length];
		int i;
		for (i = 0; i < playerScripts.Length; i++) 
		{
			players [i] = playerScripts [i].gameObject;
		}
		return players;
	}
	public GameObject FindOneRandomPlayer()
	{
		PlayerScript[] players = GameObject.FindObjectsOfType<PlayerScript> ();
		try
		{
			int pos = Random.Range (0, players.Length);
			return players[pos].gameObject;
		}
		catch 
		{
			return null;
		}
	}
    public List<GameObject> disabledPlayers = new List<GameObject>();

	public GameObject uiGameOverGameObject;
	public GameObject uiPlayer1HPGameObject;
	public GameObject uiPlayer2HPGameObject;
	public GameObject uiScoreGameObject;
	public GameObject pSystem;
    public GameObject debugUIFPS;
    public GameObject debugUICheckpoint;

    public List<GameObject> enemies = new List<GameObject>();
    public GameObject[] buffPickups;
    public GameObject[] debuffPickups;

    private Checkpoint checkpoint;
    private int checkpointNumber = 0;

    public void AddNewCheckpoint()
    {
        checkpoint.AddCheckpoint(bossPosition.position, score, checkpointNumber);
        checkpointNumber++;
    }

    public string GetCheckpoints()
    {
        return checkpoint.GetCheckpointToString();
    }
    
    public void ResumeFromLastCheckpoint()
    {
        bossPosition.position = checkpoint.GetLastCheckpointBossPosition();
        score = checkpoint.GetLastCheckpointScore();
        for (int i = 0; i < disabledPlayers.Count; i++)
        {
            disabledPlayers[i].GetComponent<PlayerScript>().Respawn();
        }
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        uiPlayer1HPGameObject.gameObject.SetActive(true);
        uiPlayer2HPGameObject.gameObject.SetActive(true);
        uiScoreGameObject.gameObject.SetActive(true);
        uiGameOverGameObject.gameObject.SetActive(false);
    }

    public void AddEnemyToEnemyList(GameObject enemy)
    {
        enemies.Add(enemy);
    }

	void Awake()
	{
        bgScrollSpeed = defaultBGScrollSpeed;
        fgScrollSpeed = defaultFGScrollSpeed;
        if (PlayerPrefs.GetInt("gameDebugUI_FPS") == 0)
        {
            debugUIFPS.SetActive(false);
        }
        else
        {
            debugUIFPS.SetActive(true);
        }
        if (PlayerPrefs.GetInt("gameDebugUI_Checkpoint") == 0)
        {
            debugUICheckpoint.SetActive(false);
        }
        else
        {
            debugUICheckpoint.SetActive(true);
        }
        checkpoint = new Checkpoint();
        players = FindAllPlayers();
        if (FindAllPlayers() != null)
        {
            playersHealth = new float[FindAllPlayers().Length];
        }
	}
	// Use this for initialization
	void Start () 
	{

	}
	void GameOverMethod()
	{
		if (gameOver == false) 
		{
			gameOver = true;
			uiPlayer1HPGameObject.gameObject.SetActive(false);
			uiPlayer2HPGameObject.gameObject.SetActive(false);
			uiScoreGameObject.gameObject.SetActive(false);
			uiGameOverGameObject.gameObject.SetActive(true);
			//pSystem.GetComponent<ParticleSystem>().Stop();
			//Implement Game Over Code Here;
			//Debug.Log ("Game Over! Returning to Main Menu in 5 seconds!");
			//StartCoroutine(WaitForNSecondsAndReturnToMainMenu(5));
		}
	}
	IEnumerator WaitForNSecondsAndReturnToMainMenu(int n)
	{
		 yield return new WaitForSeconds(n);
		 SceneManager.LoadScene("MainMenu");
	}
	// Update is called once per frame
	void Update () 
	{
        enemies.Remove(null);
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide) || Input.GetKeyDown(KeyCode.F3) || Input.GetButtonDown("MENU") && Input.GetButtonDown("GREEN0"))
        {
            if (debugUIFPS.activeSelf == true)
            {
                debugUIFPS.SetActive(false);
                PlayerPrefs.SetInt("gameDebugUI_FPS", 0);
            }
            else
            {
                debugUIFPS.SetActive(true);
                PlayerPrefs.SetInt("gameDebugUI_FPS", 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply) || Input.GetKeyDown(KeyCode.F4) || Input.GetButtonDown("MENU") && Input.GetButtonDown("RED0"))
        {
            if (debugUICheckpoint.activeSelf == true)
            {
                debugUICheckpoint.SetActive(false);
                PlayerPrefs.SetInt("gameDebugUI_Checkpoint", 0);
            }
            else
            {
                debugUICheckpoint.SetActive(true);
                PlayerPrefs.SetInt("gameDebugUI_Checkpoint", 1);
            }
        }
        players = FindAllPlayers ();
		if (players == null) 
		{
			GameOverMethod ();
		}
//		foreach (float hp in playersHealth) 
//		{
//			print (hp);
//		}
	}
}
