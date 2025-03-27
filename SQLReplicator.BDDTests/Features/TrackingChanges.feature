Feature: TrackingChanges

Scenario: Insertion in Orders table is tracked in OrdersChanges table
	Given source database does not have a trigger for table "Orders"
	And source database does not have a change tracking table for table "Orders"
	When I run CreateTrigger service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run CreateChangeTrackingTable service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I insert new row in table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|
	Then the table "OrdersChanges" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 998		| 924124	| I			|