Feature: CreateTrigger

Scenario: Create a trigger for the Orders table
	Given database "DB4" does not have a trigger for table "Orders"
	When I run CreateTrigger service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the database "DB4" should have trigger named "TrackChangesOrders"