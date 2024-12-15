using Newtonsoft.Json;

public class ModelInfo
{
    public string model_id { get; set; }
    public string create_3d_name { get; set; }
    public string creator_id { get; set; }
    public string id { get; set; }
    public string prompt_en { get; set; }
    public string prompt_ko { get; set; }
    public string select_image_name { get; set; }
   

    // // 기본 생성자 추가
    // public LandInfo()
    // {
    // }

    // 또는 JsonConstructor 특성을 사용한 생성자
    [JsonConstructor]
    public ModelInfo(string modelId)
    {
        model_id = modelId;
    }
}