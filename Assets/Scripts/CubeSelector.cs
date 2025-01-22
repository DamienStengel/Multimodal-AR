using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;

public class CubeSelector : MonoBehaviour
{
    private static CubeSelector selectedCube;
    private Renderer cubeRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isInNavigationMode = false;
    private AudioSource audioSource;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            originalColor = cubeRenderer.material.color;
        }
        else
        {
            Debug.LogError($"Pas de Renderer trouvé sur {gameObject.name}");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = Resources.Load<AudioClip>("Sounds/select_sound");
    }

    private void OnEnable()
    {
        // S'abonner aux événements LeanTouch
        LeanTouch.OnFingerTap += HandleFingerTap;
        
        // Si c'était un point sélectionné, restaurer son état
        if (isSelected)
        {
            cubeRenderer.material.color = Color.yellow;
        }
    }

    private void OnDisable()
    {
        // Sauvegarder la dernière position connue
        PathManager.Instance.RegisterCubePosition(gameObject.name, transform.position);
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    public void SetNavigationMode(bool enabled)
    {
        isInNavigationMode = enabled;
        // Réinitialiser la couleur quand on entre/sort du mode navigation
        if (!enabled && isSelected)
        {
            Deselect();
        }
    }

    private void HandleFingerTap(LeanFinger finger)
    {
        Ray ray = finger.GetRay();
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == this.transform) // Vérifier si on a cliqué sur CE cube
            {
                string cubeName = hit.transform.name;
                Debug.Log($"Tap détecté sur {cubeName}");

                if (isInNavigationMode)
                {
                    // En mode navigation, définir comme point d'arrivée
                    PathManager.Instance.SetEndPoint(hit.transform.gameObject);
                }
                else
                {
                    // Mode normal, désélectionner le cube précédent
                    if (selectedCube != null && selectedCube != this)
                    {
                        selectedCube.Deselect();
                    }

                    // Afficher les infos de la salle
                    if (cubeName.StartsWith("Cube_"))
                    {
                        string roomId = cubeName.Replace("Cube_", "");
                        Select(roomId);
                    }
                }
            }
        }
    }

    private void Select(string roomId)
    {
        isSelected = true;
        selectedCube = this;
        
        cubeRenderer.material.color = Color.yellow;
        
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Son de sélection non trouvé");
        }
        
        Debug.Log($"Tentative d'affichage du panel pour la salle: {roomId}");
        
        if (RoomDataManager.Instance == null)
        {
            Debug.LogError("RoomDataManager.Instance est null!");
            return;
        }
        
        var roomInfo = RoomDataManager.Instance.GetRoomInfo(roomId);
        if (roomInfo == null)
        {
            Debug.LogError($"Aucune information trouvée pour la salle {roomId}");
            return;
        }
        
        if (RoomInfoPanel.Instance != null)
        {
            RoomInfoPanel.Instance.ShowRoomInfo(roomId);
        }
        else
        {
            Debug.LogError("RoomInfoPanel.Instance est null!");
        }
        
        Debug.Log($"Cube sélectionné: {gameObject.name}");
    }

    public void Deselect()
    {
        isSelected = false;
        if (selectedCube == this)
            selectedCube = null;
        if (cubeRenderer != null)
            cubeRenderer.material.color = originalColor;
    }

    public static CubeSelector GetSelectedCube()
    {
        return selectedCube;
    }
} 