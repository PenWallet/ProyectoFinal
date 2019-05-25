/*USE master
GO
DROP DATABASE CHAIRDB
GO
CREATE DATABASE CHAIRDB
GO
USE CHAIRDB
GO*/

/* TABLES */
CREATE TABLE Users(
    nickname VARCHAR(20) NOT NULL,
    [password] VARCHAR(255) NOT NULL,
	salt VARCHAR(255) NULL,
    profileDescription VARCHAR(MAX) NOT NULL DEFAULT '',
    profileLocation VARCHAR(MAX) NOT NULL DEFAULT '',
    birthDate DATE NOT NULL,
    privateProfile BIT DEFAULT 0,
    accountCreationDate DATE NOT NULL DEFAULT GETDATE(),
    [online] BIT DEFAULT 0,
    [admin] BIT DEFAULT 0,
	lastIP VARCHAR(15) NOT NULL DEFAULT '',
    bannedUntil DATETIME NULL,
    banReason VARCHAR(MAX) NOT NULL DEFAULT '',
    CONSTRAINT PK_Users PRIMARY KEY(nickname),
	CONSTRAINT CHK_BirthDateBeforeToday CHECK ( birthDate < GETDATE() ),
	CONSTRAINT CHK_BanUntilBanReason CHECK ( (bannedUntil IS NULL AND banReason = '') OR (bannedUntil IS NOT NULL AND banReason != '') )
)

CREATE TABLE [Messages](
    ID BIGINT IDENTITY(0, 1) NOT NULL,
    [text] VARCHAR(MAX) NOT NULL DEFAULT '',
    sender VARCHAR(20) NOT NULL,
    receiver VARCHAR(20) NOT NULL,
    date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_Messages PRIMARY KEY (ID),
    CONSTRAINT FK_Messages_Users_Sender FOREIGN KEY (sender) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE NO ACTION,
    CONSTRAINT FK_Messages_Users_Receiver FOREIGN KEY (receiver) REFERENCES Users(nickname) ON UPDATE NO ACTION ON DELETE NO ACTION
)

-- The acceptedRequestDate is used to determine whether the receiver of the invitation has accepted
-- the request or not.
CREATE TABLE UserFriends(
    user1 VARCHAR(20) NOT NULL,
    user2 VARCHAR(20) NOT NULL,
    acceptedRequestDate DATETIME NULL,
    CONSTRAINT PK_UserFriends PRIMARY KEY (user1, user2),
    CONSTRAINT FK_UserFriends_User1 FOREIGN KEY (user1) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UserFriends_User2 FOREIGN KEY (user2) REFERENCES Users(nickname) ON UPDATE NO ACTION ON DELETE NO ACTION
)

CREATE TABLE IPBans(
    [IP] VARCHAR(15) NOT NULL,
	banReason TEXT NULL,
    untilDate DATETIME NULL,
    CONSTRAINT PK_IPBans PRIMARY KEY (IP)
)

CREATE TABLE Games(
    [name] VARCHAR(50) NOT NULL,
	[description] TEXT NOT NULL DEFAULT '',
	developer VARCHAR(MAX) NOT NULL DEFAULT '',
    minimumAge INT NULL,
    releaseDate DATE NULL,
	frontPage BIT DEFAULT 0,
    instructions VARCHAR(MAX) NOT NULL DEFAULT '',
    downloadUrl VARCHAR(MAX) NOT NULL DEFAULT '',
    storeImageUrl VARCHAR(MAX) NOT NULL DEFAULT '',
    libraryImageUrl VARCHAR(MAX) NOT NULL DEFAULT '',
    CONSTRAINT PK_Games PRIMARY KEY (name)
)

CREATE TABLE UserGames(
    [user] VARCHAR(20) NOT NULL,
    game VARCHAR(50) NOT NULL,
    hoursPlayed DECIMAL(10, 3) DEFAULT 0 NOT NULL ,
    acquisitionDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    lastPlayed DATETIME NULL,
	playing BIT DEFAULT 0,
    CONSTRAINT PK_UserGames PRIMARY KEY ([user], game),
    CONSTRAINT FK_UserGames_Users FOREIGN KEY ([user]) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UserGames_Games FOREIGN KEY (game) REFERENCES Games(name) ON UPDATE CASCADE ON DELETE CASCADE
)

CREATE TABLE NicknameChanges(
	oldNickname VARCHAR(20) NOT NULL,
	newNickname VARCHAR(20) NOT NULL,
	dateChanged DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
)

-- Create user
/*CREATE USER CHAIRMaster FOR LOGIN CHAIRMaster  
GO*/

