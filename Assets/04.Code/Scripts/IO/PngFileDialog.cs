using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public static class PngFileDialog{
    //private string fileContent = string.Empty;
    // 선택한 파일 경로 저장
    private static string filePath = string.Empty;
    // 대화상자 객체
    private static OpenFileDialog openFileDialog;

    static PngFileDialog()
    {
        openFileDialog = new OpenFileDialog();
        // 초기 디렉토리 경로 설정
        openFileDialog.InitialDirectory = "c:\\";
        // 파일 필터를 png와 모든 파일로 설정
        openFileDialog.Filter = "png files (*.png) |*.png|All files  (*.*)|*.*";
        // 파일 대화 상자가 열릴 때 n번재로 정의 된 필터가 기본 선택.
        openFileDialog.FilterIndex = 1;
        // 대화상자 제목 설정
        openFileDialog.Title = "Image Dialog";
        // 마지막 사용한 디렉토리 기억하도록
        openFileDialog.RestoreDirectory = true;

    }

    /* 파일 선택 대화 상자를 열고 선택 파일 스트림 반환 */
    public static Stream FileOpen()
    {
        Stream openStream;
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            filePath = openFileDialog.FileName;
            openStream = openFileDialog.OpenFile();
            return openStream;
        }

        return null;
    }

    /* stream데이터로 png의 base64데이터 가져오기 */
    public static string ConvertPngStreamToBase64(Stream stream)
    {
        int len = (int)stream.Length;
        // stream 현재 위치를 처음으로 이동.
        stream.Position = 0;

        // stream데이터를 바이트 배열로 읽기
        byte[] buffer = new byte[len];
        stream.Read(buffer, 0, len);

        // 바이트 배열을 base64로 변환.
        string base64String = Convert.ToBase64String(buffer);
        stream.Close();
        return base64String;
    }
}