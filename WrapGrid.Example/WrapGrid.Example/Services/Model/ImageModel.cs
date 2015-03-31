using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapGrid.Example.Services.Model
{
    public class ImageModel
    {
        public int Id { get; set; }

        public Uri Url { get; set; }

        public string Copyright { get; set; }

        public string Site { get; set; }
    }
}
