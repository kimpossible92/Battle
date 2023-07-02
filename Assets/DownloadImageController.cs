using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ImageUtilties;

public class DownloadImageController : MonoBehaviour
{
    private string firstImageURL = "https://i.ibb.co/g6V1VTn/image1.jpg";
    private string secondImageURL = "https://i.ibb.co/YL8MtZq/image2.jpg";
    private string spriteImageURL = "https://i.ibb.co/NrPkDSN/unity.png";
    private string ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";
    public RawImage image1;
    public RawImage image2;
    public RawImage[] images;
    public Image image3;
    [SerializeField] Texture GetTextureload;
    public int positionPage = 1;
    void Start()
    {
        _ = DownloadImagesAsync();
    }
    public void DownPlus()
    {
        DecCount++;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].texture = GetTextureload;
        }
        _ = DownloadImagesAsync();
    }
    public void DownMinus()
    {
        DecCount--;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].texture = GetTextureload;
        }
        _ = DownloadImagesAsync();
    }
    [SerializeField] int DecCount = 0;
    public async UniTask DownloadImagesAsync()
    {
        for (int i =0; i<images.Length;i++)
        {
            int urlPage = (DecCount * 10)+positionPage + i;
            images[i].texture = await DownloadJPGImage(ImageUrl + urlPage + ".jpg", i.ToString());
        }
    }

    public async UniTask<Texture2D> DownloadJPGImage(string url , string name )
    {
        Texture2D img = await ImageDownloader.DownloadImage(url, name, FileFormat.JPG);
        return img;
    }

    public async UniTask<Sprite> DownloadPNGImage(string url, string name)
    {
        Texture2D img = await ImageDownloader.DownloadImage(url, name, FileFormat.PNG);
        return Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
    }
}
