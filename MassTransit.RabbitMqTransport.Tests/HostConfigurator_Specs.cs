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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Security.Authentication;
    using Configurators;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class HostConfigurator_Specs
    {
        [Test]
        public void Should_set_assigned_ssl_protocol()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.Protocol = SslProtocols.Tls12;
            });

            configurator.Settings.SslProtocol.ShouldBe(SslProtocols.Tls12);
        }

        [Test, Description("Default ssl protocol should be tls 1.0 for back compatibility reason")]
        public void Should_set_tls10_protocol_by_default()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator => { });

            configurator.Settings.SslProtocol.ShouldBe(SslProtocols.Tls);
        }

        [Test, Description("Default SSL settings should not use client certificate as authentication identity")]
        public void Should_set_client_certificate_as_authentication_identity_as_false_by_default()
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator => { });

            configurator.Settings.UseClientCertificateAsAuthenticationIdentity.ShouldBeFalse();
        }

        [TestCase(true), TestCase(false), Description("SSL settings should set use client certificate as authentication identity as specified by configuration")]
        public void Should_set_client_certificate_as_authentication_identity_when_configured(bool valueToSet)
        {
            var configurator = new RabbitMqHostConfigurator(new Uri("rabbitmq://localhost"));

            configurator.UseSsl(sslConfigurator =>
            {
                sslConfigurator.UseCertificateAsAuthenticationIdentity = valueToSet;
            });

            configurator.Settings.UseClientCertificateAsAuthenticationIdentity.ShouldBe(valueToSet);
        }
    }
}