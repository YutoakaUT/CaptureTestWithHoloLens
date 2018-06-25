using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System;
using System.IO;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class TextureToHitObject : MonoBehaviour, IInputClickHandler
{
    public GameObject targetObject = null;
    public Material formatMaterial = null;

    private PhotoCapture photoCaptureObject = null;
    private Material changeMaterial = null;
    private GameObject hitObject = null; //Gazeと衝突したオブジェクト
    // Use this for initialization
    void Start()
    {
        // AirTap時のイベントを設定する
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }

    /// <summary>
    /// AirTapイベント
    /// </summary>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("AirTapされましたcaptureを開始します");


        //GazeManagerのHitObject関数を用いて衝突しているオブジェクトの情報を格納
        hitObject = GazeManager.Instance.HitObject;
        Debug.Log(hitObject);

        // キャプチャを開始する（メソッド呼び出し）
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        Debug.Log("OnPhotoCaptureCreated");
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        //フォトモードをスタートする（メソッド呼び出し）
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    /// <summary>
    /// フォトモードをスタートするメソッド
    /// </summary>
    /// <param name="result"></param>
    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("フォトモードを開始します");
        if (result.success)
        {
            Debug.Log("フォトモード: success");
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("フォトモード: Unable to start photo mode!");
        }
    }


    /// <summary>
    /// 撮影した画像データをテクスチャに変換し物体にマテリアルとして貼り付ける
    /// </summary>
    /// <param name="result"></param>
    /// <param name="photoCaptureFrame"></param>
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.Log("フォトを一時的に保存");
        if (result.success)
        {
            Debug.Log("フォトを一時的に保存: success");

            

            // 使用するTexture2Dを作成し、正しい解像度を設定する
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // 画像データをターゲットテクスチャにコピーする
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            // テクスチャをマテリアルに適用する
            changeMaterial = new Material(formatMaterial);
            changeMaterial.SetTexture("_MainTex", targetTexture);
            targetObject.GetComponent<Renderer>().material = changeMaterial;

            hitObject.GetComponent<Renderer>().material = changeMaterial; //Rayと衝突しているオブジェクトにマテリアルを貼り付ける

            Debug.Log("オブジェクトにテクスチャを貼り付けました");
        }
        // クリーンアップ（終了オプション）
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    /// <summary>
    /// 終了オプション
    /// </summary>
    /// <param name="result"></param>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("フォトモードを終了します");
        photoCaptureObject.Dispose(); //キャプチャーオブジェクトの消去
        photoCaptureObject = null; //キャプチャーオブジェクトの初期化
    }


}