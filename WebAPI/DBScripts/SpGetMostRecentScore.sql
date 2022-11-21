USE [fit_elearning]
GO
/****** Object:  StoredProcedure [dbo].[spGetQuizComplete]    Script Date: 10/18/2019 10:27:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Christian Stocker
-- Create date: 18 October 2019
-- Description:	Returns if user has scores
-- =============================================
CREATE PROCEDURE [dbo].[SpGetMostRecentDetails]

@USERNAME as varchar(250),
@StatusID as int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @maxUserQuizCompleteId int

    -- Insert statements for procedure here
	SELECT @maxUserQuizCompleteId = MAX(qc.USER_QUIZ_COMPLETE_ID) FROM USER_QUIZ_COMPLETE qc 
	LEFT JOIN [USER] u ON qc.USER_ID = u.ID 
	WHERE u.LOGIN_USER_ID = @USERNAME AND u.USER_STATUS_CODE = @StatusID AND qc.MODULE_ID = 1

	
	
	SELECT USER_QUIZ_COMPLETE_DATETIME, ml.LESSON_SORT_ORDER, qc.USER_QUIZ_SCORE
	FROM USER_QUIZ_COMPLETE qc 
	LEFT JOIN MODULE_LESSON ml ON qc.LESSON_ID = ml.LESSON_ID
	WHERE qc.USER_QUIZ_COMPLETE_ID = @maxUserQuizCompleteId



END
