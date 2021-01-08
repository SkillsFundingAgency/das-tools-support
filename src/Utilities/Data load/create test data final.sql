--STEP 1 --create accounts
 
--execute in this db [das-pp-eas-acc-db]

declare @accountsToCreate int = 10 --SET <<----
declare @counter int = 1
declare @date varchar(30) = CONVERT(VARCHAR(30), getdate(), 120);
declare @ids varchar(max) = ''

while ( @counter <= @accountsToCreate)
begin 
	declare @name varchar(100) = 'fraudtest_' + @date + '_' + cast(@counter as varchar(10))
	print 'creating account : ' + @name
	
	INSERT INTO [employer_account].[Account]
           (
		   [HashedId]
		   ,[Name]
           ,[CreatedDate]
           ,[ApprenticeshipEmployerType])
     VALUES
           (
		   NEWID()
           ,@name
           ,getdate()                      
           ,0)
	if LEN(@ids) > 0 
		set @ids =@ids + ',' + cast(SCOPE_IDENTITY() as varchar(20)) 
	else
		set @ids =@ids + cast(SCOPE_IDENTITY() as varchar(20)) 

	set @counter = @counter + 1; 
end 

select @ids as 'ids take to the next step' 

---test
--select * from [employer_account].[Account] where name like 'fraudtest_%'

-------------------------
--STEP 2 -- create levy delclarations 

--execute in this db das-pp-eas-fin-db

--SET @accountIds with values form the previous step 
declare @accountIds varchar(max) = '' --<< SET id1,id2,id3


DECLARE @accountCursor CURSOR;
DECLARE @accountId nvarchar(10);
BEGIN
    SET @accountCursor = CURSOR FOR
    select value as accountId from string_split(@accountIds, ',')
    
    OPEN @accountCursor 
    FETCH NEXT FROM @accountCursor 
    INTO @accountId

	set @accountId = TRIM(@accountId)

	IF LEN(TRIM(@accountId)) > 0 
	BEGIN
		WHILE @@FETCH_STATUS = 0
		BEGIN
			print @accountId

			--record creation

			declare @empRef nvarchar(50) = '101/test' + cast(@accountId as varchar(20))		
			declare @payrollYears nvarchar(100) = '17-18,18-19,19-20' --<<--SET payroll years: 17-18,18-19

			declare @submissionId bigint 
			select @submissionId = max(SubmissionId) from [employer_financial].[LevyDeclaration] where SubmissionId > 300000 and SubmissionId < 400000
			set @submissionId = @submissionId + 1;

			--years
			DECLARE @yearCursor CURSOR;
			DECLARE @year nvarchar(10);
			BEGIN
				SET @yearCursor = CURSOR FOR
				select value as payrollYear from string_split(@payrollYears, ',')
    
				OPEN @yearCursor 
				FETCH NEXT FROM @yearCursor 
				INTO @year			

				WHILE @@FETCH_STATUS = 0
				BEGIN
					print @year
					--months 				
					declare @levyMonth decimal(18,4)
					declare @levyDueYTD decimal(18,4) = 0.0000;		
					declare @submissionDate datetime; 
					declare @levyAllowanceForYear decimal(18,4) = FLOOR(RAND()*(150000-20000)+20000);

					declare @month int = 1
					while(@month <13)
					BEGIN
						print @month
					
						set @levyMonth = FLOOR(RAND()*(50000-5000)+5000);
						set @levyDueYTD = @levyDueYTD + @levyMonth;
						set @submissionDate = DATEADD(day, -10, [employer_financial].[CalculateSubmissionCutoffDate](@month, @year))

						--create recotds 
						INSERT INTO [employer_financial].[LevyDeclaration]
					   ([AccountId]
					   ,[empRef]
					   ,[LevyDueYTD]
					   ,[LevyAllowanceForYear]
					   ,[SubmissionDate]
					   ,[SubmissionId]
					   ,[PayrollYear]
					   ,[PayrollMonth]
					   ,[CreatedDate]
					   ,[EndOfYearAdjustment]
					   ,[EndOfYearAdjustmentAmount]
					   ,[DateCeased]
					   ,[InactiveFrom]
					   ,[InactiveTo]
					   ,[HmrcSubmissionId]
					   ,[NoPaymentForPeriod])
				 VALUES
					   ( @accountId --<AccountId, bigint,>
					   ,@empRef --<empRef, nvarchar(50),>
					   ,@levyDueYTD --<LevyDueYTD, decimal(18,4),>
					   ,@levyAllowanceForYear--<LevyAllowanceForYear, decimal(18,4),>
					   ,@submissionDate-- <SubmissionDate, datetime,>
					   ,@submissionId--<SubmissionId, bigint,>
					   ,@year --<PayrollYear, nvarchar(10),>
					   ,@month --<PayrollMonth, tinyint,>
					   ,@submissionDate--<CreatedDate, datetime,>
					   ,0--<EndOfYearAdjustment, bit,>
					   ,0.0000--<EndOfYearAdjustmentAmount, decimal(18,4),>
					   ,NULL--<DateCeased, datetime,>
					   ,NULL--<InactiveFrom, datetime,>
					   ,NULL--<InactiveTo, datetime,>
					   ,NULL--<HmrcSubmissionId, bigint,>
					   ,NULL)--<NoPaymentForPeriod, bit,>)

						set @month = @month + 1
						set @submissionId = @submissionId + 1 
					END

				  FETCH NEXT FROM @yearCursor 
				  INTO @year 
				  set @month = 1;
				END; 

				CLOSE @yearCursor ;
				DEALLOCATE @yearCursor;
			END;


			--record creation

			FETCH NEXT FROM @accountCursor 
			INTO @accountId 
		END

		CLOSE @accountCursor ;
		DEALLOCATE @accountCursor;
	END
