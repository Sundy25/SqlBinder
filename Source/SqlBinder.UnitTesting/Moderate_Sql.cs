﻿using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBinder.ConditionValues;

namespace SqlBinder.UnitTesting
{
	[TestClass]
	public partial class SqlBinder_Tests
	{
		/// <summary>
		/// Tests that cover the most common cases, moderately complex Sql.
		/// </summary>
		[TestClass]
		public class Moderate_Sql
		{
			[TestInitialize]
			public void InitializeTest()
			{
				//
			}

			[TestMethod]
			public void CommonSql_1()
			{
				var query = new MockQuery(_connection,"SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]}}");

				query.SetCondition("Criteria1", new BoolValue(true));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = :pCriteria1_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 1);
				Assert.AreEqual("pCriteria1_1", ((IDbDataParameter) cmd.Parameters[0]).ParameterName);
				Assert.AreEqual(true, ((IDbDataParameter) cmd.Parameters[0]).Value);
			}

			[TestMethod]
			public void CommonSql_2()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE{JUNK 1} {COLUMN1 [Criteria1]} {JUNK 2}}");

				query.SetCondition("Criteria1", new BoolValue(true));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = :pCriteria1_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 1);
				Assert.AreEqual("pCriteria1_1", ((IDbDataParameter)cmd.Parameters[0]).ParameterName);
				Assert.AreEqual(true, ((IDbDataParameter)cmd.Parameters[0]).Value);
			}

			[TestMethod]
			public void CommonSql_3()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]} {COLUMN2 [Criteria2]}}");

				query.SetCondition("Criteria1", new BoolValue(true));
				query.SetCondition("Criteria2", new NumberValue(13));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = :pCriteria1_1 AND COLUMN2 = :pCriteria2_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 2);
				Assert.AreEqual("pCriteria1_1", ((IDbDataParameter) cmd.Parameters[0]).ParameterName);
				Assert.AreEqual("pCriteria2_1", ((IDbDataParameter) cmd.Parameters[1]).ParameterName);
				Assert.AreEqual(true, ((IDbDataParameter) cmd.Parameters[0]).Value);
				Assert.AreEqual(13, ((IDbDataParameter) cmd.Parameters[1]).Value);
			}

			[TestMethod]
			public void CommonSql_4()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]} {JUNK 1} {COLUMN2 [Criteria2]}} {JUNK 2}");

				query.SetCondition("Criteria1", new BoolValue(true));
				query.SetCondition("Criteria2", new NumberValue(13));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);

				// Note that now there is expected extra space before the AND resulting from the JUNK 1.
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = :pCriteria1_1 AND COLUMN2 = :pCriteria2_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 2);
				Assert.AreEqual("pCriteria1_1", ((IDbDataParameter)cmd.Parameters[0]).ParameterName);
				Assert.AreEqual("pCriteria2_1", ((IDbDataParameter)cmd.Parameters[1]).ParameterName);
				Assert.AreEqual(true, ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(13, ((IDbDataParameter)cmd.Parameters[1]).Value);
			}

			[TestMethod]
			public void CommonSql_5()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]}};");

				query.SetCondition("Criteria1", new CustomParameterlessConditionValue("test"));

				var cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.IsTrue(cmd.Parameters.Count == 0);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = 'test' /*hint*/;", cmd.CommandText);

				query.SetCondition("Criteria1", Operator.IsNot, new CustomParameterlessConditionValue("test"));

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.IsTrue(cmd.Parameters.Count == 0);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 <> 'test';", cmd.CommandText);
			}

			[TestMethod]
			public void CommonSql_6()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]} {COLUMN2 [Criteria2]} {COLUMN3 [Criteria3]}};");

				query.SetCondition("Criteria1", new CustomConditionValue(1, 2, 3));
				query.SetCondition("Criteria2", new CustomParameterlessConditionValue("test"));
				query.SetCondition("Criteria3", new CustomConditionValue(4, 5, 6));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = sillyProcedure(:pCriteria1_1, :pCriteria1_2, :pCriteria1_3) " +
				                "AND COLUMN2 = 'test' /*hint*/ AND COLUMN3 = sillyProcedure(:pCriteria3_1, :pCriteria3_2, :pCriteria3_3);", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 6);
				Assert.AreEqual("pCriteria1_1", ((IDbDataParameter)cmd.Parameters[0]).ParameterName);
				Assert.AreEqual("pCriteria1_2", ((IDbDataParameter)cmd.Parameters[1]).ParameterName);
				Assert.AreEqual("pCriteria1_3", ((IDbDataParameter)cmd.Parameters[2]).ParameterName);
				Assert.AreEqual("pCriteria3_1", ((IDbDataParameter)cmd.Parameters[3]).ParameterName);
				Assert.AreEqual("pCriteria3_2", ((IDbDataParameter)cmd.Parameters[4]).ParameterName);
				Assert.AreEqual("pCriteria3_3", ((IDbDataParameter)cmd.Parameters[5]).ParameterName);
				Assert.AreEqual(1, ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(2, ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(3, ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual(4, ((IDbDataParameter)cmd.Parameters[3]).Value);
				Assert.AreEqual(5, ((IDbDataParameter)cmd.Parameters[4]).Value);
				Assert.AreEqual(6, ((IDbDataParameter)cmd.Parameters[5]).Value);

			}

			[TestMethod]
			public void JoinSql_1()
			{
				// Slightly more complex query. We're getting Orders by Customer's name and OrderDate but we also have a possibility to filter by ShippedDate 
				// which we will omit in this test. We'll looking for orders shipped in November '95 from a customer whose name begins with Thomas.

				var query = new MockQuery(_connection, "SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate " +
				                                "FROM Orders, Customers " +
				                                "WHERE Customers.CustomerID = Orders.CustomerID " +
				                                "{AND {Customers.ContactName [ContactName]} " +
				                                "{Orders.OrderDate [OrderDate]} " +
												"{Orders.ShippedDate [ShippedDate]}}");

				query.SetCondition("ContactName", Operator.Contains, 
					new StringValue("Thomas", StringValue.MatchOption.BeginsWith));
				query.SetCondition("OrderDate", Operator.IsBetween, new DateValue(
					DateTime.Parse("11/1/1995", System.Globalization.CultureInfo.InvariantCulture), 
					DateTime.Parse("11/30/1995", System.Globalization.CultureInfo.InvariantCulture)));
				
				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				
				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate " +
					"FROM Orders, Customers " +
					"WHERE Customers.CustomerID = Orders.CustomerID " +
					"AND Customers.ContactName LIKE :pContactName_1 " +
					"AND Orders.OrderDate BETWEEN :pOrderDate_1 AND :pOrderDate_2"
					, cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 3);
				Assert.AreEqual("pContactName_1", ((IDbDataParameter)cmd.Parameters[0]).ParameterName);
				Assert.AreEqual("pOrderDate_1", ((IDbDataParameter)cmd.Parameters[1]).ParameterName);
				Assert.AreEqual("pOrderDate_2", ((IDbDataParameter)cmd.Parameters[2]).ParameterName);

				Assert.AreEqual("Thomas%", ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(DateTime.Parse("11/1/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(DateTime.Parse("11/30/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[2]).Value);
			}

			[TestMethod]
			public void JoinSql_2()
			{
				// More complex query. Similar to previous one except now we're looking for customer whose name either: begins with Thomas, ends with Hardy or has " John " in the middle.
				// This means we have 3 different conditions clustered inside a single group that generates an OR query (condition1 OR condition2). Also, now, we're looking at
				// the shipment date rather than order date.

				var query = new MockQuery(_connection, "SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate \n" +
												"FROM Orders, Customers \n" +
												"WHERE Customers.CustomerID = Orders.CustomerID \n" +
												"{AND @{({Customers.ContactName [ContactNameFirst]} {Customers.ContactName [ContactNameMiddle]} {Customers.ContactName [ContactNameLast]})} \n" +
												"{Orders.OrderDate [OrderDate]} \n" +
												"{Orders.ShippedDate [ShippedDate]}}");

				query.SetCondition("ContactNameFirst", Operator.Contains,
					new StringValue("Thomas", StringValue.MatchOption.BeginsWith));
				query.SetCondition("ContactNameMiddle", Operator.Contains,
					new StringValue(" John ", StringValue.MatchOption.OccursAnywhere));
				query.SetCondition("ContactNameLast", Operator.Contains,
					new StringValue("Hardy", StringValue.MatchOption.EndsWith));

				query.SetCondition("ShippedDate", Operator.IsGreaterThanOrEqualTo, 
					new DateValue(DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture)));

				var cmd = query.CreateCommand();
				
				AssertCommand(cmd);

				// Note that linefeeds differ between the original and the output. Some are removed. When outputting scopes, text before the scope is written but with reduced whitespace
				// as to clean up any junk that was potentially left by any scopes that were previously removed. Generator maintains the original formatting but some lines might end up
				// trimmed. This is expected behavior and this test covers it.
				
				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate \n" +
					"FROM Orders, Customers \n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND (Customers.ContactName LIKE :pContactNameFirst_1 OR Customers.ContactName LIKE :pContactNameMiddle_1 OR Customers.ContactName LIKE :pContactNameLast_1)\n" +
					"AND Orders.ShippedDate >= :pShippedDate_1"
					, cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 4);
				Assert.AreEqual("pContactNameFirst_1", ((IDbDataParameter)cmd.Parameters[0]).ParameterName);
				Assert.AreEqual("pContactNameMiddle_1", ((IDbDataParameter)cmd.Parameters[1]).ParameterName);
				Assert.AreEqual("pContactNameLast_1", ((IDbDataParameter)cmd.Parameters[2]).ParameterName);
				Assert.AreEqual("pShippedDate_1", ((IDbDataParameter)cmd.Parameters[3]).ParameterName);

				Assert.AreEqual("Thomas%", ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual("% John %", ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual("%Hardy", ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual(DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[3]).Value);
			}

			[TestMethod]
			public void NullSql_1()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]}}");

				// Set the condition
				query.SetCondition("Criteria1", new StringValue((string)null));

				var cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);				

				// Now try the overload
				query.SetCondition("Criteria1", (string)null);

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);

				// Now try IS NOT
				query.SetCondition("Criteria1", null, StringOperator.IsNot);

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NOT NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);
			}

			[TestMethod]
			public void NullSql_2()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]}}");

				// Set the DateTime condition
				query.SetCondition("Criteria1", new DateValue((DateTime?)null));

				var cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);

				// Now try IS NOT
				query.SetCondition("Criteria1", Operator.IsNot, new DateValue((DateTime?)null));

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NOT NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);

				// Set the Bool condition
				query.SetCondition("Criteria1", new BoolValue(null));

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);

				// Now try IS NOT
				query.SetCondition("Criteria1", Operator.IsNot, new BoolValue(null));

				cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IS NOT NULL", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 0);
			}

			[TestMethod]
			public void EmptyStringSql()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Criteria1]}}");

				// Set the condition
				query.SetCondition("Criteria1", new StringValue(""));

				var cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 = :pCriteria1_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 1);
				Assert.AreEqual("", ((IDbDataParameter)cmd.Parameters[0]).Value);

				// Now try IS NOT
				query.SetCondition("Criteria1", "", StringOperator.IsNot);

				cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 != :pCriteria1_1", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 1);
				Assert.AreEqual("", ((IDbDataParameter)cmd.Parameters[0]).Value);

				// Set the condition
				query.SetCondition("Criteria1", Operator.IsAnyOf, new StringValue(new[] { null, "A", "B" }));

				cmd = query.CreateCommand();

				AssertCommand(cmd);
				Assert.AreEqual("SELECT * FROM TABLE1 WHERE COLUMN1 IN (:pCriteria1_1, :pCriteria1_2, :pCriteria1_3)", cmd.CommandText);
				Assert.IsTrue(cmd.Parameters.Count == 3);
				Assert.AreEqual(DBNull.Value, ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual("A", ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual("B", ((IDbDataParameter)cmd.Parameters[2]).Value);
			}

			[TestMethod]
			public void Variables_1()
			{
				var query = new MockQuery(_connection, "SELECT * FROM TABLE1 {WHERE {COLUMN1 [Variable1]}}");
				var variable = "= 'Some Value'";

				query.DefineVariable("Variable1", variable);

				var cmd = query.CreateCommand();

				Assert.IsNotNull(cmd);
				Assert.IsTrue(cmd.CommandType == CommandType.Text);
				Assert.IsTrue(cmd.Parameters.Count == 0);
				Assert.AreEqual($"SELECT * FROM TABLE1 WHERE COLUMN1 {variable}", cmd.CommandText);				
			}

			private void AssertCommand(IDbCommand cmd)
			{
				Assert.IsNotNull(cmd);
				Assert.IsTrue(cmd.CommandType == CommandType.Text);
				Assert.IsTrue(cmd.Parameters.Count != 0);
			}
		}


	}
}
