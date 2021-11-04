using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace BlogedWebapp.Helpers
{
    /// <summary>
    ///  Class for handling app secrets
    /// </summary>
    public class SecretsManager
    {


        /// <summary>
        ///  Read a specific secret
        /// </summary>
        /// <param name="secretName">Secret name</param>
        /// <param name="key">Read value of the specified key. If not provided, returns the whole secrete as plaintext.</param>
        /// <returns>A specified value if key parameter is specified, otherwise it will return the whole object as plaintext</returns>
        public static string GetSecret(string secretName, string key = "")
        {
            //string secretName = "prod/Bloged/secrets";
            string region = "eu-central-1";
            string secret = "";

            MemoryStream memoryStream = new MemoryStream();

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT";

            GetSecretValueResponse response = null;

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                throw new CannotFetchSecretsException(e.Message);
            }
            catch (InternalServiceErrorException e)
            {
                throw new CannotFetchSecretsException(e.Message);
            }
            catch (InvalidParameterException e)
            {
                throw new CannotFetchSecretsException(e.Message);
            }
            catch (InvalidRequestException e)
            {
                throw new CannotFetchSecretsException(e.Message);
            }
            catch (ResourceNotFoundException e)
            {
                throw new CannotFetchSecretsException(e.Message);
            }
            catch (System.AggregateException ae)
            {
                throw new CannotFetchSecretsException(ae.Message);
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            //if key is not set return thw whole secret as plaintext
            if ( key.Length == 0 )
            {
                return secret;
            }


            //returns a specific value, based on key
            JObject secretObject = JObject.Parse(secret);
            if (secretObject.ContainsKey(key))
            {
                return (string)secretObject[key];
            } else
            {
                throw new KeyNotFoundException(key);
            }
        }
    }

    /// <summary>
    ///  Cannot fetch secrets from AWS Secrets Manager
    /// </summary>
    public class CannotFetchSecretsException : Exception
    {
        public CannotFetchSecretsException(string message) : base(message)
        {

        }
    }

    /// <summary>
    ///  Cannot read key
    /// </summary>
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string key) : base($"Cannot read key \"{key}\".") { }
    }
}
