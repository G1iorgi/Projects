-- Insert CreditCards
INSERT INTO "DigitalBank"."CreditCards" ("Number", "ExpirationDate", "CVV")
VALUES
('4234567890123456', '01/28', '123'),
('4102030405060708', '01/24', '123'),
('4543210987654321', '07/26', '321'),
('4070605040302010', '07/22', '321'),
('4111222233334444', '03/27', '111'),
('4444333322221111', '03/23', '222');

-- Insert Balances for the first CreditCard
INSERT INTO "DigitalBank"."Balances" ("Currency", "Amount", "CreditCardId")
VALUES
('GEL', 1000, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4234567890123456')),
('USD', 100, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4234567890123456')),
('EUR', 50, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4234567890123456'));
-- Insert Balances for the first CreditCard
INSERT INTO "DigitalBank"."Balances" ("Currency", "Amount", "CreditCardId")
VALUES
('GEL', 1000, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4102030405060708')),
('USD', 100, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4102030405060708')),
('EUR', 50, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4102030405060708'));

-- Insert Balances for the second CreditCard
INSERT INTO "DigitalBank"."Balances" ("Currency", "Amount", "CreditCardId")
VALUES
('GEL', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4543210987654321')),
('USD', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4543210987654321')),
('EUR', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4543210987654321'));
-- Insert Balances for the second CreditCard
INSERT INTO "DigitalBank"."Balances" ("Currency", "Amount", "CreditCardId")
VALUES
('GEL', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4070605040302010')),
('USD', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4070605040302010')),
('EUR', 0, (SELECT "Id" FROM "DigitalBank"."CreditCards" WHERE "Number" = '4070605040302010'));
