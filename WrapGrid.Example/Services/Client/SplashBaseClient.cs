using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WrapGrid.Example.Services.Model;

namespace WrapGrid.Example.Services.Client
{
    class SplashBaseClient
    {
        private HttpClient client;

        public SplashBaseClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://www.splashbase.co/api/v1/images/");
        }

        public async Task<IEnumerable<ImageModel>> GetImagesAsync(int offset, int elementsPerRequest = 10)
        {
            List<ImageModel> result = new List<ImageModel>();

            //we can't make 10 tasks and wait for them because server will return 404
            for (int i = offset; result.Count < elementsPerRequest; i++)
            {
                var item = await GetImageAsync(i).ConfigureAwait(false);
                
                if(item != null)
                {
                    result.Add(item);
                }
                
                await Task.Delay(10).ConfigureAwait(false);
            }

            return result;
        }

        private async Task<ImageModel> GetImageAsync(int index)
        {
            try
            {
                var json = await client.GetStringAsync(index.ToString()).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ImageModel>(json);
            }
            catch (Exception)
            {
                //we dont care
                return null;
            }
           
        }
    }
}
