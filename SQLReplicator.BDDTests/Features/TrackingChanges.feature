Feature: TrackingChanges

Scenario: Insertion in Orders table is tracked in OrdersChanges table
	Given database "DB4" does not have a trigger for table "Orders"
	And database "DB4" does not have a change tracking table for table "Orders"
	When I run CreateTrigger service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run CreateChangeTrackingTable service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I insert new row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 998		| 924124	| I			|

Scenario: Deletion in Orders table is tracked in OrdersChanges table
	Given database "DB4" does not have a trigger for table "Orders"
	And database "DB4" does not have a change tracking table for table "Orders"
	When I run CreateTrigger service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run CreateChangeTrackingTable service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I delete existing row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 999		| 924125	| Tom Hanks		| 13		|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 999		| 924125	| D			|

Scenario: Update in Orders table is tracked in OrdersChanges table
	Given database "DB4" does not have a trigger for table "Orders"
	And database "DB4" does not have a change tracking table for table "Orders"
	When I run CreateTrigger service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run CreateChangeTrackingTable service on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I update existing row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 1			| 101		| D			|
	And the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 1			| 101		| I			|