/* TRIGGERS 
-- Trigger used to save when a user changes its nickname
GO
CREATE TRIGGER trg_SaveNicknameChange ON Users AFTER UPDATE
AS
BEGIN
	DECLARE @oldNickname VARCHAR(20) = (SELECT nickname FROM deleted)
	DECLARE @newNickname VARCHAR(20) = (SELECT nickname FROM inserted)

	IF(@oldNickname != @newNickname COLLATE Latin1_General_CS_AS)
	BEGIN
		BEGIN TRANSACTION
			INSERT INTO NicknameChanges (oldNickname, newNickname) VALUES (@oldNickname, @newNickname)
		COMMIT
	END
END
GO*/

-- Trigger used to ensure that when an update is made to the frontPage field, all the others games have it set to 0
-- (there can only be one game in the front page) 
GO
CREATE TRIGGER trg_ensureOnlyOneFrontPageGame ON Games AFTER UPDATE
AS
	BEGIN
		IF(UPDATE(frontPage))
		BEGIN
			DECLARE @isFrontPage bit = (SELECT frontPage FROM inserted)
			DECLARE @wasFrontPage bit = (SELECT frontPage FROM deleted)
			IF(@isFrontPage = 1 AND @wasFrontPage = 0)
			BEGIN
				DECLARE @name varchar(50) = (SELECT name FROM inserted)
				UPDATE Games SET frontPage = 0 WHERE name != @name
			END
		END
	END
GO

GO
CREATE TRIGGER trg_updateLastPlayedWhenPlaying ON UserGames AFTER UPDATE
AS
	BEGIN
		IF(UPDATE(playing))
		BEGIN
			DECLARE @isPlaying bit = (SELECT playing FROM inserted)
			DECLARE @wasPlaying bit = (SELECT playing FROM deleted)
			IF((@isPlaying = 1 AND @wasPlaying = 0) OR (@isPlaying = 0 AND @wasPlaying = 1))
			BEGIN
				DECLARE @user varchar(20) = (SELECT [user] FROM inserted)
				DECLARE @game varchar(50) = (SELECT game FROM inserted)
				UPDATE UserGames SET lastPlayed = CURRENT_TIMESTAMP WHERE [user] = @user AND game = @game
			END
		END
	END
GO

/* FUNCTIONS */
-- Function that returns a table with all the friends that play my games for each game
GO
CREATE FUNCTION GetFriendsWhoPlayMyGames (@nickname varchar(20))
RETURNS TABLE
AS
RETURN
	WITH
	MyFriends_CTE (me, friend) AS (

		SELECT user1 AS me, user2 AS friend
		FROM UserFriends
		WHERE user1 = @nickname

		UNION

		SELECT user2 AS me, user1 AS friend
		FROM UserFriends
		WHERE user2 = @nickname
	),

	MyGames_CTE (myname, mygame) as  (
		SELECT [user] AS myname, game as mygame FROM UserGames 
			WHERE [user] = @nickname
	),

	MyFriendsGames_CTE (frname, frgame) as (

	 SELECT [user] AS frname, game as frgame
		FROM myfriends_cte AS M
			JOIN UserGames AS UG 
				ON M.friend = UG.[user]
	),

	MyFriendsThatPlayMyGames_CTE as(

	SELECT *
		FROM MyfriendsGames_CTE AS M
			LEFT JOIN myGames_CTE AS M2
				ON M.frgame = M2.mygame
		WHERE M2.mygame IS NOT NULL


	)

	SELECT U.nickname, frgame, U.privateProfile, U.[online], U.[admin] FROM MyFriendsThatPlayMyGames_CTE AS M
		INNER JOIN Users AS U
			ON M.frname = U.nickname
GO

-- Function that returns all friendships from an user, along with their nicknames, online and admin status
-- and the name of the game they're playing in case they are playing a game, otherwise it's null
GO
CREATE FUNCTION GetFriends (@nickname varchar(20))
RETURNS TABLE
AS
RETURN
	SELECT UF.user1, UF.user2, UF.acceptedRequestDate, U.nickname, U.[online], U.admin, UG.game
		FROM UserFriends AS UF
			INNER JOIN Users AS U
				ON (UF.user1 = U.nickname AND UF.user1 != @nickname) OR (UF.user2 = U.nickname AND UF.user2 != @nickname)
			LEFT JOIN UserGames AS UG
				ON UG.[user] = U.nickname AND UG.playing != 0
		WHERE (user1 = @nickname AND acceptedRequestDate IS NOT NULL) OR (user2 = @nickname AND acceptedRequestDate IS NOT NULL) OR (user2 = @nickname AND acceptedRequestDate IS NULL)
GO

/* PROCEDURES */
-- Procedure used to insert a new friendship between two users
-- Returns 1 if inserted, 0 if one of the users doesn't exists, -1 if a friendship already exists
GO
CREATE PROCEDURE InsertNewFriendship (@user1 VARCHAR(20), @user2 VARCHAR(20), @status INT OUTPUT)
AS
BEGIN
	IF(EXISTS(SELECT 1 FROM UserFriends WHERE user1 = @user1 AND user2 = @user2 OR user1 = @user2 AND user2 = @user1))
		BEGIN
			SET @status = -1;
		END
	ELSE
		BEGIN
			IF(EXISTS(SELECT 1 FROM Users WHERE nickname = @user1) AND EXISTS(SELECT 1 FROM Users WHERE nickname = @user2))
				BEGIN
					INSERT INTO UserFriends (user1, user2) VALUES (@user1, @user2)
					SET @status = 1
				END
			ELSE
				BEGIN
					SET @status = 0;
				END
		END
		
