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
    public int car_type;
    public bool active;
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


public class Simulation : MonoBehaviour
{
    string simulationURL;
    public GameObject prefabAuto1;
    public GameObject prefabAuto2;
    public GameObject prefabAuto3;
    public GameObject prefabAuto4;
    public GameObject prefabAuto5;
    public GameObject trafficlight;
    public Texture greenLight;
    public Texture redLight;
    List<GameObject> listaCoches;
    List<GameObject> listaTlights;
    List<List<float>> tlightPos = new List<List<float>>();
    public MyCar[] cars;
    private float waitTime = 0.3f;
    private float timer = 0.0f;
    public MyCar[] carsObj;
    public MyTrafficLight[] trafficObj;
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

    void addTlightsPositions()
    {
        // 0 - 3
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 265.0f });
        tlightPos.Add(new List<float>() { 335.0f, 1.5f, 340.0f });
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 340.0f });
        tlightPos.Add(new List<float>() { 335.0f, 1.5f, 265.0f });
        // 4 - 7
        tlightPos.Add(new List<float>() { 44.0f, 1.5f, 265.0f });
        tlightPos.Add(new List<float>() { 113.0f, 1.5f, 340.0f });
        tlightPos.Add(new List<float>() { 44.0f, 1.5f, 340.0f });
        tlightPos.Add(new List<float>() { 113.0f, 1.5f, 265.0f });
        // 8 - 11
        tlightPos.Add(new List<float>() { 487.0f, 1.5f, 267.0f });
        tlightPos.Add(new List<float>() { 555.0f, 1.5f, 337.0f });
        tlightPos.Add(new List<float>() { 487.0f, 1.5f, 337.0f });
        tlightPos.Add(new List<float>() { 555.0f, 1.5f, 267.0f });
        // 12 - 15
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 43.0f });
        tlightPos.Add(new List<float>() { 334.0f, 1.5f, 118.0f });
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 118.0f });
        tlightPos.Add(new List<float>() { 334.0f, 1.5f, 43.0f });
        // 16 - 19
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 482.0f });
        tlightPos.Add(new List<float>() { 335.0f, 1.5f, 558.0f });
        tlightPos.Add(new List<float>() { 265.0f, 1.5f, 558.0f });
        tlightPos.Add(new List<float>() { 335.0f, 1.5f, 482.0f });

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
                    instantiateGameObjects(agents.cars, agents.tlights);
                }
                else
                {
                    for (int i = 0; i < agents.cars.Length; i++)
                    {
                        positionCar(agents.cars[i], listaCoches[i]);
                    }
                    for (int i = 0; i < agents.tlights.Length; i++)
                    {
                        changeTlight(agents.tlights[i], listaTlights[i]);
                    }
                }
                start++;             
            }
        }
    }

    void positionCar(MyCar car, GameObject carObj)
    {
        if (car.active)
        {
            carObj.transform.position = new Vector3(car.x, 1.5f, car.y);
            rotateCarDirection(car, carObj);
        }
        else
        {
            Destroy(carObj);
        }
    }

    void changeTlight(MyTrafficLight tlight, GameObject tlObj)
    {
        if (tlight.state)
        {
            tlObj.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", greenLight);
        }
        else
        {
            tlObj.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", redLight);
        }
    }

    void rotateCarDirection(MyCar car, GameObject carObj)
    {
        if (car.direction == 0)
        {
            carObj.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 90.0f);
        }
        if (car.direction == 1)
        {
            carObj.transform.eulerAngles = new Vector3(-90.0f, 0.0f, -90.0f);
        }
        if (car.direction == 2)
        {
            carObj.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 180.0f);
        }
        if (car.direction == 3)
        {
            carObj.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        }
    }

    void instantiateGameObjects(MyCar[] cars, MyTrafficLight[] tlights)
    {
        listaCoches = new List<GameObject>();
        listaTlights = new List<GameObject>();

        addTlightsPositions();

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
            rotateCarDirection(cars[i], listaCoches[i]);
        }

        for (int i = 0; i < tlights.Length; i++)
        {
            GameObject prefabT = trafficlight;

            listaTlights.Add(Instantiate(prefabT, new Vector3(tlightPos[i][0], tlightPos[i][1], tlightPos[i][2]), Quaternion.identity));
        }

        for (int i = 0; i < listaTlights.Count; i++)
        {
            MyTrafficLight tlightL = tlights[i];
            GameObject tlightObj = listaTlights[i];
            if (tlightL.direction == 0)
            {
                tlightObj.transform.Rotate(-90.0f, 0.0f, -90.0f, Space.Self);
            }
            if (tlightL.direction == 1)
            {
                tlightObj.transform.Rotate(-90.0f, 0.0f, 90.0f, Space.Self);
            }
            if (tlightL.direction == 2)
            {
                tlightObj.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            }
            if (tlightL.direction == 3)
            {
                tlightObj.transform.Rotate(-90.0f, 0.0f, 180.0f, Space.Self);
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
