Feature: TrackingChanges

Scenario: Insertion in Orders table is tracked in OrdersChanges table
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	When I insert new row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 998		| 924124	| I			|

Scenario: Deletion in Orders table is tracked in OrdersChanges table
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	When I delete existing row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 999		| 924125	| Tom Hanks		| 13		|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 999		| 924125	| D			|

Scenario: Update in Orders table is tracked in OrdersChanges table
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	When I update existing row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|
	Then the table "OrdersChanges" in database "DB4" should have row with values:
		| OrderID	| ProductID	| Operation	|
		| 1			| 101		| U			|