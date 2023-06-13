using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OzMaps.Web.Converters
{
    public static class Utilities
    {
        public static void SkipComments(this Utf8JsonReader reader)
        {
            while (reader.TokenType == JsonTokenType.Comment)
            {
                if (!reader.Read())
                {
                    break;
                }
            }
        }
    }
}
