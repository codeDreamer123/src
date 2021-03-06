﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// A body serializer takes a byte array message body and just streams it out to the message
    /// unmodified.
    /// </summary>
    public class StringMessageSerializer :
        IMessageSerializer
    {
        readonly Encoding _encoding;
        string _body;

        public StringMessageSerializer(ContentType contentType, string body, Encoding encoding = null)
        {
            ContentType = contentType;
            _body = body;
            _encoding = encoding ?? Encoding.UTF8;
        }

        public ContentType ContentType { get; }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            byte[] body = _encoding.GetBytes(_body);

            stream.Write(body, 0, body.Length);
        }

        public void UpdateJsonHeaders(IDictionary<string, object> values)
        {
            var envelope = JObject.Parse(_body);

            var headersToken = envelope["headers"] ?? new JObject();
            var headers = headersToken.ToObject<Dictionary<string, object>>();

            foreach (KeyValuePair<string, object> payloadHeader in values)
            {
                headers[payloadHeader.Key] = payloadHeader.Value;
            }
            envelope["headers"] = JToken.FromObject(headers);

            _body = JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        public void UpdateXmlHeaders(IDictionary<string, object> values)
        {
            using (var reader = new StringReader(_body))
            {
                var document = XDocument.Load(reader);

                var envelope = (from e in document.Descendants("envelope") select e).Single();

                var headers = (from h in envelope.Descendants("headers") select h).SingleOrDefault();
                if (headers == null)
                {
                    headers = new XElement("headers");
                    envelope.Add(headers);
                }


                foreach (KeyValuePair<string, object> payloadHeader in values)
                {
                    headers.Add(new XElement(payloadHeader.Key, payloadHeader.Value));
                }

                _body = document.ToString();
            }
        }
    }
}