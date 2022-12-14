
GO
/****** Object:  StoredProcedure [dbo].[SpGetMostRecentDetails]    Script Date: 25-06-2019 15:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SpGetMostRecentDetails]
@USERID as varchar(max)

AS
BEGIN

SELECT mu.LOGIN_USER_ID as UserName,
	(SELECT TOP(1)
	LESSON_NAME from LESSON l
	JOIN USER_LESSON_COMPLETE as ulc on l.LESSON_ID=ulc.LESSON_ID
	JOIN [USER] as u on u.ID=ulc.USER_ID
	WHERE u.LOGIN_USER_ID = mu.LOGIN_USER_ID
	order by convert(datetime,ulc.USER_LESSON_COMPLETE_DATETIME,106) DESC) as mostRecentLesson,
	(SELECT TOP(1)
	USER_QUIZ_SCORE from USER_QUIZ_COMPLETE uqz
	JOIN [USER] as u on u.ID=uqz.USER_ID
	WHERE u.LOGIN_USER_ID = mu.LOGIN_USER_ID
	order by convert(datetime,uqz.USER_QUIZ_COMPLETE_DATETIME,106) DESC) as mostRecentScore
from [USER] mu
WHERE mu.LOGIN_USER_ID in (Select Item FROM [dbo].[SplitString](@USERID,','))
END


