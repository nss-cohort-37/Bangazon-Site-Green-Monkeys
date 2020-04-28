INSERT INTO [Order] (DateCreated, userId, PaymentTypeId)
VALUES (GETDATE(), '00000000-ffff-ffff-ffff-ffffffffffff', Null)

INSERT INTO OrderProduct (OrderId, ProductId)
VALUES (8, 2)

INSERT INTO OrderProduct (OrderId, ProductId)
VALUES (8, 1)

