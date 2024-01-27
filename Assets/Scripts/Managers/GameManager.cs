using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject m_player;
    public Camera m_activeCamera;
    public bool IsInCombat = false;
    public String time_desplay;

    private int hours = 10; //9am to 6pm
    private float time = 0.0f;
    private float total_time { get { return hours * timePerHour; } }
    public float timePerHour;
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }
    private void Update()
    {
        time += Time.deltaTime;
        if(time >= total_time)
        {

        }
        time_desplay = GetTimeNow();
    }
    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }

    public String GetTimeNow()
    {
        int extra_hours = (int)(time / timePerHour);

        int hour = 9 + extra_hours;
        if (extra_hours > 12)
            return (hour - 12) + "pm";
        else if (extra_hours == 12)
            return hour + "pm";
        return hour + "am";
    }
}
