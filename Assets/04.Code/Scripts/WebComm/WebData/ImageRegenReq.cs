using UnityEngine;

public class ImageRegenReq
{
    // 프롬프트 텍스트
    public string prompt;
    // 생성할 이미지 수
    public int batch;
    // 생성 유저 아이디
    public string creator_id;
    // 생성될 모델 아이디
    public string model_id;
}