/*
Script changed from pause script to stop script.
*/

set nocount on;

declare @employers TABLE (
	accountId bigint not null,
	ULN nvarchar(50) null,
	stopdate date not null
);
declare @apprenticeshipId bigint
declare @stopdate DATETIME
declare @providerid bigint
declare @EmployerAccountId bigint

insert into @employers
values 
/* ========================================================================== */
/* =================== DO NOT MODIFY ABOVE THIS LINE ======================== */
/* ========================================================================== */
-- put employer account Ids (numeric), ULNs (characters / optional - leave NULL if not required) and stop dates (iso-format) here e.g.
(1234,'0123456789','2021-01-01')
;
/* ========================================================================== */
/* =================== DO NOT MODIFY BELOW THIS LINE ======================== */
/* ========================================================================== */

declare cur cursor local for
with apprenticesToStop (apprenticeshipId, stopdate,providerid,EmployerAccountID) as (
	select a.Id, e.stopdate, c.ProviderId,c.EmployerAccountId
	from Apprenticeship a
	join Commitment c
		on c.Id = a.CommitmentId
	join @employers e
		on e.accountId = c.EmployerAccountId
	where PaymentStatus in (1,2) -- UPDATE TO INCLUDE PAUSED active or paused 
	and a.ULN = ISNULL(e.ULN, a.ULN) -- match ULN if set
)
select * from apprenticesToStop
open cur 
fetch next from cur into @apprenticeshipId, @stopdate,@providerid,@EmployerAccountId

while @@FETCH_STATUS = 0 begin
	BEGIN TRAN
	  
    --Just some vars here
    DECLARE @error INT
    declare @originalHistoryId BIGINT
	
	---------------------------------------------------------------------------------------------------------
	--update to the json extraction using stored proc logic
	---------------------------------------------------------------------------------------------------------
	declare @originalHistoryJson NVARCHAR(MAX) = (
	SELECT 
	s.*,
	CASE
		WHEN
			s.PaymentStatus = 0
		THEN
			s.Cost
		ELSE
			(
			SELECT TOP 1 Cost FROM PriceHistory WHERE ApprenticeshipId = s.Id
				AND ( 
					-- If started take if now with a PriceHistory or the last one (NULL end date)
					( s.StartDate <= getdate()
					  AND ( 
						( FromDate <= getdate() AND ToDate >= FORMAT(getdate(),'yyyMMdd')) 
						  OR ToDate IS NULL
						)
					)
					-- If not started take the first one
					OR (s.StartDate > getdate()) 
				)
				ORDER BY FromDate
			 )
	END AS 'Cost',
	p.*
	FROM ApprenticeshipSummary s
	left join PriceHistory p on p.ApprenticeshipId = s.Id
	WHERE
	s.Id = @apprenticeshipId FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER
	)
	
	-------------------------------------------------------------------------------------------------------
	
	declare @historyJson NVARCHAR(MAX)
      
    /* Read some data */           
   -- set @originalHistoryId = null
   -- select top 1 @originalHistoryId = Id, @originalHistoryJson = UpdatedState from History where ApprenticeshipId = @apprenticeshipId order by Id desc
   -------------------------------------
   --reading the table takes too long so using a FOR JSON AUTO so we capture it above.
   -------------------------------------
  
   
    /* End data read */
   
    print '-- Apprenticeship Id: ' + convert(varchar, @apprenticeshipId)
    
    /* Stop the record */
    update Apprenticeship set PaymentStatus=3, stopdate = @stopdate where Id = @apprenticeshipId
       
    if(@@ERROR != 0) BEGIN SET @error = @@ERROR GOTO batch_abort END
    print '-- Apprenticeship Stop date: ' + convert(varchar, @stopdate, 126)
   
    /* History */
----------------------------------------------------------------------
--no longer required as we always have history with the FOR JSON AUTO.
----------------------------------------------------------------------
--	if(@originalHistoryId is null) begin
--		print '-- No History record found - history will not be written'
--	end else begin

begin

  print ' --Updating history record: ' + convert(varchar, @originalHistoryJson) 

		set @historyJson = JSON_MODIFY(@originalHistoryJson,'$.PaymentStatus', 3)	
		set @historyJson = JSON_MODIFY(@historyJson,'$.StopDate',CONVERT(varchar(50),@stopdate,126))

		insert into History (EntityType, ApprenticeshipId, UserId, UpdatedByRole, ChangeType, CreatedOn, ProviderId, EmployerAccountId, UpdatedByName, OriginalState, UpdatedState)
		Values (
		'Apprenticeship', @ApprenticeshipId, 'DataFix', 'Employer', 'ChangeOfStopDate', GETDATE(), @ProviderId, @EmployerAccountId, 'DataFix', @originalHistoryJson, @historyJson
		)

	end   
   
    /* End History */

   
batch_abort:
           
    IF @error != 0
    BEGIN
        ROLLBACK;
        print '-- Rollback performed'
        RAISERROR ('-- Error(s) occurred', 11, 1);
    END
	ELSE
	BEGIN

		print '-- Committing transaction for ApprenticeshipId [' + cast(@apprenticeshipId as varchar) + ']'
		Rollback -- use ROLLBACK for dev'ing
		print '-- Completed'

	END
	print '-- ================================================================================'
	fetch next from cur into @apprenticeshipId, @stopdate,@providerid,@EmployerAccountId
end

close cur
deallocate cur

set nocount off;