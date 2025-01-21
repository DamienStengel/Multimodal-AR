using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoomInfoPanel : MonoBehaviour
{
    private static RoomInfoPanel instance;
    public static RoomInfoPanel Instance => instance;

    [Header("Room Info")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI nbPlaceText;

    [Header("Lists")]
    [SerializeField] private Transform objectListContent;
    [SerializeField] private Transform studentListContent;
    [SerializeField] private TextMeshProUGUI listItemPrefab; // Un seul prefab pour les deux listes
    [SerializeField] private TMP_Dropdown timeSelector;

    [Header("Navigation Buttons")]
    [SerializeField] private Button setAsStartButton;
    [SerializeField] private Button setAsEndButton;

    private string currentRoomId;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<GameObject> spawnedStudents = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeTimeSelector();
        InitializeButtons();
        gameObject.SetActive(false); // Cacher le panel au démarrage
    }

    private void InitializeTimeSelector()
    {
        timeSelector.ClearOptions();
        timeSelector.AddOptions(new List<string> { "13h", "14h", "15h", "16h", "17h" });
        timeSelector.onValueChanged.AddListener(OnTimeSelectorChanged);
    }

    private void InitializeButtons()
    {
        setAsStartButton.onClick.AddListener(() => {
            var cube = GameObject.Find($"Cube_{currentRoomId}");
            if (cube != null)
            {
                PathManager.Instance.SetStartPoint(cube);
                gameObject.SetActive(false);
            }
        });

        setAsEndButton.onClick.AddListener(() => {
            var cube = GameObject.Find($"Cube_{currentRoomId}");
            if (cube != null)
            {
                PathManager.Instance.SetEndPoint(cube);
                gameObject.SetActive(false);
            }
        });
    }

    public void ShowRoomInfo(string roomId)
    {
        currentRoomId = roomId;
        var roomInfo = RoomDataManager.Instance.GetRoomInfo(roomId);
        
        if (roomInfo != null)
        {
            roomNameText.text = roomInfo.name;
            nbPlaceText.text = $"Capacité: {roomInfo.capacity} places";

            UpdateObjectsList(roomInfo.equipment);
            OnTimeSelectorChanged(timeSelector.value);

            gameObject.SetActive(true);
        }
    }

    private void UpdateObjectsList(string[] equipment)
    {
        ClearList(spawnedObjects);

        foreach (var item in equipment)
        {
            var objectItem = Instantiate(listItemPrefab, objectListContent);
            objectItem.text = $"• {item}";
            spawnedObjects.Add(objectItem.gameObject);
        }
    }

    private void OnTimeSelectorChanged(int hourIndex)
    {
        int hour = 13 + hourIndex;
        var students = RoomDataManager.Instance.GetStudentsInRoom(currentRoomId, hour);
        UpdateStudentsList(students);
    }

    private void UpdateStudentsList(List<string> students)
    {
        ClearList(spawnedStudents);

        foreach (var student in students)
        {
            var studentItem = Instantiate(listItemPrefab, studentListContent);
            studentItem.text = $"• {student}";
            spawnedStudents.Add(studentItem.gameObject);
        }
    }

    private void ClearList(List<GameObject> list)
    {
        foreach (var item in list)
        {
            if (item != null)
                Destroy(item);
        }
        list.Clear();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
} 