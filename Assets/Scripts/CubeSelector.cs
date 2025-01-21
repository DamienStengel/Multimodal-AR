using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;

public class CubeSelector : MonoBehaviour
{
    private static CubeSelector selectedCube;
    private Renderer cubeRenderer;
    private Color originalColor;
    private bool isSelected = false;

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
        Deselect();
    }

    private void OnDisable()
    {
        // Se désabonner des événements LeanTouch
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
        cubeRenderer.material.color = Color.yellow;
        Debug.Log($"Cube sélectionné: {gameObject.name}");
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