﻿using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBinder.ConditionValues;

namespace SqlBinder.UnitTesting
{
	public partial class SqlBinder_Tests
	{
		/// <summary>
		/// Tests that cover more complex Sql. A flexible script is defined which can be used for many different queries with different conditions.
		/// </summary>
		[TestClass]
		public class Complex_Sql
		{
			private MockQuery _query;

			[TestInitialize]
			public void InitializeTest()
			{
				_query = new MockQuery(_connection,
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
				    "FROM Orders, Customers\n" +
				    "WHERE Customers.CustomerID = Orders.CustomerID\n" +
				    "{AND @{({Customers.ContactName [ContactNameFirst]} {Customers.ContactName [ContactNameMiddle]} {Customers.ContactName [ContactNameLast]})}\n" +
					"{Orders.OrderID [OrderID]}" +
				    "@{({Orders.OrderDate [OrderDate]} {Orders.ShippedDate [ShippedDate]} {Orders.RequiredDate [RequiredDate]})}\n" +
				    "{Orders.EmployeeID IN (SELECT EmployeeID FROM Employees WHERE {FirstName [EmployeeFirstName]} {LastName [EmployeeLastName]})}\n" +
					"{Orders.OrderID IN (SELECT OrderID FROM $[Order Details]$ WHERE ProductID IN (SELECT ProductID FROM Products WHERE {ProductName [ProductName]} {SupplierID IN (SELECT SupplierID FROM Suppliers WHERE {CompanyName [SupplierCompanyName]})}))}\n" +
				    "{Orders.Freight [Freight]}}");
				_query.ThrowScriptErrorException = true;
			}

			/// <summary>
			/// Simple test where the above heavy-duty query is reduced to a simple ID query
			/// </summary>
			[TestMethod]
			public void ComplexSql_1()
			{
				_query.SetCondition("OrderID", Operator.Is, new NumberValue(10));

				var cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"Orders.OrderID = :pOrderID_1",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 1);

				Assert.AreEqual(10, ((IDbDataParameter)cmd.Parameters[0]).Value);
			}

			/// <summary>
			/// Simple test where we just want orders with specific freight cost
			/// </summary>
			[TestMethod]
			public void ComplexSql_2()
			{
				_query.SetCondition("Freight", Operator.IsNotBetween, new NumberValue(50, 100));

				var cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"Orders.Freight NOT BETWEEN :pFreight_1 AND :pFreight_2",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 2);

				Assert.AreEqual(50, ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(100, ((IDbDataParameter)cmd.Parameters[1]).Value);
			}

			/// <summary>
			/// A much more complex query now, we're looking for specific freight, specific dates and product name now.
			/// </summary>
			[TestMethod]
			public void ComplexSql_3()
			{			
				_query.SetCondition("ShippedDate", Operator.IsLessThanOrEqualTo,
					new DateValue(DateTime.Parse("12/30/1995", System.Globalization.CultureInfo.InvariantCulture)));
				_query.SetCondition("RequiredDate", Operator.IsBetween, new DateValue(
					DateTime.Parse("9/1/1995", System.Globalization.CultureInfo.InvariantCulture),
					DateTime.Parse("11/30/1995", System.Globalization.CultureInfo.InvariantCulture)));

				_query.SetCondition("ProductName", Operator.Is, new StringValue("Tofu"));

				_query.SetCondition("Freight", Operator.IsLessThan, new NumberValue(200));

				var cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"( Orders.ShippedDate <= :pShippedDate_1 OR Orders.RequiredDate BETWEEN :pRequiredDate_1 AND :pRequiredDate_2)\n" +
					"AND Orders.OrderID IN (SELECT OrderID FROM [Order Details] WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductName = :pProductName_1 ))\n" +
					"AND Orders.Freight < :pFreight_1",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 5);

