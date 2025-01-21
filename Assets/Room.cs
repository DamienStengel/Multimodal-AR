using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Student> students {get; set;}  // Liste pour stocker les étudiants
    public List<object> objets {get; set;}  // Liste pour stocker les objets
    public int numbersOfSeats {get; set;}  // Nombre de places dans la salle

    public Room(List<Student> students, List<object> objets, int numbersOfSeats)
    {
        this.students = students;
        this.objets = objets;
        this.numbersOfSeats = numbersOfSeats;
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
