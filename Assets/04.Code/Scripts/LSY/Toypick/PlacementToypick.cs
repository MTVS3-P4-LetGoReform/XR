// using System;
// using GLTFast;
// using UnityEngine;
//
// public class PlacementToypick : MonoBehaviour
// {
//     public PlacedToypickData placedToypickData;
//
//     public GameObject ToypicInteractionUI;
//     // 설치시 토이픽 데이터 설정
//     public void SetPlacedToypickData(string modelName, string imageName, Sprite imageSprite, GltfImport gltfImport)
//     {
//         placedToypickData = new PlacedToypickData(modelName, imageName, imageSprite, gltfImport);
//     }
//
//     public void OnTriggerStay(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             if (Input.GetKeyDown(KeyCode.E))
//             {
//                 ToypicInteractionUI.SetActive(true);
//             }
//         }
//         
//     }
// }