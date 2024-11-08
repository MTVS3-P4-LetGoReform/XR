using UnityEngine;

public class TestSkyboxController : MonoBehaviour
{
    [SerializeField] 
    private SkyboxDatabase skyboxDatabase;

    private int selecedSkyboxIndex = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selecedSkyboxIndex < 0)
        {
            return;
        }
    }

    public void ChangeSkyboxOnClick(int ID)
    {
        selecedSkyboxIndex = skyboxDatabase.skyboxData.FindIndex(data => data.ID == ID);
        if (selecedSkyboxIndex < 0)
        {
            return;
        }

        RenderSettings.skybox = (skyboxDatabase.skyboxData[selecedSkyboxIndex].ThemeSkybox);
    }
}
