Feature: CreateChangeTrackingTable

Scenario: Create a change tracking table for the Orders table
	Given database "SourceDB" does not have a change tracking table for table "Orders"
	When I run CreateChangeTrackingTable service on database "SourceDB" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the database "SourceDB" should have change tracking table named "OrdersChanges"
	And table named "OrdersChanges" in database "SourceDB" should be empty