using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Sinder
{
    /// <summary>
    /// Web related helpers
    /// </summary>
    public static class WebHelper
    {
        /// <summary>
        /// Combining Uri
        /// </summary>
        public static Uri CombineUri(string baseUri, string relativeOrAbsoluteUri)
        {
            return new Uri(new Uri(baseUri), relativeOrAbsoluteUri);
        }

        /// <summary>
        /// Combining URL paths
        /// </summary>
        public static string CombineUriToString(string baseUri, string relativeOrAbsoluteUri)
        {
            return new Uri(new Uri(baseUri), relativeOrAbsoluteUri).ToString();
        }

        /// <summary>
        /// Web requests to external adresses
        /// </summary>
        public static async Task<ImageModel> UploadProfileImage(string uploadedFilePath)
        {
            JObject jObject;
            // target URL
            string url = "http://api.jkb.zone/file";
            // Client initiated
            HttpClient httpClient = new HttpClient();
            // Form containing data
            MultipartFormDataContent form = new MultipartFormDataContent();

            // Some magic with the file
            byte[] file = System.IO.File.ReadAllBytes(uploadedFilePath);

            form.Add(new ByteArrayContent(file, 0, file.Length), "file", uploadedFilePath);
            form.Add(new StringContent("false"), "cover");
            //// Sorry attempt at security
            form.Add(new StringContent("ijfgiouahfbnuivboaefh"), "upload_preset");

            // POST method
            HttpResponseMessage response = await httpClient.PostAsync(url, form);

            // Parse the response into string
            string responseBody = await response.Content.ReadAsStringAsync();

            jObject = JObject.Parse(responseBody);
            ImageModel imageModel = new ImageModel();

            string rootUrl = "http://api.jkb.zone/file/";
            string urlPath = jObject.Value<string>("location");
            string queryParameter = "?display";

            //"1600258795.27e987d9f81d552cca0ae013a2eecb7a586c80cb.dm2jSBk.png"
            // => "http://api.jkb.zone/file/27e987d9f81d552cca0ae013a2eecb7a586c80cb.png?display"
            List<string> parts = urlPath.Split('.').ToList();
            urlPath = parts[1] + "." + parts.Last();

            string fullUrl = CombineUriToString(rootUrl, urlPath + queryParameter);

            imageModel.Url = fullUrl;

            // Murder the variable
            httpClient.Dispose();

            return imageModel;
        }
    }
}
