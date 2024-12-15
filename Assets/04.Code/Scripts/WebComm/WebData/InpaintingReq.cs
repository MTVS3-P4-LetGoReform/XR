public class InpaintingReq
{
    // 기존 이미지 파일명
    public string image_filename;
    // 머리 수정 여부
    public bool is_hair_change;
    // 머리 수정 프롬프트
    public string hair_prompt;
    // 옷 수정 여부
    public bool is_clothes_change;
    // 옷 수정 프롬프트
    public string clothes_prompt;
    // 생성자 아이디
    public string creator_id; 
}