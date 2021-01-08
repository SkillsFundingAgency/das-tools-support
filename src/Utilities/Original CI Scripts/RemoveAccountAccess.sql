DECLARE @accountName VARCHAR(200);
SET @accountName = 'xxx';

SELECT * FROM employer_account.Account
WHERE Name LIKE '%' + @accountName + '%'


-- COPY APPROPRIATE ACCOUNT ID's


SELECT * FROM employer_account.[User] u
JOIN employer_account.UserAccountSettings uas
ON u.Id = uas.UserId
and uas.AccountId = xxx
JOIN employer_account.Membership m
ON U.Id = m.UserId
WHERE m.AccountId = xxx
and Email != 'xxx'


-- DELETE USER MEMBERSHIPS FOR ACCOUNT(S) IN QUESTION
BEGIN TRAN

DELETE m FROM employer_account.[User] u
JOIN employer_account.UserAccountSettings uas
ON u.Id = uas.UserId
and uas.AccountId = xxx
JOIN employer_account.Membership m
ON U.Id = m.UserId
WHERE m.AccountId = xxx
and Email != 'xxx'

COMMIT;
-- 6 rows updated

-- Post Check
SELECT * FROM employer_account.[User] u
JOIN employer_account.UserAccountSettings uas
ON u.Id = uas.UserId
and uas.AccountId = xxx
JOIN employer_account.Membership m
ON U.Id = m.UserId
WHERE m.AccountId = xxx
and Email != 'xxx'
-- Zero rows returned

select * from  [employer_account].[Invitation]
where Email = 'xxx'


BEGIN TRAN
delete from [employer_account].[Invitation]
where Email = 'xxx'
COMMIT;
-- 1 row deleted

-- Post Check
select * from  [employer_account].[Invitation]
where Email = 'xxx'
-- No rows