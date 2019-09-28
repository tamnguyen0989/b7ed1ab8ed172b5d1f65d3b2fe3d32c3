using Newtonsoft.Json;
using SoftBBM.Web.Enum;
using SoftBBM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SoftBBM.Web.DAL.Repositories
{
    public interface IShopeeRepository
    {
        string update_stock(int item_id, int stock, int partner_id, int shopid);
    }
    public class ShopeeRepository : IShopeeRepository
    {
        ISoftChannelRepository _softChannelRepository;
        int _apiId;
        string _apiPassowrd;
        int _apiPartnerId;
        bool _validApi;


        public ShopeeRepository(ISoftChannelRepository softChannelRepository)
        {
            _softChannelRepository = softChannelRepository;

            _apiId = 1928354;
            _apiPassowrd = "64e256a952a7d79e9c0d09cd2075b8249b6cbba27586a4238e2733af28e26266";
            _apiPartnerId = 842214;

            //_validApi = false;
            //var channel = _softChannelRepository.GetSingleById((int)ChannelEnum.SPE);
            //var apiPartnerId = 0;
            //var apiId = 0;
            //int.TryParse(channel.ApiId, out apiId);
            //int.TryParse(channel.ApiPartnerId, out apiPartnerId);

            //var result = authenShopee(apiId, channel.ApiPassword, apiPartnerId);
            //if(!string.IsNullOrEmpty(result))
            //{
            //    var resultJson = JsonConvert.DeserializeObject<ShopInfo>(result);
            //    if (resultJson.status == "NORMAL" && channel.Enabled == true)
            //    {
            //        _apiId = apiId;
            //        _apiPassowrd = channel.ApiPassword;
            //        _apiPartnerId = apiPartnerId;
            //        _validApi = true;
            //    }
            //}
        }

        public string authenShopee(int apiId, string apiPassword, int apiPartnerId)
        {
            var jsonStrResult = "";
            //var channel = _softChannelRepository.GetSingleById((int)ChannelEnum.SPE);
            var timeStamp = getTimestamp();
            string url = "https://partner.shopeemobile.com/api/v1/shop/get";
            string dataJson = "{'partner_id':" + apiPartnerId + "," +
                              "'shopid':" + apiId + "," +
                              "'timestamp':" + timeStamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            string signatureBaseString = url + '|' + dataJson;
            string signatureAuth = createSignature(apiPassword, signatureBaseString);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", signatureAuth);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataJson);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.Default, true))
            {
                jsonStrResult = streamReader.ReadToEnd();
            }

            return jsonStrResult;
        }

        public int getTimestamp()
        {
            var unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        //key=password, data = dataJson
        public string createSignature(string key, string data)
        {
            var result = "";
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(data);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return result;
        }

        public string postRequest(string url, string dataJson)
        {
            var jsonStrResult = "";
            var timeStamp = getTimestamp();
            string signatureBaseString = url + '|' + dataJson;
            _apiPassowrd = "64e256a952a7d79e9c0d09cd2075b8249b6cbba27586a4238e2733af28e26266";

            string signatureAuth = createSignature(_apiPassowrd, signatureBaseString);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", signatureAuth);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataJson);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.Default, true))
            {
                jsonStrResult = streamReader.ReadToEnd();
            }

            return jsonStrResult;
        }

        public string update_stock(int item_id, int stock, int partner_id, int shopid)
        {
            var timestamp = getTimestamp();
            var result = "";
            var url= "https://partner.shopeemobile.com/api/v1/items/update_stock";
            string dataJson = "{'item_id':" + item_id + "," +
                              "'stock':" + stock + "," +
                              "'partner_id':" + partner_id + "," +
                              "'shopid':" + shopid + "," +
                              "'timestamp':" + timestamp + "}";
            dataJson = dataJson.Replace("'", "\"");
            result = postRequest(url, dataJson);
            return result;
        }

    }
}