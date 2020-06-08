﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;

namespace ExactlyOnce.AzureFunctions
{
    [Extension("ExactlyOnce")]
    public class ExactlyOnceExtensions : IExtensionConfigProvider
    {
        IConfiguration configuration;
        MessageSender sender;
        MessageSender messageSender;

        public ExactlyOnceExtensions(IConfiguration configuration, MessageSender sender)
        {
            this.configuration = configuration;
            this.sender = sender;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<ExactlyOnceResponseAttribute>();            
 
            rule.BindToCollector<ResponseCollectorOpenType>(typeof(ResponseCollectorConverter<>), this);

            messageSender = sender;
        }
 
        class ResponseCollectorOpenType : OpenType.Poco
        {
            public override bool IsMatch(Type type, OpenTypeMatchContext context)
            {
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return false;
                }

                if (type.FullName == "System.Object")
                {
                    return true;
                }

                return base.IsMatch(type, context);
            }
        }

        internal ResponseCollectorContext CreateContext(ExactlyOnceResponseAttribute attribute)
        {
            return new ResponseCollectorContext
            {
                ResolvedAttribute = attribute,
                Sender = messageSender
            };
        }
    }
}