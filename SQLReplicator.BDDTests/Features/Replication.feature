Feature: Replication

Scenario: Insertion in Orders table of source server causes insertion in Orders table of destination server
	Given database "DB4" does not have a trigger for table "Orders"
	And database "DB4" does not have a change tracking table for table "Orders"
	And database "DB5" has an empty "Orders" table
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
	And I run service for generating commands on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run service for executing generated commands on database "DB5"
	Then the table "Orders" in database "DB5" should have row with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|

Scenario: Update in Orders table of source server causes update in Orders table of destination server
	Given database "DB4" does not have a trigger for table "Orders"
	And database "DB4" does not have a change tracking table for table "Orders"
	And database "DB5" has an empty "Orders" table
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
	And I run service for generating commands on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run service for executing generated commands on database "DB5"
	Then the table "Orders" in database "DB5" should have row with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|