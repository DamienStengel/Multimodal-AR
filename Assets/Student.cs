using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : MonoBehaviour
{
    public string name {get; set;}
    public string gender {get; set;}
    public string thirteen {get; set;}
    public string fourteen {get; set;}
    public string fifteen {get; set;}
    public string sixteen {get; set;}
    public string seventeen {get; set;}
    public string hair {get; set;}
    public int height {get; set;}
    public string transport {get; set;}
    public string clothing {get; set;}
    public int year {get; set;}
    public string specialization {get; set;}

    public Student(string name, string gender, string thirteen, string fourteen, string fifteen, string sixteen, string seventeen, string hair, int height, string transport, string clothing, int year, string specialization)
    {
        this.name = name;
        this.gender = gender;
        this.thirteen = thirteen;
        this.fourteen = fourteen;
        this.fifteen = fifteen;
        this.sixteen = sixteen;
        this.seventeen = seventeen;
        this.hair = hair;
        this.height = height;
        this.transport = transport;
        this.clothing = clothing;
        this.year = year;
        this.specialization = specialization;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
