// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;

namespace Sheet.Editor
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object value)
        {
            var settings = new JsonSerializerSettings() 
            { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }

        public T Deerialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
