using UnityEngine;

// Attach this to any ground/platform object
public class GroundType : MonoBehaviour
{
    public enum Type { Normal, Sunling, Moonling, Poison }

    [SerializeField] public Type groundType = Type.Normal;

    private void Start()
    {
        // Auto set color based on type
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (groundType)
        {
            case Type.Normal:
                sr.color = Color.white;
                break;
            case Type.Sunling:
                sr.color = new Color(1f, 0.85f, 0f); // Yellow
                break;
            case Type.Moonling:
                sr.color = new Color(0.2f, 0.5f, 1f); // Blue
                break;
            case Type.Poison:
                sr.color = new Color(0.2f, 0.8f, 0.2f); // Green
                break;
        }
    }
}