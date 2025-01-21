using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class RoomDataManager : MonoBehaviour
{
    private static RoomDataManager instance;
    public static RoomDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomDataManager>(true);
                if (instance == null)
                {
                    Debug.LogError("Aucun RoomDataManager trouvé dans la scène!");
                }
            }
            return instance;
        }
    }

    private RoomsData roomsData;
    private List<StudentData> studentsData;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
            
        LoadRoomsData();
        LoadStudentsData();
    }

    void LoadRoomsData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("rooms_data");
        if (jsonFile == null)
        {
            Debug.LogError("Le fichier rooms_data.json n'a pas été trouvé dans le dossier Resources");
            return;
        }
        
        try
        {
            string jsonContent = jsonFile.text;
            Debug.Log($"Contenu JSON chargé: {jsonContent}"); // Pour déboguer
            
            roomsData = JsonUtility.FromJson<RoomsData>(jsonContent);
            
            if (roomsData == null)
            {
                Debug.LogError("Échec de la désérialisation des données des salles");
                return;
            }
            
            if (roomsData.rooms == null)
            {
                Debug.LogError("Le tableau des salles est null");
                return;
            }
            
            Debug.Log($"Données des salles chargées avec succès. Nombre de salles: {roomsData.rooms.Length}");
            
            // Afficher les salles chargées pour déboguer
            foreach (var room in roomsData.rooms)
            {
                Debug.Log($"Salle chargée: {room.id} - {room.info.name}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors du chargement des données des salles: {e.Message}\n{e.StackTrace}");
        }
    }

    void LoadStudentsData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("students(1)");
        // Parser le CSV et stocker les données
        studentsData = ParseCSV(csvFile.text);
    }

    public List<string> GetStudentsInRoom(string roomName, int hour)
    {
        return studentsData
            .Where(s => GetRoomAtHour(s, hour) == roomName)
            .Select(s => s.name)
            .ToList();
    }

    private string GetRoomAtHour(StudentData student, int hour)
    {
        switch(hour)
        {
            case 13: return student.h13;
            case 14: return student.h14;
            case 15: return student.h15;
            case 16: return student.h16;
            case 17: return student.h17;
            default: return "";
        }
    }

    public RoomInfo GetRoomInfo(string roomId)
    {
        if (roomsData == null)
        {
            Debug.LogError("Les données des salles ne sont pas initialisées!");
            return null;
        }

        var rooms = roomsData.GetRoomsDictionary();
        Debug.Log($"Recherche d'informations pour la salle: {roomId}");
        
        if (rooms.ContainsKey(roomId))
        {
            return rooms[roomId];
        }
        
        Debug.LogWarning($"Aucune information trouvée pour la salle: {roomId}");
        return null;
    }

    private List<StudentData> ParseCSV(string csvText)
    {
        var students = new List<StudentData>();
        var lines = csvText.Split('\n');
        
        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            var values = line.Split(',');
            if (values.Length >= 13)
            {
                var student = new StudentData
                {
                    name = values[0],
                    gender = values[1],
                    h13 = values[2],
                    h14 = values[3],
                    h15 = values[4],
                    h16 = values[5],
                    h17 = values[6],
                    hair = values[7],
                    height = int.Parse(values[8]),
                    transport = values[9],
                    clothing = values[10],
                    year = int.Parse(values[11]),
                    specialization = values[12]
                };
                students.Add(student);
            }
        }
        return students;
    }
} 