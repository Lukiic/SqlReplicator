Feature: CreateChangeTrackingTable

Scenario: Create a change tracking table for the Orders table
	Given source database does not have a change tracking table for table "Orders"
	When I run CreateChangeTrackingTable service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the source database should have change tracking table named "OrdersChanges"
	And the table named "OrdersChanges" should be empty