END

-------------------------
--STEP 3 -- create apprenticeship 

--execute in this db das-pp-comt-db

declare @accountIds varchar(max) = '' --<< SET 
--declare @accountIds varchar(max) = '187265'
declare @accountAndapprenticeshipIds nvarchar(max) = '';

DECLARE @accountCursor CURSOR;
DECLARE @accountId nvarchar(10);
BEGIN
    SET @accountCursor = CURSOR FOR
    select value as accountId from string_split(@accountIds, ',')
    
    OPEN @accountCursor 
    FETCH NEXT FROM @accountCursor 
    INTO @accountId

	set @accountId = TRIM(@accountId)
	
	BEGIN
		WHILE @@FETCH_STATUS = 0
		BEGIN
			print 'creating commitment'

			--record creation

			--create commitment

			INSERT INTO [dbo].[Commitment]
			   ([Reference]
			   ,[EmployerAccountId]
			   ,[ProviderId]
			   ,[CommitmentStatus]
			   ,[EditStatus]
			   ,[CreatedOn]
			   ,[LastAction]
			   ,[LastUpdatedByEmployerName]
			   ,[LastUpdatedByEmployerEmail]
			   ,[LastUpdatedByProviderName]
			   ,[LastUpdatedByProviderEmail]
			   ,[TransferSenderId]
			   ,[TransferApprovalStatus]
			   ,[TransferApprovalActionedByEmployerName]
			   ,[TransferApprovalActionedByEmployerEmail]
			   ,[TransferApprovalActionedOn]
			   ,[Originator]
			   ,[ApprenticeshipEmployerTypeOnApproval]
			   ,[IsFullApprovalProcessed]
			   ,[IsDeleted]
			   ,[AccountLegalEntityId]
			   ,[IsDraft]
			   ,[WithParty]
			   ,[LastUpdatedOn]
			   ,[Approvals]
			   ,[EmployerAndProviderApprovedOn]
			   ,[ChangeOfPartyRequestId])
			VALUES
			   (newid()-- <Reference, nvarchar(100),>
			   ,cast(@accountId as bigint)--<EmployerAccountId, bigint,>
			   ,1--<ProviderId, bigint,>
			   ,1--<CommitmentStatus, smallint,>
			   ,0--<EditStatus, smallint,>
			   ,NULL--<CreatedOn, datetime,>
			   ,2--<LastAction, smallint,>
			   ,'test data'--<LastUpdatedByEmployerName, nvarchar(255),>
			   ,NULL--<LastUpdatedByEmployerEmail, nvarchar(255),>
			   ,NULL--<LastUpdatedByProviderName, nvarchar(255),>
			   ,NULL--<LastUpdatedByProviderEmail, nvarchar(255),>
			   ,NULL--<TransferSenderId, bigint,>
			   ,NULL--<TransferApprovalStatus, tinyint,>
			   ,NULL--<TransferApprovalActionedByEmployerName, nvarchar(255),>
			   ,NULL--<TransferApprovalActionedByEmployerEmail, nvarchar(255),>
			   ,NULL--<TransferApprovalActionedOn, datetime2(7),>
			   ,0--<Originator, tinyint,>
			   ,NULL--<ApprenticeshipEmployerTypeOnApproval, tinyint,>
			   ,0--<IsFullApprovalProcessed, bit,>
			   ,0--<IsDeleted, bit,>
			   ,NULL--<AccountLegalEntityId, bigint,>
			   ,0--<IsDraft, bit,>
			   ,0--<WithParty, smallint,>
			   ,getdate()--<LastUpdatedOn, datetime2(7),>
			   ,3--<Approvals, smallint,>
			   ,NULL--<EmployerAndProviderApprovedOn, datetime2(7),>
			   ,NULL)--<ChangeOfPartyRequestId, bigint,>)

			   declare @commitmentId bigint = SCOPE_IDENTITY();

			--commitment created

			--create apprenticeship 
			INSERT INTO [dbo].[Apprenticeship]
			   ([CommitmentId]
			   ,[FirstName]
			   ,[LastName]
			   ,[ULN]
			   ,[TrainingType]
			   ,[TrainingCode]
			   ,[TrainingName]
			   ,[Cost]
			   ,[StartDate]
			   ,[EndDate]
			   ,[AgreementStatus]
			   ,[PaymentStatus]
			   ,[DateOfBirth]
			   ,[NINumber]
			   ,[EmployerRef]
			   ,[ProviderRef]
			   ,[CreatedOn]
			   ,[AgreedOn]
			   ,[PaymentOrder]
			   ,[StopDate]
			   ,[PauseDate]
			   ,[HasHadDataLockSuccess]
			   ,[PendingUpdateOriginator]
			   ,[EPAOrgId]
			   ,[CloneOf]
			   ,[ReservationId]
			   ,[CompletionDate]
			   ,[ContinuationOfId]
			   ,[MadeRedundant]
			   ,[OriginalStartDate])
			VALUES
			   (@commitmentId --<CommitmentId, bigint,>
			   ,NEWID() --<FirstName, nvarchar(100),>
			   ,NEWID() --<LastName, nvarchar(100),>
			   ,NULL --<ULN, nvarchar(50),>
			   ,0--<TrainingType, int,>
			   ,91--<TrainingCode, nvarchar(20),>
			   ,'test course level 4'--<TrainingName, nvarchar(126),>
			   ,FLOOR(RAND()*(90000-1000)+1000) --<Cost, decimal(18,0),>
			   ,'2017-01-01 00:00:00.000'--<StartDate, datetime,>
			   ,'2020-12-01 00:00:00.000'--<EndDate, datetime,>
			   ,3--<AgreementStatus, smallint,>
			   ,1--<PaymentStatus, smallint,>
			   ,NULL-- <DateOfBirth, datetime,>
			   ,NULL--<NINumber, nvarchar(10),>
			   ,NULL--<EmployerRef, nvarchar(50),>
			   ,NULL--<ProviderRef, nvarchar(50),>
			   ,GETDATE()-- <CreatedOn, datetime,>
			   ,GETDATE()--<AgreedOn, datetime,>
			   ,6--<PaymentOrder, int,>
			   ,NULL--<StopDate, date,>
			   ,NULL--<PauseDate, date,>
			   ,0--<HasHadDataLockSuccess, bit,>
			   ,NULL--<PendingUpdateOriginator, tinyint,>
			   ,NULL--<EPAOrgId, char(7),>
			   ,NULL--<CloneOf, bigint,>
			   ,NULL--<ReservationId, uniqueidentifier,>
			   ,NULL--<CompletionDate, datetime,>
			   ,NULL--<ContinuationOfId, bigint,>
			   ,NULL--<MadeRedundant, bit,>
			   ,NULL)--<OriginalStartDate, datetime,>)
			--apprenticeship created 

			
			
			if LEN(@accountAndapprenticeshipIds) > 0 
				set @accountAndapprenticeshipIds = @accountAndapprenticeshipIds + ',' + @accountId + '.'+ cast(SCOPE_IDENTITY() as varchar(20)) 
			else
				set @accountAndapprenticeshipIds = @accountAndapprenticeshipIds + @accountId + '.'+ cast(SCOPE_IDENTITY() as varchar(20)) 
			
			--record creation

			FETCH NEXT FROM @accountCursor 
			INTO @accountId 
		END

		CLOSE @accountCursor ;
		DEALLOCATE @accountCursor;
	END
