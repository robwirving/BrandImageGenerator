using System;
using System.Configuration;
using Hammock;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using Newtonsoft.Json.Linq;

namespace BrandImageGenerator
{
    public class TwitterAvatarLookup 
    {
        //const string ConsumerKey = "OXEht656fm8Uew720qlfIyc5y";
        //const string ConsumerSecret = "XFuDUnnT0EkmUdS2JIxZijBAkoMysgL5ywTq03HQWZzOWyhZUP";
        //const string Token = "14737082-jjUPRDkfaqSPT8KQfnq2LHsBW1A9O9RmtmM1lBECa";
        //const string TokenSecret = "r0vdZaGGmsH76qf0W8XaEHLlWzLX9vJS4aTegSCi8XQ5B";

        readonly string _consumerKey;
        readonly string _consumerSecret;
        readonly string _token;
        readonly string _tokenSecret;

        public TwitterAvatarLookup()
        {
            _consumerKey = ConfigurationManager.AppSettings.Get("TwitterConsumerKey");
            _consumerSecret = ConfigurationManager.AppSettings.Get("TwitterConsumerSecret");
            _token = ConfigurationManager.AppSettings.Get("TwitterToken");
            _tokenSecret = ConfigurationManager.AppSettings.Get("TwitterTokenSecret");
        }

        public string GetTwitterAvatarUrl(string twitterHandle)
        {
            string avatarUrl;
            var request = new RestRequest
            {
                Credentials = new OAuthCredentials
                {
                    Type = OAuthType.ProtectedResource,
                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                    ConsumerKey = _consumerKey,
                    ConsumerSecret = _consumerSecret,
                    Token = _token,
                    TokenSecret = _tokenSecret,
                },
                Path = string.Format("https://api.twitter.com/1.1/users/lookup.json?screen_name={0}&include_entities=0&include_rts=0", twitterHandle),
                Method = WebMethod.Get
            };

            var client = new RestClient();
            try
            {
                var response = client.Request(request);
                var jArray = JArray.Parse(response.Content);
                avatarUrl = (string)jArray[0]["profile_image_url_https"];
            }
            catch (Exception)
            {
                return "default.png";
            }
            return avatarUrl;
        }

    }
}