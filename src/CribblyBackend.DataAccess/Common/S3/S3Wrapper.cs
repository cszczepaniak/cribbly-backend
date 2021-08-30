using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace CribblyBackend.DataAccess.Common.S3
{
    public interface IS3Wrapper
    {
        Task<PutObjectResponse> PutObjectAsync<T>(string key, T obj);
        Task<(T, bool)> GetObjectAsync<T>(string key);
    }

    public class S3Wrapper : IS3Wrapper
    {
        private readonly string _bucket;

        public S3Wrapper(string bucket)
        {
            _bucket = bucket;
            _s3 = new AmazonS3Client();
        }

        private readonly IAmazonS3 _s3;

        public async Task<(T, bool)> GetObjectAsync<T>(string key)
        {
            try
            {
                var content = await _s3.GetObjectAsync(_bucket, key);
                var obj = await JsonSerializer.DeserializeAsync<T>(content.ResponseStream);
                return (obj, true);
            }
            catch (AmazonS3Exception e) when (e.Message == "The specified key does not exist")
            {
                return (default(T), false);
            }
        }

        public async Task<PutObjectResponse> PutObjectAsync<T>(string key, T obj)
        {
            var req = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                ContentType = "application/json",
            };

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, obj);
            req.InputStream = stream;
            return await _s3.PutObjectAsync(req);
        }
    }
}