using TMPro;
using UnityEngine;

public class Tile : GridCell
{
    [SerializeField] MeshRenderer tileMeshRenderer;
    [SerializeField] Color clickableColor;
    [SerializeField] Color obstacleColor;
    [SerializeField] Color clickedColor; // tile clicked color

    public bool isBlocked = false;

    private TMP_Text labelText;
    private Color originalTileColor;
    private static Tile currentClickedTile;

    private void OnEnable() => Player.onDestinationReached += ResetToOriginalColor;
    private void OnDisable() => Player.onDestinationReached -= ResetToOriginalColor;

    private void Start()
    {
        labelText = GetComponent<TMP_Text>();
        originalTileColor = tileMeshRenderer.material.color;

        SetLabelTextVisibility(false); // Initially hide the label text
    }

    private void Update()
    {
        HandleMouseOver();
        HandleMouseClick();
    }

    private void HandleMouseOver()
    {
        bool isHitTile = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.gameObject == gameObject;

        if (isHitTile)
        {
            SetLabelText(hit.collider.GetComponent<Tile>());
        }
        else
        {
            SetLabelTextVisibility(false);
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool isHitTile = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.gameObject == gameObject;

            if (isHitTile)
            {
                if (currentClickedTile != null && currentClickedTile != this)
                {
                    currentClickedTile.ResetToOriginalColor();
                }

                currentClickedTile = this;
                SetMeshColor(clickedColor);
            }
        }
    }

    private void SetLabelText(Tile hitTile)
    {
        labelText.text = $"{hitTile.GridPosition.x},{hitTile.GridPosition.y}";
        SetLabelTextVisibility(true);
    }

    private void OnMouseEnter()
    {
        if (currentClickedTile != this)
        {
            UpdateMeshColor();
        }
    }

    private void OnMouseExit()
    {
        if (currentClickedTile != this)
        {
            tileMeshRenderer.material.color = originalTileColor;
        }
        SetLabelTextVisibility(false);
    }

    private void UpdateMeshColor()
    {
        if (!isBlocked)
            SetMeshColor(clickableColor);
        else
            SetMeshColor(obstacleColor);
    }

    public void SetMeshColor(Color color)
    {
        tileMeshRenderer.material.color = color;
    }

    private void SetLabelTextVisibility(bool isVisible)
    {
        if (labelText != null)
        {
            labelText.enabled = isVisible;
        }
    }

    public void ResetToOriginalColor()
    {
        SetMeshColor(originalTileColor);
    }
}
