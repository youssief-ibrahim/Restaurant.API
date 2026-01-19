using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;


namespace Restaurant.Infrastructure.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer();
        }
        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer();
        }
    }

}
