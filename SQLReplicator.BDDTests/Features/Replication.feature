Feature: Replication

Scenario: Insertion in Orders table of source server causes insertion in Orders table of destination server
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And database "DB5" has an empty "Orders" table
	When I insert new row in database "DB4" table "Orders" with values:
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
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And database "DB5" has an empty "Orders" table
	When I insert new row in database "DB5" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|
	When I update existing row in database "DB4" table "Orders" with values:
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

Scenario: Deletion in Orders table of source server causes deletion in Orders table of destination server
	Given database "DB4" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And database "DB5" has an empty "Orders" table
	When I insert new row in database "DB5" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 6			| 101		| Alice Johnson	| 5			|
	And I delete existing row in database "DB4" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 6			| 101		| Alice Johnson	| 5			|
	And I run service for generating commands on database "DB4" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run service for executing generated commands on database "DB5"
	Then table named "Orders" in database "DB5" should be empty