END

select @accountAndapprenticeshipIds as 'account and apprenticeship Ids'


---------------
--STEP 4 -- create payments  

--execute in this db das-pp-datamgmt-staging-db

declare @accountAndApprenticeshipIds nvarchar(max) = '' --<< SET example '187265.201706' 'accountId.apprenticeshipId,accountId.apprenticeshipId'


DECLARE @accountCursor CURSOR;
DECLARE @ids nvarchar(30);
declare @accountId nvarchar(10)
declare @apprenticeshipId nvarchar(10)
BEGIN
    SET @accountCursor = CURSOR FOR
    select value as accountAndApprenticeshipId from string_split(@accountAndApprenticeshipIds, ',')
    
    OPEN @accountCursor 
    FETCH NEXT FROM @accountCursor 
    INTO @ids	
	
	BEGIN
		WHILE @@FETCH_STATUS = 0
		BEGIN
			set @accountId = Parsename(@ids,2);
			set @apprenticeshipId = Parsename(@ids,1);
						
			print 'accountId: ' + @accountId
			print 'apprenticeshipId: ' + @apprenticeshipId
			
			--record creation

			declare @month int = 1
			declare @id int 
			select @id = ISNULL(max(id),1) from [StgPmts].[Payment]

			declare @academicYears nvarchar(100) = '1718,1819,1920'

			DECLARE @yearCursor CURSOR;
			DECLARE @year nvarchar(10);
			BEGIN
				SET @yearCursor = CURSOR FOR
				select value as academicYear from string_split(@academicYears, ',')
    
				OPEN @yearCursor 
				FETCH NEXT FROM @yearCursor 
				INTO @year

				WHILE @@FETCH_STATUS = 0
				BEGIN
					print 'academic year: ' + @year
	
					--months
					while(@month <13)
					BEGIN						

						INSERT INTO [StgPmts].[Payment]
								   ([Id]
								   ,[EventId] --PaymentID
								   ,[EarningEventId]
								   ,[FundingSourceEventId]
								   ,[EventTime]
								   ,[JobId]
								   ,[DeliveryPeriod]
								   ,[CollectionPeriod]
								   ,[AcademicYear]
								   ,[Ukprn]
								   ,[LearnerReferenceNumber]
								   ,[LearnerUln]
								   ,[PriceEpisodeIdentifier]
								   ,[Amount]
								   ,[LearningAimReference]
								   ,[LearningAimProgrammeType]
								   ,[LearningAimStandardCode]
								   ,[LearningAimFrameworkCode]
								   ,[LearningAimPathwayCode]
								   ,[LearningAimFundingLineType]
								   ,[ContractType]
								   ,[TransactionType]
								   ,[FundingSource]
								   ,[IlrSubmissionDateTime]
								   ,[SfaContributionPercentage]
								   ,[AgreementId]
								   ,[AccountId]
								   ,[TransferSenderAccountId]
								   ,[CreationDate]
								   ,[EarningsStartDate]
								   ,[EarningsPlannedEndDate]
								   ,[EarningsActualEndDate]
								   ,[EarningsCompletionStatus]
								   ,[EarningsCompletionAmount]
								   ,[EarningsInstalmentAmount]
								   ,[EarningsNumberOfInstalments]
								   ,[LearningStartDate]
								   ,[ApprenticeshipId]
								   ,[ApprenticeshipPriceEpisodeId]
								   ,[ApprenticeshipEmployerType]
								   ,[ReportingAimFundingLineType]
								   ,[NonPaymentReason]
								   ,[DuplicateNumber])
							 VALUES
								   (@id
								   ,NEWID()--<EventId, uniqueidentifier,>
								   ,NEWID()--<EarningEventId, uniqueidentifier,>
								   ,NEWID()--<FundingSourceEventId, uniqueidentifier,>
								   ,GETDATE() --<EventTime, datetimeoffset(7),>
								   ,0 --<JobId, bigint,>
								   ,@month--<DeliveryPeriod, tinyint,>
								   ,@month--<CollectionPeriod, tinyint,>
								   ,@year--<AcademicYear, smallint,>
								   ,10001467--<Ukprn, bigint,>
								   ,'test'--<LearnerReferenceNumber, nvarchar(50),>
								   ,3713987466--<LearnerUln, bigint,>
								   ,'25-5-01/08/2019'--<PriceEpisodeIdentifier, nvarchar(50),>
								   ,FLOOR(RAND()*(500-10)+10)--30.00000--<Amount, decimal(15,5),>
								   ,'ZPROtest'--<LearningAimReference, nvarchar(8),>
								   ,0--<LearningAimProgrammeType, int,>
								   ,0--<LearningAimStandardCode, int,>
								   ,0--<LearningAimFrameworkCode, int,>
								   ,0--<LearningAimPathwayCode, int,>
								   ,'LearningAimFundingLineType'--<LearningAimFundingLineType, nvarchar(100),>
								   ,2--<ContractType, tinyint,>
								   ,1--<TransactionType, tinyint,>
								   ,3--<FundingSource, tinyint,>
								   ,GETDATE()-- <IlrSubmissionDateTime, datetime2(7),>
								   ,1.00000 --<SfaContributionPercentage, decimal(15,5),>
								   ,NULL --<AgreementId, nvarchar(255),>
								   ,cast(@accountId as bigint) --<AccountId, bigint,>
								   ,NULL --<TransferSenderAccountId, bigint,>
								   ,GETDATE() --<CreationDate, datetimeoffset(7),>
								   ,GETDATE() --<EarningsStartDate, datetime,>
								   ,NULL --<EarningsPlannedEndDate, datetime,>
								   ,NULL --<EarningsActualEndDate, datetime,>
								   ,NULL --<EarningsCompletionStatus, tinyint,>
								   ,NULL --<EarningsCompletionAmount, decimal(15,5),>
								   ,NULL --<EarningsInstalmentAmount, decimal(15,5),>
								   ,NULL --<EarningsNumberOfInstalments, smallint,>
								   ,NULL --<LearningStartDate, datetime2(7),>
								   ,cast(@apprenticeshipId as bigint) --<ApprenticeshipId, bigint,>
								   ,NULL --<ApprenticeshipPriceEpisodeId, bigint,>
								   ,NULL --<ApprenticeshipEmployerType, tinyint,>
								   ,NULL --<ReportingAimFundingLineType, nvarchar(120),>
								   ,NULL --<NonPaymentReason, tinyint,>
								   ,NULL) --<DuplicateNumber, int,>)

						set @month = @month + 1
						set @id = @id + 1	
					END

				  FETCH NEXT FROM @yearCursor 
				  INTO @year 
				  set @month = 1;
				END;

				CLOSE @yearCursor ;
				DEALLOCATE @yearCursor;
			END;
			

			--record creation

			FETCH NEXT FROM @accountCursor 
			INTO @ids 
		END

		CLOSE @accountCursor ;
		DEALLOCATE @accountCursor;
	END
END