END
GO

/* SOME TEST DATA */
INSERT INTO Users (nickname, [password], salt, birthDate, lastIP, [admin]) 
	VALUES	('Penny', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 1),
			('Migue', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 0),
			('Test', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 0)

INSERT INTO Games (name, description, developer, minimumAge, releaseDate, storeImageUrl, libraryImageUrl, frontPage, downloadUrl) VALUES
('Portal', 'Best game eva but not really its just very very good', 'VALVe', 3, '2007-10-10', 'https://i.imgur.com/1X60875.jpg', 'https://i.imgur.com/6FpgZlV.gif', 0, ''),
('Portal 2', 'Best game eva 2 but not really its just very very good', 'VALVe', 3, '2011-04-18', 'https://i.imgur.com/KRAuJqV.jpg', 'https://i.imgur.com/8AJKwE9.png', 0, ''),
('Overwatch', 'Play Mercy, like Medic in TF2', 'Blizzard', 10, '2016-05-24', 'https://i.imgur.com/hNBUyoc.jpg', 'https://i.imgur.com/M4WcKHc.png', 0, ''),
('Crashex Legends', 'You will love our non-descriptive crash errors', 'Respawn Entertainment', 12, '2019-02-04', 'https://i.imgur.com/kG4nQo7.jpg', 'https://i.imgur.com/WbBV5KU.png', 0, ''),
('Last Quest', 'Do you miss pixels? Then, remember good old times with Last Quest!

Last Quest is a Zelda-like ARPG filled with enemies, puzzles, items and enemies

Do you dare to face evil and bring peace back to the world?', 'jolsensei', 3, '2019-06-14', 'https://i.imgur.com/YvH79xF.png', 'https://i.imgur.com/cX6cVln.png', 1, 'http://8music.ddns.net/ODeloitte/Last%20Quest.zip')

/*GO

INSERT INTO UserGames ([user], game)
	VALUES	('Penny', 'Portal'),('Penny', 'Overwatch'),('Penny', 'Crashex Legends'),
			('Migue', 'Portal'),('Migue', 'Portal 2'),('Test', 'Crashex Legends'),
			('Test', 'Portal'),('Test', 'Portal 2')

INSERT INTO UserFriends(user1, user2, acceptedRequestDate)
	VALUES	('Penny', 'Migue', '2019-05-23T21:14:00'), ('Migue', 'Test', NULL), ('Test', 'Penny', NULL)

INSERT INTO [Messages](sender, receiver, text, date)
	VALUES  ('Penny', 'Migue', 'Illo', '2019-05-23T21:15:16'), ('Penny', 'Migue', 'K ase', '2019-05-23T21:15:16'), ('Migue', 'Penny', 'Pos na, programacion', '2019-05-23T21:15:16'), 
			('Penny', 'Migue', 'Deja ya eso que es casi verano, cabróoooooooooooooooooon', '2019-05-23T21:15:16'), ('Migue', 'Penny', 'Tus muerto', '2019-05-23T21:15:16'), ('Migue', 'Penny', 'Gilipollas', '2019-05-23T21:15:16'), 
			('Penny', 'Migue', 'Ira e', '2019-05-23T21:15:16'), ('Migue', 'Penny', 'Que', '2019-05-23T21:15:16'), ('Penny', 'Migue', 'Retrasao', '2019-05-23T21:15:16')*/

/*
GUARDAR COMO ORO EN PA�O CAGO EN DIO
WITH
MyFriends_CTE (me, friend)as(

    SELECT user1 AS me, user2 AS friend
    FROM userFriends
    WHERE user1 = 'Penny'

    UNION

    SELECT user2 AS me, user1 AS friend
    FROM userFriends
    WHERE user2 = 'Penny'
),

MyGames_CTE (myname, mygame) as  (
	SELECT [user] AS myname, game as mygame FROM UserGames 
		WHERE [user] = 'Penny'
),

MyFriendsGames_CTE (frname, frgame) as (

 SELECT [user] AS frname, game as frgame
	FROM myfriends_cte AS M
		JOIN UserGames AS UG 
			ON M.friend = UG.[user]
),

MyFriendsThatPlayMyGames_CTE as(

SELECT *
	FROM MyfriendsGames_CTE AS M
		LEFT JOIN myGames_CTE AS M2
			ON M.frgame = M2.mygame
	WHERE M2.mygame is not null


)

select * from MyFriendsThatPlayMyGames_CTE
*/