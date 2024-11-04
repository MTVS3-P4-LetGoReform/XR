using TMPro;
using UnityEngine;

public enum State
{
    InteriorMode,
    CraftMode,
    
}
public class TestPlayerModeController : MonoBehaviour
{
    private State modeState;
    public GameObject craftSystem;
    public GameObject interiorSystem;
    public TMP_Text state_Text;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            modeState = State.CraftMode;
            craftSystem.gameObject.SetActive(true);
            interiorSystem.gameObject.SetActive(false);
            state_Text.text = "Craft Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            modeState = State.InteriorMode;
            craftSystem.gameObject.SetActive(false);
            interiorSystem.gameObject.SetActive(true);
            state_Text.text = "Interior Mode";
        }
    }
}
