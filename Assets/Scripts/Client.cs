using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//public static class JsonHelper
//{
//    public static T[] FromJson<T>(string json)
//    {
//        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
//        return wrapper.Items;
//    }

//    public static string ToJson<T>(T[] array)
//    {
//        Wrapper<T> wrapper = new Wrapper<T>();
//        wrapper.Items = array;
//        return JsonUtility.ToJson(wrapper);
//    }

//    public static string ToJson<T>(T[] array, bool prettyPrint)
//    {
//        Wrapper<T> wrapper = new Wrapper<T>();
//        wrapper.Items = array;
//        return JsonUtility.ToJson(wrapper, prettyPrint);
//    }

//    [System.Serializable]
//    private class Wrapper<T>
//    {
//        public T[] Items;
//    }
//}

[System.Serializable]
public class MyCar
{
    public int id;
    public int x;
    public int y;
    public int direction;
    public int speed;
    public int acceleration;
    public int car_type;
    public int max_speed;
    public List<List<int>> route;
    override public string ToString()
    {
        return "Id: " + id + ", X: " + x + ", Y: " + y;
    }
}

[System.Serializable]
public class MyTrafficLight
{
    public int id;
    public bool state;
    public int x;
    public int y;
    public int direction;
    public override string ToString()
    {
        return "Id: " + id + ", State: " + state + ", X: " + x + ", Y: " + y;
    }
}

[System.Serializable]
class AgentCollection
{
    public MyCar[] cars;
    public MyTrafficLight[] tlights;
}


public class Client : MonoBehaviour
{
    string simulationURL;
    public GameObject prefabAuto1;
    public GameObject prefabAuto2;
    public GameObject prefabAuto3;
    public GameObject prefabAuto4;
    public GameObject prefabAuto5;
    List<GameObject> listaCoches;
    public MyCar[] cars;
    private float waitTime = 0.1f;
    private float timer = 0.0f;
    public MyCar[] carsObj;
    public MyTrafficLight[] trafficObj;
    public Car carsManager = new Car();
    int start = 0;


    void Start()
    {
        StartCoroutine(ConnectToMesa());
        StartCoroutine(UpdatePositions());
    }

    IEnumerator ConnectToMesa()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post("https://simulaciontrafico-noisy-guanaco-wx.mybluemix.net/simulations", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                simulationURL = www.GetResponseHeader("Location");
                Debug.Log("Connected to simulation through Web API");
                Debug.Log(simulationURL);
            }
        }

    }

    IEnumerator UpdatePositions()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(simulationURL))
        {
            if (simulationURL != null)
            {
                // Request and wait for the desired page.
                yield return www.SendWebRequest();

                Debug.Log(www.downloadHandler.text);
                Debug.Log("Data has been processed");
                AgentCollection agents = JsonUtility.FromJson<AgentCollection>(www.downloadHandler.text);
                if (start == 0)
                {
                    instantiateGameObjects(agents.cars);
                }
                else
                {
                    for (int i = 0; i < agents.cars.Length; i++)
                    {
                        positionCar(agents.cars[i], listaCoches[i]);
                    }
                    foreach (MyTrafficLight tlight in agents.tlights)
                        Debug.Log(tlight.ToString());
                }
                start++;
                // transform.position = new Vector3(cars[0].x, 0, cars[0].y);                
            }
        }
    }

    void positionCar(MyCar car, GameObject carObj)
    {
        carObj.transform.position = new Vector3(car.x, 1.5f, car.y);
    }

    void instantiateGameObjects(MyCar[] cars)
    {
        listaCoches = new List<GameObject>();

        for (int i = 0; i < cars.Length; i++)
        {
            Debug.Log(cars[i].ToString());
            MyCar car = cars[i];
            GameObject prefab = prefabAuto5;
            if (car.car_type == 1)
            {
                prefab = prefabAuto1;
            }
            if (car.car_type == 2)
            {
                prefab = prefabAuto2;
            }
            if (car.car_type == 3)
            {
                prefab = prefabAuto3;
            }
            if (car.car_type == 4)
            {
                prefab = prefabAuto4;
            }
            listaCoches.Add(Instantiate(prefab, new Vector3(car.x, 1.5f, car.y), Quaternion.identity));
        }

        for (int i = 0; i < listaCoches.Count; i++)
        {
            MyCar car = cars[i];
            GameObject carObj = listaCoches[i];
            if (car.direction == 0)
            {
                carObj.transform.Rotate(-90.0f, 0.0f, 90.0f, Space.Self);
            }
            if (car.direction == 1)
            {
                carObj.transform.Rotate(-90.0f, 0.0f, -90.0f, Space.Self);
            }
            if (car.direction == 2)
            {
                carObj.transform.Rotate(-90.0f, 0.0f, 180.0f, Space.Self);
            }
            if (car.direction == 3)
            {
                carObj.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            StartCoroutine(UpdatePositions());
            timer = timer - waitTime;
        }
    }
}
