using UnityEngine;

public class StartingUNSet : MonoBehaviour
{
    private bool didWeDoIt = false;
    [SerializeField] private float setNum = 0f;

    void Start()
    {
        if (!didWeDoIt)
        {
            PlayerPrefs.SetFloat("UnlockNum", setNum);
            didWeDoIt = true;
        }
    }
}
