using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;

public class CubeSelector : MonoBehaviour
{
    private static CubeSelector selectedCube;
    private Renderer cubeRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isStartPoint = false;

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
    }

    private void OnEnable()
    {
        // S'abonner aux événements LeanTouch
        LeanTouch.OnFingerTap += HandleFingerTap;
        
        // Si c'était un point sélectionné, restaurer son état
        if (isSelected)
        {
            cubeRenderer.material.color = isStartPoint ? Color.green : Color.red;
        }
    }

    private void OnDisable()
    {
        // Sauvegarder la dernière position connue
        PathManager.Instance.RegisterCubePosition(gameObject.name, transform.position);
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    private void HandleFingerTap(LeanFinger finger)
    {
        // Vérifier si le tap est sur ce cube
        var ray = finger.GetRay(Camera.main);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == this.transform)
            {
                Debug.Log($"Tap détecté sur {gameObject.name}");

                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                if (selectedCube != null && selectedCube != this)
                {
                    selectedCube.Deselect();
                }

                if (!isSelected)
                {
                    Select();
                }
                else
                {
                    Deselect();
                }
            }
        }
    }

    private void Select()
    {
        isSelected = true;
        selectedCube = this;

        // Si c'est le premier point sélectionné
        if (PathManager.Instance.GetStartPoint() == null)
        {
            isStartPoint = true;
            cubeRenderer.material.color = Color.green; // Point de départ en vert
            PathManager.Instance.SetStartPoint(gameObject);
        }
        else
        {
            isStartPoint = false;
            cubeRenderer.material.color = Color.red; // Point d'arrivée en rouge
            PathManager.Instance.SetEndPoint(gameObject);
        }

        Debug.Log($"Cube sélectionné comme {(isStartPoint ? "départ" : "arrivée")}: {gameObject.name}");
    }

    private void Deselect()
    {
        isSelected = false;
        if (selectedCube == this)
            selectedCube = null;
        cubeRenderer.material.color = originalColor;
    }

    public static CubeSelector GetSelectedCube()
    {
        return selectedCube;
    }
} 