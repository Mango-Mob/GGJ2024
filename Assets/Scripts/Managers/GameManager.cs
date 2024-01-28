using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject m_player;
    public Camera m_activeCamera;
    public bool IsInCombat = false;
    public String time_desplay;
    public float SpawnDelay;
    System.Random random;

    public int MoneyPerGlass;
    public bool canSell;
    public AnimationCurve customerSpawnRate;
    public AnimationCurve customerPatience;
    public AnimationCurve customerPatienceDecay;

    public GameObject[] EnemiesToSpawn;
    public Spawner[] spawners;

    public int[] fruitCount = new int[(int)LiquidType.NumOfLiquid];
    public int initial_count;
    public int spawn_count;
    public float fruit_spawn_time;
    private float fruit_spawn_delay;

    struct Customer
    {
        public Customer(CustomerData data, float patience) { this.data = data; this.patience = patience; acceptDelay = data.glassesRevelTime; money_earned = 0; }

        public bool isNull { get { return data == null; } }

        public CustomerData data { get; private set; }
        public int money_earned;
        public float patience;
        public float acceptDelay;
    };
    
    public CustomerManager[] customerDisplay;
    private Customer[] customers = new Customer[4];

    private int currentCustomerCount
    {
        get
        {
            int result = 0;
            for (int i = 0; i < customers.Length; i++)
            {
                if(!customers[i].isNull)
                    result++;
            }
            return result;
        } 
    
    }

    private float[] customer_delay = { 1.0f, 1.0f, 1.0f, 1.0f };
    private float spawn_delay = 2.0f;
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

    public void RegisterFruit(LiquidType my_type)
    {
        fruitCount[(int)my_type]++;
    }
    public void RemoveFruit(LiquidType my_type)
    {
        fruitCount[(int)my_type]++;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
        fruit_spawn_delay = fruit_spawn_time;
        if (spawners.Length > 0)
        {
            for (int i = 0; i < (int)LiquidType.NumOfLiquid; i++)
            {
                for (int j = 0; j < initial_count; j++)
                {
                    int select = UnityEngine.Random.Range(0, spawners.Length);
                    spawners[select].Spawn(EnemiesToSpawn[i]);
                }
            }
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuScene")
            return;

        if(time < total_time)
            time += Time.deltaTime;

        if(time >= total_time && currentCustomerCount <= 0)
        {
            return;
        }

        CustomerUpdate();

        if (time < total_time)
            FruitSpawn();

        time_desplay = GetTimeNow();
    }

    private void FruitSpawn()
    {
        if(fruit_spawn_delay > 0)
        {
            fruit_spawn_delay -= Time.deltaTime;
            if (fruit_spawn_delay <= 0)
            {
                fruit_spawn_delay = fruit_spawn_time;
                for (int i = 0; i < (int)LiquidType.NumOfLiquid; i++)
                {
                    for (int j = 0; j < initial_count; j++)
                    {
                        int select = UnityEngine.Random.Range(0, spawners.Length);
                        spawners[select].Spawn(EnemiesToSpawn[i]);
                    }
                }
            }
        }
    }

    private void CustomerUpdate()
    {
        int spawn_customers_count = (int)customerSpawnRate.Evaluate(time) - customerCount;

        if (spawn_delay > 0)
            spawn_delay -= Time.deltaTime;

        for (int i = 0; i < customers.Length; i++)
        {
            if (customer_delay[i] > 0)
                customer_delay[i] -= Time.deltaTime;

            bool can_spawn = customers[i].isNull //Spot is free
                && customer_delay[i] < 0.0f     //delay is empty
                && spawn_delay < 0.0f           //No other spawn has occured
                && spawn_customers_count > 0    //There is enough to spawn
                && time <= total_time;          //Time hasn't finished.

            if (can_spawn)
            {
                customers[i] = new Customer(CustomerData.GenerateCustomer(random), customerPatience[customerPatience.keys.Length - 1].time);
                customerDisplay[i].UpdateCharacter(customers[i].data);
                spawn_delay = SpawnDelay;
                customerCount++;
            }
            else if (!customers[i].isNull)
            {
                if (customers[i].acceptDelay > 0)
                {
                    customers[i].acceptDelay -= Time.deltaTime;
                    if (customers[i].acceptDelay < 0)
                    {
                        customerDisplay[i].Show();
                    }
                    else
                        continue;
                }

                customers[i].patience -= Time.deltaTime * customerPatienceDecay.Evaluate(customers[i].data.count);
                if (customers[i].patience < 0)
                {
                    var earned = (int)(customers[i].money_earned * 0.5f);
                    money += earned;
                    if(earned > 0)
                        GetComponent<SoloAudioAgent>().Play();
                    customers[i] = new Customer();
                    customerDisplay[i].RemoveCustomer(false);
                    customer_delay[i] = 3.0f;
                }
                else
                    customerDisplay[i].UpdatePatience(customerPatience.Evaluate(customers[i].patience));
            }
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }

    public void SellTo( int index, LiquidQuantity fluid )
    {
        (float, int) diff = customers[index].data.RemoveBestCase(fluid);
        customers[index].money_earned += (int)(diff.Item1 * MoneyPerGlass);
        customers[index].patience = Mathf.Min(customers[index].patience + 0.5f, customerPatience[customerPatience.keys.Length - 1].time);
        customerDisplay[index].RemoveGlass(diff.Item2);
        if (customers[index].data.count <= 0)
        {
            var earned = (int)(customers[index].money_earned * (customerPatience.Evaluate(customers[index].patience) + 0.4f));
            money += earned;
            if (earned > 0)
                GetComponent<SoloAudioAgent>().Play();

            customers[index] = new Customer();
            customerDisplay[index].RemoveCustomer(diff.Item1 > 0);
            customer_delay[index] = 3.0f;
        }
    }

    public String GetTimeNow()
    {
        int extra_hours = (int)(time / timePerHour);

        int hour = 9 + extra_hours;
        if (hour > 12)
            return (hour - 12) + "pm";
        else if (hour == 12)
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
