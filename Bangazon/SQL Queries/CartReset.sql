-- run this query to reset your cart for testing 
UPDATE [Order]
SET PaymentTypeId = null
WHERE OrderId = 1