				Assert.AreEqual(DateTime.Parse("12/30/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(DateTime.Parse("9/1/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(DateTime.Parse("11/30/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual("Tofu", ((IDbDataParameter)cmd.Parameters[3]).Value);
				Assert.AreEqual(200, ((IDbDataParameter)cmd.Parameters[4]).Value);
			}

			/// <summary>
			/// Here with such complex query we'll now test the shortcut SetCondition overloads. We'll also dynamically extend and reduce the query without rebuilding it.
			/// </summary>
			[TestMethod]
			public void ComplexSql_4()
			{
				// Compile the query 

				_query.SetCondition("OrderDate", DateTime.Parse("12/30/1995", System.Globalization.CultureInfo.InvariantCulture), NumericOperator.IsLessThanOrEqualTo);
				_query.SetCondition("ShippedDate",
					new[]
					{
						DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture),
						DateTime.Parse("11/21/1995", System.Globalization.CultureInfo.InvariantCulture),
						DateTime.Parse("11/25/1995", System.Globalization.CultureInfo.InvariantCulture)
					});
				_query.SetCondition("ProductName", new[] { "Tofu", "Chai", "Chocolade" });
				_query.SetCondition("Freight", 200m, NumericOperator.IsGreaterThanOrEqualTo);

				var cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"(Orders.OrderDate <= :pOrderDate_1 OR Orders.ShippedDate IN (:pShippedDate_1, :pShippedDate_2, :pShippedDate_3) )\n" +
					"AND Orders.OrderID IN (SELECT OrderID FROM [Order Details] WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductName IN (:pProductName_1, :pProductName_2, :pProductName_3) ))\n" +
					"AND Orders.Freight >= :pFreight_1",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 8);

				Assert.AreEqual(DateTime.Parse("12/30/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(DateTime.Parse("11/21/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual(DateTime.Parse("11/25/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[3]).Value);
				Assert.AreEqual("Tofu", ((IDbDataParameter)cmd.Parameters[4]).Value);
				Assert.AreEqual("Chai", ((IDbDataParameter)cmd.Parameters[5]).Value);
				Assert.AreEqual("Chocolade", ((IDbDataParameter)cmd.Parameters[6]).Value);
				Assert.AreEqual(200m, ((IDbDataParameter)cmd.Parameters[7]).Value);

				// Add extra condition

				_query.SetCondition("SupplierCompanyName", "Tokyo Traders");

				cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"(Orders.OrderDate <= :pOrderDate_1 OR Orders.ShippedDate IN (:pShippedDate_1, :pShippedDate_2, :pShippedDate_3) )\n" +
					"AND Orders.OrderID IN (SELECT OrderID FROM [Order Details] WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductName IN (:pProductName_1, :pProductName_2, :pProductName_3) AND SupplierID IN (SELECT SupplierID FROM Suppliers WHERE CompanyName = :pSupplierCompanyName_1)))\n" +
					"AND Orders.Freight >= :pFreight_1",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 9);

				Assert.AreEqual(DateTime.Parse("12/30/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(DateTime.Parse("11/21/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual(DateTime.Parse("11/25/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[3]).Value);
				Assert.AreEqual("Tofu", ((IDbDataParameter)cmd.Parameters[4]).Value);
				Assert.AreEqual("Chai", ((IDbDataParameter)cmd.Parameters[5]).Value);
				Assert.AreEqual("Chocolade", ((IDbDataParameter)cmd.Parameters[6]).Value);
				Assert.AreEqual("Tokyo Traders", ((IDbDataParameter)cmd.Parameters[7]).Value);
				Assert.AreEqual(200m, ((IDbDataParameter)cmd.Parameters[8]).Value);

				// Remove two conditions

				_query.RemoveCondition("ProductName");
				_query.RemoveCondition("OrderDate");

				cmd = _query.CreateCommand();

				AssertCommand(cmd);

				Assert.AreEqual(
					"SELECT Orders.OrderID, Customers.ContactName, Orders.OrderDate, Orders.ShippedDate\n" +
					"FROM Orders, Customers\n" +
					"WHERE Customers.CustomerID = Orders.CustomerID\n" +
					"AND\n" +
					"( Orders.ShippedDate IN (:pShippedDate_1, :pShippedDate_2, :pShippedDate_3) )\n" +
					"AND Orders.OrderID IN (SELECT OrderID FROM [Order Details] WHERE ProductID IN (SELECT ProductID FROM Products WHERE SupplierID IN (SELECT SupplierID FROM Suppliers WHERE CompanyName = :pSupplierCompanyName_1)))\n" +
					"AND Orders.Freight >= :pFreight_1",
					cmd.CommandText);

				Assert.IsTrue(cmd.Parameters.Count == 5);

				Assert.AreEqual(DateTime.Parse("11/20/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[0]).Value);
				Assert.AreEqual(DateTime.Parse("11/21/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[1]).Value);
				Assert.AreEqual(DateTime.Parse("11/25/1995", System.Globalization.CultureInfo.InvariantCulture), ((IDbDataParameter)cmd.Parameters[2]).Value);
				Assert.AreEqual("Tokyo Traders", ((IDbDataParameter)cmd.Parameters[3]).Value);
				Assert.AreEqual(200m, ((IDbDataParameter)cmd.Parameters[4]).Value);
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
