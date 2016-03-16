IF Object_id('USP_RentCalcSchedule', 'P') IS NOT NULL 
  DROP PROC USP_RentCalcSchedule 

go 

CREATE PROC USP_RentCalcSchedule(@DT_SELDATE DATETIME) 
AS 
SET NOCOUNT ON
  BEGIN 
      IF NOT EXISTS(SELECT NULL 
                    FROM   JobStatus 
                    WHERE  JobType = 1 
						   AND DATEADD(dd, 0, DATEDIFF(dd, 0, RunDate)) = 
                           DATEADD(dd, 0, DATEDIFF(dd, 0, 
                                          @DT_SELDATE))) 
        BEGIN 
            INSERT INTO RentDue 
                        (UserID, 
                         PropertyID, 
                         DueMon, 
                         DueYear, 
                         DueDate, 
                         DueAmount, 
                         [Description], 
                         DueStatus, 
                         Createdon) 
            SELECT T.UserID, 
                   P.PropertyID, 
                   MONTH(@DT_SELDATE), 
                   Year(@DT_SELDATE), 
                   @DT_SELDATE, 
                   T.RentAmount + ISNULL(T.PetRentAmount, 0), 
                   CONCAT('Rent ', Datename(MONTH, @DT_SELDATE), ' - ', 
                   Year(@DT_SELDATE)), 
                   1, 
                   GETDATE() 
            FROM   Properties P 
                   INNER JOIN Tenants T 
                           ON P.PropertyID = T.PropertyID 
            WHERE  DATEADD(MONTH, DATEDIFF(MONTH, 0, @DT_SELDATE), 0) < 
                          DATEADD(MONTH, DATEDIFF(MONTH, 0, T.FinalDueDate), 0) 
                   AND P.RentDueDay = Day(@DT_SELDATE) 

            IF DAY(@DT_SELDATE) = 15 
              BEGIN 
                  INSERT INTO RentDue 
							(UserID, 
							 PropertyID, 
							 DueMon, 
							 DueYear, 
							 DueDate, 
							 DueAmount, 
							 [Description], 
							 DueStatus, 
							 Createdon) 
                  SELECT T.UserID, 
						 P.PropertyID, 
						 MONTH(@DT_SELDATE), 
						 Year(@DT_SELDATE), 
						 @DT_SELDATE, 
						 T.RentAmount, 
						 CONCAT('Rent ', Datename(MONTH, @DT_SELDATE), ' - ', 
						 Year(@DT_SELDATE)), 
						 1, 
						 GETDATE() 
                  FROM   Properties P 
                         INNER JOIN Tenants T 
                                 ON P.PropertyID = T.PropertyID 
                  WHERE  DATEADD(MONTH, DATEDIFF(MONTH, 0, @DT_SELDATE), 0) = 
                         DATEADD(MONTH, DATEDIFF(MONTH, 0, T.FinalDueDate), 0) 
              END 

            INSERT INTO RentDue 
                        (UserID, 
                         PropertyID, 
                         DueDate, 
                         DueAmount, 
                         [Description], 
                         DueStatus, 
                         Createdon) 
            SELECT T.UserID, 
                   P.PropertyID, 
                   @DT_SELDATE, 
                   T.Deposit, 
                   'Deposit', 
                   1, 
                   GETDATE() 
            FROM   Properties P 
                   INNER JOIN Tenants T 
                           ON P.PropertyID = T.PropertyID 
            WHERE  T.DepostDueDate IS NOT NULL 
                   AND ( DATEADD(dd, 0, DATEDIFF(dd, 0, T.DepostDueDate)) = ( 
                         DATEADD(dd, 0, 
                             DATEDIFF(dd, 0, @DT_SELDATE)) ) )
			INSERT INTO JobStatus(JobType, JobName, RunDate) VALUES(1, 'Daily Rent Calc', @DT_SELDATE);
        END 
SET NOCOUNT OFF        
  END 
GO