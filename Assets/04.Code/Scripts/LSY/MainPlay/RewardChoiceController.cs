using UnityEngine;
using DG.Tweening;

public class RewardChoiceController : MonoBehaviour
{
    public GameObject statueIcon;
    public GameObject creditIcon;
    public void ChoiceStatue()
    {
        statueIcon.SetActive(true);
    }
    
}
