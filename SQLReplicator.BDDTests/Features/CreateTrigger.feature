Feature: CreateTrigger

Scenario: Create a trigger for the Orders table
	Given source database does not have a trigger for table "Orders"
	When I run CreateTrigger service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the source database should have trigger named "TrackChangesOrders"