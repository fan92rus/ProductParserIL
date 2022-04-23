using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace ProductParserIL.Helpers
{
    public class RestClientFactory
    {
        public RestClientFactory(IProxyManager proxyManager)
        {
            this.ProxyManager = proxyManager;
        }

        public IProxyManager ProxyManager { get; set; }

        public RestClient CreateRc(string url, bool useProxy)
        {
            var restClient = new RestClient() { MaxRedirects = 30, Timeout = 40000 };

            if (!string.IsNullOrEmpty(url))
                restClient.BaseUrl = new Uri(url);

            restClient.ConfigureWebRequest((r) =>
            {
                r.ServicePoint.Expect100Continue = false;
                r.ServicePoint.MaxIdleTime = 0;
            });

            restClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36";
            if (useProxy)
                restClient.Proxy = this.ProxyManager.Get();

            return restClient;
        }

        public RestClient CreateRc(Uri uri, bool useProxy) => CreateRc($"{uri.Scheme}://{uri.Host}", useProxy);
    }

    public interface IProxyManager
    {
        IWebProxy Get();
    }

    public static class RestExtension
    {
        private static ILogger logger { get; }


        /// <summary>
        /// Приведение обьекта к валиднаму для обработки на основе PropertyInfo
        /// </summary>
        /// <param name="property">Информация о свойстве</param>
        /// <param name="value">Значение</param>
        /// <returns>Нормализованое значение</returns>
        private static object NormalizeData(PropertyInfo property, object value)
        {
            try
            {
                if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                {
                    if ((bool)value)
                    {
                        return 1;
                    }

                    return 0;
                }

                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    var date = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                    return date;
                }

                if (property.PropertyType.BaseType == typeof(Enum))
                {
                    return (int)value;
                }

                return value;
            }
            catch (Exception e)
            {
                logger.LogError("", e);
                throw;
            }
        }

        /// <summary>
        /// Выполнение запроса с использованием полити запросов Policy
        /// </summary>
        /// <typeparam name="T">Тип обьекта для десериализации обьекта ответа</typeparam>
        /// <param name="client">Клиент для отправки запроса</param>
        /// <param name="request">Сформиованый запрос для отправки на сервер</param>
        /// <param name="policy">Политика обработки ответа и повторения запроса</param>
        /// <returns>Полученый ответ с десериализованым обьектом</returns>
        public static IRestResponse<T> ExecuteWithPolicy<T>(this IRestClient client, RestRequest request, Policy<IRestResponse<T>> policy)
        {
            var val = policy.ExecuteAndCapture(() => client.Execute<T>(request));

            return val.Result ?? new RestResponse<T> { Request = request, ErrorException = val.FinalException };
        }

        /// <summary>
        /// Выполнение запроса с использованием полити запросов Policy
        /// </summary>
        /// <param name="client">Клиент для отправки запроса</param>
        /// <param name="request">Сформиованый запрос для отправки на сервер</param>
        /// <param name="policy">Политика обработки ответа и повторения запроса</param>
        /// <returns>Ответ сервера</returns>
        public static IRestResponse ExecuteWithPolicy(this IRestClient client, RestRequest request, Policy<IRestResponse> policy)
        {
            var val = policy.ExecuteAndCapture(() => client.Execute(request));

            return val.Result ?? new RestResponse { Request = request, ErrorException = val.FinalException };
        }

        public static IRestResponse ExecuteWitHeaders(this IRestClient client, RestRequest request, Policy<IRestResponse> policy)
        {
            var val = policy.ExecuteAndCapture(() => client.ExecuteWitHeaders(request));

            return val.Result ?? new RestResponse { Request = request, ErrorException = val.FinalException };
        }

        public static async Task<IRestResponse> ExecuteWitHeadersAsync(this IRestClient client, RestRequest request)
        {
            bool CheckHeader(RestRequest req, string header) => req.Parameters.All(x => x.Name != header);
            try
            {
                while (true)
                {
                    client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    if (CheckHeader(request, "Accept"))
                    {
                        request.AddHeader("Accept", "application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        request.AddHeader("Accept-Language", "en-US;q=0.8,en;q=0.7");
                        request.AddHeader("Connection", "keep-alive");
                        request.AddHeader("Accept-Encoding", "gzip, deflate");
                        request.AddHeader("TE", "Trailers");
                        var uriCreated = Uri.TryCreate(request.Resource, UriKind.Absolute, out var uri);
                        if (!uriCreated)
                        {
                            uri = client.BaseUrl;
                        }
                        else
                            request.AddHeader("Host", uri.Host);
                    }
                    else
                    {
                        request.Parameters.RemoveAll(x => x.Name == "X-Requested-With");
                    }

                    var resp = await client.ExecuteAsync(request);

                    if (resp.StatusCode != 0) return resp;

                    if (CheckHeader(request, "X-Requested-With"))
                        request.AddHeader("X-Requested-With", "XMLHttpRequest");

                    resp = await client.ExecuteAsync(request);

                    return resp;
                }
            }
            catch (Exception e)
            {
                logger.LogError("", e);
                throw e;
            }
        }

        public static IRestResponse ExecuteWitHeaders(this IRestClient client, RestRequest request)
        {
            var task = client.ExecuteWitHeadersAsync(request);
            task.Wait();
            return task.Result;
        }
        public static IRestResponse ExecuteWithRedirects(this IRestClient client, RestRequest request, Policy<IRestResponse> policy)
        {
            return ExecuteWithRedirects(client, request, policy, false);
        }
        public static IRestResponse ExecuteWithRedirects(this IRestClient client, RestRequest request, Policy<IRestResponse> policy, bool ignoreNextRedirects)
        {
            try
            {
                var lastLocation = client.BuildUri(request).AbsoluteUri;

                while (true)
                {
                    var resp = client.ExecuteWitHeaders(request, policy);
                    resp.Content = null;

                    var location = GetUrlFromResp(resp) ?? resp?.ResponseUri?.AbsoluteUri;

                    var isCreated = Uri.TryCreate(location, UriKind.Absolute, out var newLocation);

                    if (isCreated && lastLocation != location)
                    {
                        lastLocation = newLocation.AbsoluteUri;
                        request = new RestRequest(newLocation);
                    }
                    else if (!ignoreNextRedirects && isCreated)
                    {
                        var req = new RestRequest(resp.ResponseUri);
                        var redirect = ExecuteWithRedirects(client, req, policy, true);
                        if (redirect.ResponseUri.Host == resp.ResponseUri.Host)
                            return redirect;
                    }
                    else
                        return resp;
                }
            }
            catch (Exception e)
            {
                logger.LogError("", e);
                throw;
            }
        }

        private static string GetUrlFromResp(IRestResponse response)
        {
            try
            {
                if (response?.ResponseUri == null)
                    return null;

                var location = response.Headers.FirstOrDefault(x => x.Name == "Location")?.Value?.ToString();
                if (!string.IsNullOrEmpty(location))
                    return location;

                var urlExpr = "(https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|www\\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9]+\\.[^\\s]{2,}|www\\.[a-zA-Z0-9]+\\.[^\\s]{2,})";
                var decoded = HttpUtility.ParseQueryString(response.ResponseUri.PathAndQuery);
                var url = decoded.AllKeys.Where(x => x != null).FirstOrDefault(x => Regex.IsMatch(x, urlExpr));

                var jsredirectURL = Regex.Unescape(Regex.Match(response.Content, "(?<=window.location.replace\\(')(.+)(?= ')").Value);

                if (!string.IsNullOrEmpty(url))
                    return Regex.Match(url, urlExpr)?.Value;

                if (!string.IsNullOrEmpty(jsredirectURL))
                    return jsredirectURL;

                return null;
            }
            catch (Exception e)
            {
                logger.LogError("", e);
                return null;
            }
        }
    }
}