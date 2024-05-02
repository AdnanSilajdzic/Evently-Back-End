using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

public class Backblaze
{
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _bucketName;
    private readonly string _endpoint;

    public Backblaze()
    {
        _accessKey = Environment.GetEnvironmentVariable("AppKey");
        _secretKey = Environment.GetEnvironmentVariable("KeyId");
        _bucketName = "evently-images";
        _endpoint = "https://s3.eu-central-003.backblazeb2.com";
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile, string keyName)
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                // Copy the image file to the memory stream
                await imageFile.CopyToAsync(memoryStream);

                // Upload the image to S3
                using (var client = new AmazonS3Client(_accessKey, _secretKey, new AmazonS3Config
                {
                    ServiceURL = _endpoint,
                    ForcePathStyle = true // Set this to true for custom endpoints like Backblaze B2
                }))
                {
                    var fileTransferUtility = new TransferUtility(client);
                    memoryStream.Position = 0; // Reset the memory stream position
                    await fileTransferUtility.UploadAsync(memoryStream, _bucketName, keyName);
                }

                // Construct and return the URL of the uploaded image
                return $"{_endpoint}/{_bucketName}/{keyName}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading image: {ex.Message}");
            return null; // Handle error appropriately in your application
        }
    }

    }
