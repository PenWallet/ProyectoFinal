USE master
GO
DROP DATABASE CHAIRDB
GO
CREATE DATABASE CHAIRDB
GO
USE CHAIRDB
GO

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
    hoursPlayed DECIMAL(10, 2) DEFAULT 0 NOT NULL ,
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
CREATE USER CHAIRMaster FOR LOGIN CHAIRMaster  
GO

/* TRIGGERS */
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
GO

/* TRIGGERS */
GO
CREATE TRIGGER trg_ensureOnlyOneFrontPageGame ON Games INSTEAD OF UPDATE 
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
				UPDATE Games SET frontPage = 1 WHERE name = @name
			END
		END
	END
GO

/* SOME TEST DATA */
INSERT INTO Users (nickname, [password], salt, birthDate, lastIP, [admin]) 
	VALUES	('Penny', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 1),
			('Migue', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 0),
			('Test', '0c0qYhFIv8qP2y9yXaHK1VCZgvQmZ/2TF5/ooNRSODc=', 'TlO2kHcldnFdtbJH2yNNPg==', '1999-12-13', '192.168.0.0', 0)

INSERT INTO Games (name, description, developer, minimumAge, releaseDate) VALUES
('Portal', 'Best game eva but not really its just very very good', 'VALVe', 3, '2007-10-10'),
('Portal 2', 'Best game eva 2 but not really its just very very good', 'VALVe', 3, '2011-04-18'),
('Overwatch', 'Play Mercy, like Medic in TF2', 'Blizzard', 10, '2016-05-24'),
('Crashex Legends', 'You will love our non-descriptive crash errors', 'Respawn Entertainment', 12, '2019-02-04')

GO

INSERT INTO UserGames ([user], game)
	VALUES	('Penny', 'Portal'),('Penny', 'Overwatch'),('Penny', 'Crashex Legends'),
			('Migue', 'Portal'),('Migue', 'Portal 2'),('Test', 'Crashex Legends'),
			('Test', 'Portal'),('Test', 'Portal 2')

INSERT INTO UserFriends(user1, user2)
	VALUES	('Penny', 'Migue'), ('Migue', 'Test'), ('Test', 'Penny')

/*
GUARDAR COMO ORO EN PAÑO CAGO EN DIO
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