﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;

namespace FantasyData
{
    public class FantasyDataApiBase
    {
        private const string JSON = "JSON";

        #region Properties and Accessors

        protected string BaseUrl { get; private set; }
        protected string PrimarySubscriptionKey { get; private set; }
        protected string SecondarySubscriptionKey { get; private set; }

        private static string _userAgent;

        private static string UserAgent
        {
            get
            {
                if (_userAgent == null)
                {
                    _userAgent = String.Format("FantasyData.NET RestSharp Client v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                }
                return _userAgent;
            }
        }

        private RestClient _client;

        #endregion

        #region Constructors

        public FantasyDataApiBase(string baseUrl, string primarySubscriptionKey)
        {
            if (string.IsNullOrEmpty(primarySubscriptionKey))
                throw new ArgumentException("Primary Subscription Key is required.");

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Api Base Url is required.");

            PrimarySubscriptionKey = primarySubscriptionKey;

            if (baseUrl.EndsWith("/"))
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            BaseUrl = baseUrl;

            _client = new RestClient();
            _client.UserAgent = UserAgent;
            _client.BaseUrl = Config.ApiBaseUrl;
        }


        public FantasyDataApiBase(string baseUrl, string primarySubscriptionKey, string secondarySubscriptionKey)
            : this(baseUrl, primarySubscriptionKey)
        {
            SecondarySubscriptionKey = secondarySubscriptionKey;
        }

        #endregion

        public Task<T> GetRequestAsync<T>(string path, params object[] args) where T : new()
        {
            return GetRequestAsync<T>(path, string.Empty, args);
        }

        public T GetRequest<T>(string path, params object[] args) where T : new()
        {
            return GetRequest<T>(path, string.Empty, args);
        }

        public T GetRequest<T>(string path, string rootElement, params object[] args) where T : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            InitializeRequest(request);

            var response = _client.Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FantasyDataException("Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new FantasyDataException("Internal Server Error");
            }

            return response.Data;
        }

        public Task<T> GetRequestAsync<T>(string path, string rootElement, params object[] args) where T : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            InitializeRequest(request);

            TaskCompletionSource<T> result = new TaskCompletionSource<T>();

            _client.ExecuteAsync<T>(request, response =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new FantasyDataException("Not Found");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new FantasyDataException("Internal Server Error");
                }

                result.SetResult(response.Data);
            });

            return result.Task;
        }

        protected int GetInt(string path, params object[] args)
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            InitializeRequest(request);

            var response = _client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FantasyDataException("Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new FantasyDataException("Internal Server Error");
            }

            return Convert.ToInt32(response.Content);
        }

        protected bool GetBool(string path, params object[] args)
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            InitializeRequest(request);

            var response = _client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FantasyDataException("Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new FantasyDataException("Internal Server Error");
            }

            return Convert.ToBoolean(response.Content);
        }

        public string BuildUrl(string path, params object[] args)
        {
            var formatPart = string.Format("/{0}", JSON);

            return formatPart + string.Format(path, args);
        }


        public void InitializeRequest(RestRequest request)
        {
            request.AddHeader("Ocp-Apim-Subscription-Key", GetSubscriptionKey());
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new JsonSerializer();
        }

        public string GetSubscriptionKey()
        {
            if (!string.IsNullOrEmpty(PrimarySubscriptionKey))
                return PrimarySubscriptionKey;
            else if (!string.IsNullOrEmpty(SecondarySubscriptionKey))
                return SecondarySubscriptionKey;

            throw new FantasyDataException("No subscription key set.");
        }

        public virtual string ResourceType
        {
            get { return "stats"; }
        }
    }
}
