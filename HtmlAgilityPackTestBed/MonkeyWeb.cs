using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlAgilityPackTestBed
{
    public class MonkeyWeb
    {
        HttpClient _httpClient = null;

        /// <summary>
        /// Default request timeout in seconds.
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// The connection key
        /// </summary>
        public string ConnectionKey { get; set; } = null;

        /// <summary>
        /// Cancelation Token
        /// </summary>
        public CancellationTokenSource CancelationToken { get; set; } = new CancellationTokenSource();

        static Lazy<MonkeyWeb> _lazyWebClient => new Lazy<MonkeyWeb>(() =>
        {
            return new MonkeyWeb();
        });
        public static MonkeyWeb Monkey { get; set; } = _lazyWebClient.Value;


        HttpClient ConnectClient()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(this.Timeout)
                };

                if (!string.IsNullOrWhiteSpace(this.ConnectionKey))
                {
                    var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.ConnectionKey));

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
                }
            }
            return _httpClient;
        }

        #region GET methods
        public async Task<string> GetStringAsync(string urlPath)
        {
            var client = ConnectClient();

            string retItem = "";
            try
            {
                var response = await client.GetAsync(urlPath);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    retItem = content;
                }
                else
                {
                    //Logger.I.Log("ApiServiceController::GetItemByIdAsync " + urlPath + "/" + id + " failed: " + response.StatusCode.ToString());
                }
            }
            catch (HttpRequestException httpException)
            {
                //Logger.I.LogError(httpException);
            }
            catch (Exception ex)
            {
                //Logger.I.LogError(ex);
            }
            return retItem;
        }
        #endregion

        #region POST methods
        #endregion
    }
}
