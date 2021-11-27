using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Car : MonoBehaviour
{

    int id;
    int x;
    int y;
    public Client rest_client;
    public GameObject prefabAuto1;
    public GameObject prefabAuto2;
    public GameObject prefabAuto3;
    public GameObject prefabAuto4;
    public GameObject prefabAuto5;
    public int numAutos;
    List<GameObject> listaCoches;
    public MyCar[] cars;

    // Start is called before the first frame update
    void Start()
    {
        listaCoches = new List<GameObject>();

        for (int i = 0; i < rest_client.carsObj.Length; i++)
        {
            Debug.Log(cars[i].ToString());
            MyCar car = cars[i];
            GameObject prefab = prefabAuto5;
            if(car.car_type == 1){
               prefab = prefabAuto1;
            }
            if (car.car_type == 2){
                prefab = prefabAuto2;
            }
            if (car.car_type == 3){
                prefab = prefabAuto3;
            }
            if (car.car_type == 4){
                prefab = prefabAuto4;
            }
            Instantiate(prefab, new Vector3(30, 0.62f, 30), Quaternion.identity);
            listaCoches.Add(Instantiate(prefab, new Vector3(car.x, 0.62f, car.y), Quaternion.identity));
        }
        //transform.position = new Vector3(cars[0].x, 0, cars[0].y);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
