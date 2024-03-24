namespace AzureBlob1.Helpers;

public static class MediaHelper
{
    // IMAGES
    public static string MakeImageName(string filename)
    {
        FileInfo fileInfo = new FileInfo(filename);

        string extension = fileInfo.Extension;
        string name = "IMG_" + Guid.NewGuid() + extension;

        return name;
    }
    public static string[] GetImageExtensions()
    {
        return new string[]
        {
                // .JPG files
                ".jpg", ".jpeg",
                // .PNG files
                ".png",
                // .BMP files
                ".bmp",
                // .SVG files
                ".svg"
        };
    }


    //VIDEOS
    public static string MakeVideoName(string filename)
    {
        FileInfo fileInfo = new FileInfo(filename);

        string extension = fileInfo.Extension;
        string name = "VIDEO_" + Guid.NewGuid() + extension;

        return name;
    }

    public static string[] GetVideoExtensions()
    {
        return new string[]
        {
            ".mp4", ".mkv"
        };
    }
}
