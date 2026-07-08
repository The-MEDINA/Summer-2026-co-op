using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float maxRotationDegrees = 30;
    [SerializeField] private BoxCollider2D boxCollider;
    private bool applyRotation = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (applyRotation)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 relativePosition = mouse - transform.position;
            Vector2 rotationDegrees = new Vector2(relativePosition.x / (boxCollider.bounds.extents.x / maxRotationDegrees), relativePosition.y / (boxCollider.bounds.extents.y / maxRotationDegrees));
            transform.rotation = Quaternion.Euler(rotationDegrees.y, rotationDegrees.x * -1, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        applyRotation = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        applyRotation = false;
        transform.rotation = Quaternion.identity;
    }
}
