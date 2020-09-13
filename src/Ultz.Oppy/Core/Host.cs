﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ultz.Extensions.PrivacyEnhancedMail;
using Ultz.Oppy.Configuration;
using Ultz.Oppy.Content;

namespace Ultz.Oppy.Core
{
    public class Host
    {
        public Host(in HostInfo info)
        {
            Listeners = new Dictionary<ushort, (SslProtocols, HttpProtocols, Dictionary<string, X509Certificate2>)>();
            foreach (var listener in info.Listeners)
            {
                var ssl = SslProtocols.None;
                var protos = HttpProtocols.None;
                foreach (var protocol in listener.Protocols ?? Enumerable.Empty<string>())
                {
                    switch (protocol.Trim().Replace(".", null).ToLower())
                    {
                        case "tls13":
                        {
                            ssl |= SslProtocols.Tls13;
                            break;
                        }
                        case "tls12":
                        {
                            ssl |= SslProtocols.Tls12;
                            break;
                        }
                        case "tls11":
                        {
                            ssl |= SslProtocols.Tls11;
                            break;
                        }
                        case "tls10":
                        {
                            ssl |= SslProtocols.Tls;
                            break;
                        }
                        case "http2":
                        {
                            protos |= HttpProtocols.Http2;
                            break;
                        }
                        case "http1":
                        {
                            protos |= HttpProtocols.Http1;
                            break;
                        }
                    }
                }

                Listeners.Add(listener.Port,
                    (ssl, protos,
                        listener.KeyPairs.Select(x => (x.Key.Trim().ToLower(), x.Value))
                            .Where(x => !(x.Value.PemCert is null) && !(x.Value.PemKey is null))
                            .ToDictionary(x => x.Item1, x => Pem.GetCertificate(x.Value.PemCert, x.Value.PemKey))));
            }

            Names = info.ServerNames?.Select(x => x.Trim().ToLower()).ToArray();
            Content = new ContentRegistrar(info.ContentDirectory);
        }

        public ContentRegistrar Content { get; }
        public string[]? Names { get; }

        public Dictionary<ushort, (SslProtocols, HttpProtocols, Dictionary<string, X509Certificate2>)> Listeners
        {
            get;
        }
    }
}