using UnityEngine;

public class StartingUNSet : MonoBehaviour
{
    private bool didWeDoIt = false;
    [SerializeField] private bool resetSetNum = false;
    [SerializeField] private float setNum = 0f;

    void Start()
    {
        if(resetSetNum)
        {
            PlayerPrefs.SetFloat("CheckIfWe'veDoneThis", 1);
        }

        if (PlayerPrefs.GetFloat("CheckIfWe'veDoneThis") != 12345)
        {
            if (!didWeDoIt)
            {
                PlayerPrefs.SetFloat("UnlockNum", setNum);
                didWeDoIt = true;
            }
            PlayerPrefs.SetFloat("CheckIfWe'veDoneThis", 12345);
        }
    }
}
