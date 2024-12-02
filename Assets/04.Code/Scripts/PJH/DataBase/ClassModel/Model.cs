using System;

[Serializable]
public class Model
{
    public string id;
    public string creator_id;
    public string create_3d_name;
    public string select_image_name;
    public string prompt_ko;
    public int score;
    
    public Model() { }

    public Model(string id, string creator_id, string create_3d_name, string select_image_name)
    {
        this.id = id;
        this.creator_id = creator_id;
        this.create_3d_name = create_3d_name;
        this.select_image_name = select_image_name;
    }
}