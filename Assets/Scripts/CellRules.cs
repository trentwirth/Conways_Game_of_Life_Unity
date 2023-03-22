using UnityEngine;

public class CellRules : MonoBehaviour
{
    public bool IsAlive { get; set; }
    public Color ColorInheritance { get; private set; }

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        IsAlive = false;
        ColorInheritance = Color.black;
        UpdateColor();
    }

    public void ToggleState()
    {
        IsAlive = !IsAlive;

        if (IsAlive)
        {
            // Generate a random color when the cell is clicked into existence.
            ColorInheritance = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        else
        {
            ColorInheritance = Color.black;
        }

        UpdateColor();
    }

    public void InheritColor(Color parentColor1, Color parentColor2)
    {
        // Average the colors of the two parent cells.
        ColorInheritance = (parentColor1 + parentColor2) / 2;

        // Optionally, you can add a small random mutation to the inherited color.
        float mutationStrength = 0.05f;
        Color mutation = new Color(
            Random.Range(-mutationStrength, mutationStrength),
            Random.Range(-mutationStrength, mutationStrength),
            Random.Range(-mutationStrength, mutationStrength)
        );
        ColorInheritance += mutation;
        ColorInheritance = new Color(
            Mathf.Clamp01(ColorInheritance.r),
            Mathf.Clamp01(ColorInheritance.g),
            Mathf.Clamp01(ColorInheritance.b)
        );
    }

    private void UpdateColor()
    {
        _spriteRenderer.color = IsAlive ? ColorInheritance : Color.black;
    }

    public void SetState(bool state)
    {
        IsAlive = state;
        UpdateColor();
    }
}