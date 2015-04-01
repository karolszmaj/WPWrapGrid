using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrapGrid.Example.Services.Model;

namespace WrapGrid.Example.Services.Client
{
    public class SplashBaseCachedClient
    {
        public async Task<IEnumerable<ImageModel>> GetImagesAsync(int offset, int elementsPerRequest = 10)
        {
            List<ImageModel> result = new List<ImageModel>();

            var resource = App.GetResourceStream(new Uri("Assets/data.json", UriKind.Relative));

            using (StreamReader reader = new StreamReader(resource.Stream))
            {
                var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<IEnumerable<ImageModel>>(json);
                result = new List<ImageModel>(data);
            }

            //JsonConvert.DeserializeObject<IEnumerable<ImageModel>>()

            return result.Skip(offset).Take(elementsPerRequest);
        }
    }
}
