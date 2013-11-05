﻿using System;
using System.IO;
using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class CollectionTests
	{
		private Library _library;
		private Database _dataBase;
		private Collection _collection;
		private BSONDocument _origin;
		private const string DbName = "test.db";

		[SetUp]
		public void Setup()
		{
			if (File.Exists(DbName))
			{
				File.Delete(DbName);
			}
			_library = Library.Create();

			_dataBase = _library.CreateDatabase();

			_dataBase.Open(DbName);

			_collection = _dataBase.CreateCollection("default", new CollectionOptions());

			_origin = BSONDocument.ValueOf(new
				{
					name = "Grenny",
					type = "African Grey",
					male = true,
					age = 1,
					birthdate = DateTime.Now,
					likes = new[] { "green color", "night", "toys" },
					extra = BSONull.VALUE
				});
		}

		[TearDown]
		public void TearDown()
		{
			_dataBase.Dispose();
			_library.Dispose();
		}

		[Test]
		public void Can_begin_and_commit_transaction()
		{
			_collection.BeginTransaction();

			var isActive = _collection.TransactionActive;

			_collection.CommitTransaction();

			var notActiveTransaction = !_collection.TransactionActive;

			Assert.That(isActive, Is.True, "Transaction should be active after begin");
			Assert.That(notActiveTransaction, Is.True, "Transaction should be active after commit");
		}

		[Test]
		public void Can_synchronize_collection()
		{
			_collection.Synchronize();

			//TODO: Assert something?
		}

		[Test]
		public void Can_save_and_load_document()
		{
			_collection.Save(_origin, false);

			var id = _origin.GetBSONValue("_id");

			var reloaded = _collection.Load((BSONOid)id.Value);
			//TODO: made more string assertion
			Assert.That(reloaded, Is.Not.Null);
		}


		[Test]
		public void Can_delete_document()
		{
			_collection.Save(_origin, false);
			var id = _origin.GetBSONValue("_id");
			var bsonOid = (BSONOid)id.Value;
			
			_collection.Delete(bsonOid);

			var reloaded = _collection.Load(bsonOid);
			
			Assert.That(reloaded, Is.Not.Null);
		}
	}
}