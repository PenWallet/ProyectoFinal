-- DROP DATABASE CHAIRDB
CREATE DATABASE CHAIRDB;
USE CHAIRDB;

/* TABLES */
CREATE TABLE Users(
    nickname VARCHAR(20) NOT NULL,
    password VARCHAR(255) NOT NULL,
    profileDescription TEXT NULL,
    profileLocation TINYTEXT NULL,
    birthDate DATE NOT NULL,
    privateProfile BOOLEAN DEFAULT 0,
    accountCreationDate DATE,
    admin BOOLEAN DEFAULT 0,
    bannedUntil DATETIME NULL DEFAULT NULL,
    banReason TEXT NULL DEFAULT NULL,
    CONSTRAINT PK_Users PRIMARY KEY(nickname)
);

CREATE TABLE Messages(
    ID BIGINT AUTO_INCREMENT,
    text TEXT,
    sender VARCHAR(20),
    receiver VARCHAR(20),
    date DATETIME,
    CONSTRAINT PK_Messages PRIMARY KEY (ID),
    CONSTRAINT FK_Messages_Users_Sender FOREIGN KEY (sender) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE NO ACTION,
    CONSTRAINT FK_Messages_Users_Receiver FOREIGN KEY (receiver) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE NO ACTION
);

CREATE TABLE UsersFriends(
    user1 VARCHAR(20) NOT NULL,
    user2 VARCHAR(20) NOT NULL,
    acceptedRequest BOOLEAN DEFAULT 0,
    CONSTRAINT PK_UsersFriends PRIMARY KEY (user1, user2),
    CONSTRAINT FK_UsersFriends_User1 FOREIGN KEY (user1) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UsersFriends_User2 FOREIGN KEY (user2) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE IPBans(
    IP VARCHAR(15) NOT NULL,
    untilDate DATETIME NULL,
    CONSTRAINT PK_IPBans PRIMARY KEY (IP)
);

CREATE TABLE Games(
    name VARCHAR(50) NOT NULL,
    minimumAge TINYINT,
    instructions TEXT,
    storeImageUrl TEXT,
    libraryImageUrl TEXT,
    CONSTRAINT PK_Games PRIMARY KEY (name)
);

CREATE TABLE UsersGames(
    user VARCHAR(20) NOT NULL,
    game VARCHAR(50) NOT NULL,
    hoursPlayed DOUBLE NOT NULL DEFAULT 0,
    lastPlayed DATETIME NULL,
    CONSTRAINT PK_UsersGames PRIMARY KEY (user, game),
    CONSTRAINT FK_UsersGames_Users FOREIGN KEY (user) REFERENCES Users(nickname) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_UsersGames_Games FOREIGN KEY (game) REFERENCES Games(name) ON UPDATE CASCADE ON DELETE CASCADE
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
