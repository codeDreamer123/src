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
namespace MassTransit.MongoDbIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using MessageData;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;
    using NUnit.Framework;


    [TestFixture]
    public class MongoMessageDataRepositoryTestsForPuttingMessageData
    {
        [Test]
        public async Task ThenExpirationIsNotSet()
        {
            IAsyncCursor<GridFSFileInfo> cursor = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, _filename));
            List<GridFSFileInfo> list = await cursor.ToListAsync();
            var doc = list.Single();

            Assert.That(doc.Metadata.Contains("expiration"), Is.False);
        }

        [Test]
        public async Task ThenMessageStoredAsExpected()
        {
            byte[] result = await _bucket.DownloadAsBytesByNameAsync(_filename);

            Assert.That(result, Is.EqualTo(_expectedData));
        }

        [OneTimeSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            _expectedData = Encoding.UTF8.GetBytes("This is a test message data block");

            _resolver = new MessageDataResolver();
            _nameCreator = new StaticFileNameGenerator();
            _filename = _nameCreator.GenerateFileName();

            IMessageDataRepository repository = new MongoDbMessageDataRepository(_resolver, _bucket, _nameCreator);

            using (var stream = new MemoryStream(_expectedData))
            {
                _actualUri = repository.Put(stream).GetAwaiter().GetResult();

            }
        }

        [OneTimeTearDown]
        public void Kill()
        {
            _bucket.DropAsync().GetAwaiter().GetResult();
        }

        GridFSBucket _bucket;
        byte[] _expectedData;
        IFileNameGenerator _nameCreator;
        IMessageDataResolver _resolver;
        Uri _actualUri;
        string _filename;


        class StaticFileNameGenerator :
            IFileNameGenerator
        {
            public string GenerateFileName()
            {
                return "etdpgnbhbtdgjs8h1pzcbxyyyy";
            }
        }
    }
}