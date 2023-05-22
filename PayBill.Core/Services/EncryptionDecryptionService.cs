using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayBill.Core.Models;
using Polly;

namespace PayBill.Core.Services;

public class EncryptionDecryptionService : IEncryptionDecryptionService
{
    private readonly HttpClient _client = new();
    private readonly string _decryptorUrl;
    private readonly string _encryptorUrl;

    public EncryptionDecryptionService(IConfiguration config)
    {
        _client.Timeout = TimeSpan.FromSeconds(30);
        _encryptorUrl = config["Ekpay:EncryptorUrl"]!;
        _decryptorUrl = config["Ekpay:DecryptorUrl"]!;
    }

    #region "Encryptor"

    public async Task<string> Encrypt(EncryptionDecryptionModel model)
    {
        var requestData = JsonConvert.SerializeObject(new
        {
            JSON_String = model.EncodedData
        });

        // Define the retry policy
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(2, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Execute the API call with the retry policy
        return await retryPolicy.ExecuteAsync(async () =>
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _encryptorUrl))
            {
                request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");
                using (var response = await _client.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    dynamic decryptedData = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return decryptedData.resultOutput.ToString();
                }
            }
        }).ConfigureAwait(false);
    }

    #endregion

    #region "Decryptor"

    public async Task<string> Decrypt(EncryptionDecryptionModel model)
    {
        var requestData = JsonConvert.SerializeObject(new
        {
            encrypted_String = model
        });

        // Define the retry policy
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(2, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Execute the API call with the retry policy
        return await retryPolicy.ExecuteAsync(async () =>
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _decryptorUrl))
            {
                request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");
                using (var response = await _client.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    dynamic decryptedData = JObject.Parse(await response.Content.ReadAsStringAsync());
                    string result = decryptedData.resultOutput.ToString();
                    return Encoding.UTF8.GetString(Convert.FromBase64String(result));
                }
            }
        }).ConfigureAwait(false);
    }

    #endregion
}