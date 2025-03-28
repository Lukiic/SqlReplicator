# BDD Tests for SQL Replicator

## Overview
This project contains BDD tests for verifying the replication functionality between a source database (`DB4`) and a destination database (`DB5`). The tests ensure that changes in the `Orders` table on the source server are correctly tracked and replicated to the destination server.

## Database Setup
Before running the tests, ensure that both `DB4` (source) and `DB5` (destination) are properly set up with the required tables.

### 1. Create the `Orders` Table
Run the following SQL script on **both** `DB4` and `DB5` to create the `Orders` table:

```sql
CREATE TABLE Orders (
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    CustomerName VARCHAR(255) NOT NULL,
    Quantity INT NOT NULL,
    PRIMARY KEY (OrderID, ProductID)
);
```

### 2. Insert Sample Data into `Orders`
To populate the `Orders` table with initial data, run this script on **both** `DB4` and `DB5`:

```sql
INSERT INTO Orders (OrderID, ProductID, CustomerName, Quantity) VALUES
(1, 101, 'Alice Johnson', 2),
(2, 102, 'Bob Smith', 5),
(3, 103, 'Charlie Brown', 1),
(4, 104, 'David Williams', 3),
(5, 105, 'Emma Davis', 4);
```

### 3. Create the `OrdersChanges` Table
The `OrdersChanges` table is used for tracking changes in the `Orders` table. Run the following script on **both** `DB4` and `DB5`:

```sql
SELECT TOP 0 OrderID, ProductID INTO OrdersChanges FROM Orders;
ALTER TABLE OrdersChanges
ADD Operation CHAR(1),
    ChangeID BIGINT IDENTITY(1,1),
    IsReplicated1 BIT,
    IsReplicated2 BIT;
```