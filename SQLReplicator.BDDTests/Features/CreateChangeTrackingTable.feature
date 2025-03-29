Feature: CreateChangeTrackingTable

Scenario: Create a change tracking table for the Orders table
	Given database "DB4" does not have a change tracking table for table "Orders"
	When I run CreateChangeTrackingTable service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the database "DB4" should have change tracking table named "OrdersChanges"
	And table named "OrdersChanges" in database "DB4" should be empty