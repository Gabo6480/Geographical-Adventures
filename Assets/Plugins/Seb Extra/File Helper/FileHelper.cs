using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public static class FileHelper
{
	public enum SaveTextureFileFormat
	{
		JPG,
		PNG,
        TGA,
        EXR
    }

    public static string MakePath(params string[] folderNames)
	{
		return Path.Combine(folderNames);
	}

	public static string PersistantDataPath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	/// <summary>
	/// Save bytes to file.
	/// </summary>
	public static void SaveBytesToFile(string path, string fileName, byte[] data, bool log = false)
	{
		string fullPath = Path.Combine(path, fileName + ".bytes");

		using (BinaryWriter writer = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
		{
			writer.Write(data);
		}

		if (log)
		{
			Debug.Log("Saved data to: " + fullPath);
		}
	}

	public static void SaveTextToFile(string path, string fileName, string fileExtension, string data, bool log = false)
	{
		if (fileExtension[0] != '.')
		{
			fileExtension = "." + fileExtension;
		}

		string fullPath = Path.Combine(path, fileName + fileExtension);

		SaveTextToFile(fullPath, data, log);
	}


	public static void SaveTextToFile(string fullPath, string data, bool log = false)
	{

		using (var writer = new StreamWriter(File.Open(fullPath, FileMode.Create)))
		{
			writer.Write(data);
		}

		if (log)
		{
			Debug.Log("Saved data to: " + fullPath);
		}
	}

    public static void SaveRenderTextureToJPG(string filePath, RenderTexture texture, uint height, uint width, int jpgQuality = 95)
    {
        SaveRenderTextureToImage(filePath, texture, height, width, SaveTextureFileFormat.JPG, jpgQuality);
    }
    public static void SaveRenderTextureToPNG(string filePath, RenderTexture texture, uint height, uint width)
    {
        SaveRenderTextureToImage(filePath, texture, height, width, SaveTextureFileFormat.PNG);
    }

    public static void SaveRenderTextureToImage(string filePath, RenderTexture texture, uint height, uint width, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.JPG, int jpgQuality = 95)
    {
        // create a native array to receive data from the GPU:
        var narray = new NativeArray<short>((int)(width * height * 4), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        // request the texture data back from the GPU:
        var request = AsyncGPUReadback.RequestIntoNativeArray(ref narray, texture, 0, (AsyncGPUReadbackRequest request) =>
        {
            // if the readback was successful, encode and write the results to disk
            if (!request.hasError)
            {
                NativeArray<byte> encoded;

                switch (fileFormat)
                {
                    case SaveTextureFileFormat.EXR:
                        encoded = ImageConversion.EncodeNativeArrayToEXR(narray, texture.graphicsFormat, (uint)width, (uint)height);
                        break;
                    case SaveTextureFileFormat.JPG:
                        encoded = ImageConversion.EncodeNativeArrayToJPG(narray, texture.graphicsFormat, (uint)width, (uint)height, 0, jpgQuality);
                        break;
                    case SaveTextureFileFormat.TGA:
                        encoded = ImageConversion.EncodeNativeArrayToTGA(narray, texture.graphicsFormat, (uint)width, (uint)height);
                        break;
                    default:
                        encoded = ImageConversion.EncodeNativeArrayToPNG(narray, texture.graphicsFormat, (uint)width, (uint)height);
                        break;
                }

                System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                encoded.Dispose();
            }

            narray.Dispose();
        });
    }
}
