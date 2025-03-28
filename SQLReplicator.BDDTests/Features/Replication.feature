Feature: Replication

Scenario: Insertion in Orders table of source server causes insertion in Orders table of destination server
	Given source database does not have a trigger for table "Orders"
	And source database does not have a change tracking table for table "Orders"
	And destination database has an empty "Orders" table
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
	And I run services for generating and executing commands on table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the table "Orders" of destination server should have row with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|

Scenario: Update in Orders table of source server causes update in Orders table of destination server
	Given source database does not have a trigger for table "Orders"
	And source database does not have a change tracking table for table "Orders"
	And destination database has an empty "Orders" table
	When I run CreateTrigger service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run CreateChangeTrackingTable service for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I update existing row in table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|
	And I run services for generating and executing commands on table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	Then the table "Orders" of destination server should have row with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 1			| 101		| Alice Johnson	| 2			|