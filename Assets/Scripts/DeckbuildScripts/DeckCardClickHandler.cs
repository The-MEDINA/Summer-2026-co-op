using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float maxRotationDegrees = 30;
    [SerializeField] private float hoverSizeIncrease = 1.25f;
    [SerializeField] private float examineSizeIncrease = 2f;
    [SerializeField] private BoxCollider2D boxCollider;

    private bool applyRotation = false;
    private Vector3 originalScale;
    private Vector3 hoverScale;
    private Vector3 examineScale;
    private bool examining = false;
    private bool hovering = false;
    private bool down = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
        hoverScale = transform.localScale * hoverSizeIncrease;
        examineScale = transform.localScale * examineSizeIncrease;
    }

    // Update is called once per frame
    void Update()
    {
        if (examining)
        {
            transform.rotation = Quaternion.identity;
            Vector3 position = transform.position;
            position.z = -1;
            transform.position = position;
            transform.localScale = examineScale;
        }
        else
        {
            Vector3 position = transform.position;
            position.z = 0;
            transform.position = position;
        }
        if (applyRotation && !examining)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 relativePosition = mouse - transform.position;
            Vector2 rotationDegrees = new Vector2(relativePosition.x / (boxCollider.bounds.extents.x / maxRotationDegrees), relativePosition.y / (boxCollider.bounds.extents.y / maxRotationDegrees));
            transform.rotation = Quaternion.Euler(rotationDegrees.y, rotationDegrees.x * -1, 0);
            transform.localScale = hoverScale;
        }
        else if (hovering && !examining)
        {
            applyRotation = true;
            transform.localScale = hoverScale;
        }
        if (down)
        {
            applyRotation = false;
            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (examining) examining = false;
        else examining = true;
        applyRotation = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        down = true;
        applyRotation = false;
        transform.localScale = originalScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //if (!hovering) transform.localScale = originalScale;
        //else
        //{
        //    transform.localScale = hoverScale;
        //}
        down = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        if (!examining)
        {
            applyRotation = true;
            transform.localScale = hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        applyRotation = false;
        hovering = false;
        if (!examining)
        {
            transform.rotation = Quaternion.identity;
            transform.localScale = originalScale;
        }
    }
}
