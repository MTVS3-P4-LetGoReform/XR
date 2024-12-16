using System;
using UnityEngine;

public class AIDBTest : MonoBehaviour
{
    private async void Start()
    {
        /*
        var model = new Model("m_id_fc9d928d",
            "cRShguIcsSX1ctiiruERJpFVwQD2",
            "241215_160401_8962318021d9594b534a29d0358c8d9c_0.glb",
            "241215_160401_8962318021d9594b534a29d0358c8d9c_0.png");
            */
        /*
        var model = new Model("m_id_fa22650e",
            "cRShguIcsSX1ctiiruERJpFVwQD2",
            "241114_045529_0489077e6f678fb9867f5dbf19c6400f_0.glb",
            "241114_045529_0489077e6f678fb9867f5dbf19c6400f_0.png");

        await RealtimeDatabase.AddAiModelAsync("cRShguIcsSX1ctiiruERJpFVwQD2",model);
        */
        //var list = await RealtimeDatabase.GetModelListAsync("cRShguIcsSX1ctiiruERJpFVwQD2");
        await RealtimeDatabase.AddAiModelAsync("cRShguIcsSX1ctiiruERJpFVwQD2","m_id_fa22650e");
        var models = await RealtimeDatabase.GetAllUserModelsAsync("cRShguIcsSX1ctiiruERJpFVwQD2");
        foreach (var model in models)
        {
            Debug.Log(model.select_image_name);
        }
    }
}
