-- DROP DATABASE CHAIRDB
CREATE DATABASE CHAIRDB;
USE CHAIRDB;

/* TABLES */
CREATE TABLE Users(
    nickname VARCHAR(20) NOT NULL,
    password VARCHAR(255) NOT NULL,
    profileDescription TEXT NULL,
    profileLocation TINYTEXT NULL,
    birthDate DATE NULL,
    privateProfile BOOLEAN DEFAULT 0,
    accountCreationDate DATE NULL,
    online BOOLEAN DEFAULT 0,
    admin BOOLEAN DEFAULT 0,
    bannedUntil DATETIME NULL,
    banReason TEXT NULL,
    CONSTRAINT PK_Users PRIMARY KEY(nickname)
);

CREATE TABLE Messages(
    ID BIGINT AUTO_INCREMENT NOT NULL,
    text TEXT NULL,
    sender VARCHAR(20) NOT NULL,
    receiver VARCHAR(20) NOT NULL,
    date DATETIME NULL,
    CONSTRAINT PK_Messages PRIMARY KEY (ID),
    CONSTRAINT FK_Messages_Users_Sender FOREIGN KEY (sender) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE NO ACTION,
    CONSTRAINT FK_Messages_Users_Receiver FOREIGN KEY (receiver) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE NO ACTION
);

-- The acceptedRequestDate is used to determine whether the receiver of the invitation has accepted
-- the request or not.
CREATE TABLE UserFriends(
    user1 VARCHAR(20) NOT NULL,
    user2 VARCHAR(20) NOT NULL,
    acceptedRequestDate DATETIME NULL,
    CONSTRAINT PK_UserFriends PRIMARY KEY (user1, user2),
    CONSTRAINT FK_UserFriends_User1 FOREIGN KEY (user1) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UserFriends_User2 FOREIGN KEY (user2) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE IPBans(
    IP VARCHAR(15) NOT NULL,
    untilDate DATETIME NULL,
    CONSTRAINT PK_IPBans PRIMARY KEY (IP)
);

CREATE TABLE Games(
    name VARCHAR(50) NOT NULL,
    minimumAge TINYINT NULL,
    releaseDate DATE NULL,
    instructions TEXT NULL,
    downloadUrl TEXT NULL,
    storeImageUrl TEXT NULL,
    libraryImageUrl TEXT NULL,
    CONSTRAINT PK_Games PRIMARY KEY (name)
);

CREATE TABLE UserGames(
    user VARCHAR(20) NOT NULL,
    game VARCHAR(50) NOT NULL,
    hoursPlayed DOUBLE NOT NULL DEFAULT 0,
    acquisitionDate DATETIME NULL,
    lastPlayed DATETIME NULL,
    CONSTRAINT PK_UserGames PRIMARY KEY (user, game),
    CONSTRAINT FK_UserGames_Users FOREIGN KEY (user) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UserGames_Games FOREIGN KEY (game) REFERENCES Games(name) ON UPDATE CASCADE ON DELETE CASCADE
);


/* TRIGGERS */
CREATE TRIGGER trg_SetCreationDate_BI BEFORE INSERT ON Users FOR EACH ROW
BEGIN
    SET NEW.accountCreationDate = CURDATE();
END;

CREATE TRIGGER trg_CheckBirthDateBeforeToday_BI BEFORE INSERT ON Users FOR EACH ROW
BEGIN
    IF(NEW.birthDate >= CURDATE())
    THEN
        SET NEW.birthDate = NULL;
    END IF;
END;

CREATE TRIGGER trg_CheckBirthDateBeforeToday_BU BEFORE UPDATE ON Users FOR EACH ROW
BEGIN
    IF(NEW.birthDate >= CURDATE())
    THEN
        SET NEW.birthDate = NULL;
    END IF;
END;




/* NOT POSSIBLE
CREATE TRIGGER trg_DeleteBan_AI AFTER INSERT ON IPBans FOR EACH ROW
BEGIN
    IF(untilDate IS NULL)
    THEN
        CREATE EVENT
    END IF;
END;
 */
