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

    System.Random random;

    public AnimationCurve customerSpawnRate;
    public AnimationCurve customerPatience;
    
    struct Customer
    {
        public Customer(CustomerData data) { this.data = data; patience = 5.0f; }

        public bool isNull { get { return data == null; } }

        public CustomerData data { get; private set; }
        public float patience;
    };
    
    public CustomerManager[] customerDisplay;
    private Customer[] customers = new Customer[4];

    private int customerCount = 0;
    public int money = 0;
    private int hours = 10; //9am to 6pm
    private float time = 0.0f;
    private float total_time { get { return hours * timePerHour; } }
    public float timePerHour;
    protected override void Awake()
    {
        base.Awake();
        random = random = new System.Random((int)System.DateTime.Now.Ticks);
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
            return;
        }
        int spawn_customers_count = (int)customerSpawnRate.Evaluate(time) - customerCount;

        for (int i = 0; i < customers.Length; i++)
        {
            if(customers[i].isNull && spawn_customers_count > 0)
            {
                customers[i] = new Customer(CustomerData.GenerateCustomer(random));
                customerDisplay[i].UpdateCharacter(customers[i].data);
            }
            else
            {
                customers[i].patience -= Time.deltaTime;
                customerDisplay[i].UpdatePatience(customerPatience.Evaluate(customers[i].patience));
            }
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

    public String GetScoreDisplay()
    {
        int cents = (int)(money % 100);
        if(cents <= 10)
            return "$" + (int)(money / 100) + ".0" + cents;
        else
            return "$" + (int)(money / 100) + "." + (int)(money % 100);
    }
}
