use eShop

INSERT INTO Product (ProductName,UnitPrice ,UnitInStock) VALUES('Mac Air',50000,10) 

SELECT * FROM Employee 

UPDATE Employee SET EmployeeName='โอ๊ต',UserName = 'oat',Password='oat' WHERE EmployeeID=7

DELETE FROM Employee WHERE EmployeeID=7  

SELECT * FROM Employee WHERE UserName='admin' AND Password='admin'

SELECT * FROM Employee WHERE EmployeeName LIKE '[ก-ธ]%'  ORDER BY EmployeeName DESC,StartDate 
SELECT SUM(NetTotal) As SaleSummation   FROM Sale WHERE SaleDate BETWEEN '2012-07-01' AND '2012-07-15'

SELECT * FROM Sale 

SELECT COUNT(ProductID) As ProductCount FROM Product 

 SELECT ProductName,UnitPrice FROM Product
  WHERE UnitPrice=(SELECT MAX(UnitPrice) AS MaxPrice FROM Product )