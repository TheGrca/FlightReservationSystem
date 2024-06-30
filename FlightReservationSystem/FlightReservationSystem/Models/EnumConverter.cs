using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightReservationSystem.Models
{
    //Helper class for converting enums as strings, instead of index numbers
    public class EnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Enum)
            {
                // Serialize the enum value as string
                writer.WriteValue(value.ToString());
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }
}
