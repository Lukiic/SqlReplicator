Feature: ReplicationTrackingExclusion

Scenario: Changes replicated to the Orders table of destination server are not tracked in the change tracking table
	Given database "SourceDB" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And database "DestinationDB" has a trigger and an empty change tracking table for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	When I insert new row in database "SourceDB" table "Orders" with values:
		| OrderID	| ProductID	| CustomerName	| Quantity	|
		| 998		| 924124	| Tom Hanks		| 12		|
	And I run service for generating commands on database "SourceDB" for table "Orders" with key attributes:
		| AttributeName |
		| OrderID       |
		| ProductID     |
	And I run service for executing generated commands on database "DestinationDB"
	Then table named "OrdersChanges" in database "DestinationDB" should be empty