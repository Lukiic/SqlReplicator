Feature: CreateTrigger

Scenario: Create a trigger for the Orders table
	Given database "SourceDB" does not have a trigger for table "Orders"
	When I run CreateTrigger service on database "SourceDB" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the database "SourceDB" should have trigger named "TrackChangesOrders"