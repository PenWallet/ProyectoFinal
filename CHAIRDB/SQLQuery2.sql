SELECT nickname AS frname, frgame, privateProfile, [online], [admin] FROM GetFriendsWhoPlayMyGames('Penny')
SELECT TOP 3 UG.game, UG.hoursPlayed, UG.acquisitionDate, UG.lastPlayed, UG.playing, G.developer, G.libraryImageUrl FROM UserGames AS UG INNER JOIN Games AS G ON UG.game = G.name WHERE UG.[user] = 'Penny' AND lastPlayed IS NOT NULL AND lastPlayed > DATEADD(MONTH, -1, CURRENT_TIMESTAMP) ORDER BY UG.playing desc, UG.lastPlayed, UG.game
SELECT * FROM Games
UPDATE UserGames SET playing = 0 WHERE [user] = 'Migue' AND game = 'Portal'
SELECT * FROM UserGames
DELETE FROM Games WHERE name = 'Last Quest'

INSERT INTO Games ([name], developer, minimumAge, releaseDate, frontPage, storeImageUrl, libraryImageUrl)
	VALUES ('Last Quest', 'jolsensei', 3, '2019-06-14', 1, 'https://i.imgur.com/YvH79xF.png', 'https://i.imgur.com/cX6cVln.png')

ALTER TABLE Games DISABLE TRIGGER trg_ensureOnlyOneFrontPageGame
UPDATE Games SET description = 'Do you miss pixels? Then, remember good old times with Last Quest!

Last Quest is a Zelda-like ARPG filled with enemies, puzzles, items and enemies

Do you dare to face evil and bring peace back to the world?' WHERE name = 'Last Quest'
ALTER TABLE Games ENABLE TRIGGER trg_ensureOnlyOneFrontPageGame

INSERT INTO UserGames ([user], game, hoursPlayed, lastPlayed, acquisitionDate, playing)
	VALUES ('Penny', 'Last Quest', 3.14, '2019-01-25', '2018-10-25', 1)

UPDATE UserGames SET lastPlayed = '2019-01-20' WHERE [user] = 'Penny' AND game = 'Overwatch'

SELECT * FROM Games AS G
	LEFT JOIN UserGames AS UG
		ON G.name = UG.game
	WHERE UG.[user] = 'Penny'

DELETE FROM Games WHERE name IN ('Portal 3','Portal 4','Overwatch 2','Crashex Legends 2','Last Quest 2')

	INSERT INTO Games (name, description, developer, minimumAge, releaseDate, storeImageUrl, libraryImageUrl, frontPage) VALUES
('Portal 3', 'Best game eva but not really its just very very good', 'VALVe', 3, '2007-10-10', 'https://i.imgur.com/6FpgZlV.gif', '', 0),
('Portal 4', 'Best game eva 2 but not really its just very very good', 'VALVe', 3, '2011-04-18', 'https://i.imgur.com/8AJKwE9.png', '', 0),
('Overwatch 2', 'Play Mercy, like Medic in TF2', 'Blizzard', 10, '2016-05-24', 'https://i.imgur.com/M4WcKHc.png', '', 0),
('Crashex Legends 2', 'You will love our non-descriptive crash errors', 'Respawn Entertainment', 12, '2019-02-04', '', 'https://i.imgur.com/WbBV5KU.png', 0),
('Last Quest 2', 'Ya le he preguntao a Jorge, pero aún me la tiene que dar, así que hasta que me la de, tendré que inventarme algo para poner aquí, pero tiene que ser un texto largo y no me apetece poner un Lorem Ipsum porque queda bastante feote, pero bueno, que esto ya va marchando más o menos jeje', 'jolsensei', 3, '2019-06-14', 'https://i.imgur.com/YvH79xF.png', 'https://i.imgur.com/cX6cVln.png', 0)

SELECT UG.game, UG.hoursPlayed, UG.acquisitionDate, UG.lastPlayed, UG.playing, G.description, G.developer, G.minimumAge, G.releaseDate, G.frontPage, G.instructions, G.downloadUrl, G.storeImageUrl, G.libraryImageUrl FROM UserGames AS UG INNER JOIN Games AS G ON UG.game = G.name WHERE UG.[user] = 'Penny'

UPDATE Games SET frontPage = 1 WHERE name = 'Last Quest'

UPDATE Users SET online = 1 WHERE nickname = 'Migue'

SELECT * FROM IPBANS
DELETE FROM IPBans

DECLARE @algo INT
EXEC InsertNewFriendship 'Penny3', 'Migue3', @status = @algo OUTPUT
PRINT @algo

SELECT nickname, profileDescription, admin, [online], bannedUntil, banReason FROM Users WHERE nickname LIKE '%e%' AND nickname != 'Penny'

SELECT *
	FROM UserFriends AS UF
		INNER JOIN Users AS U
			ON (UF.user1 = U.nickname AND UF.user1 != 'Penny') OR (UF.user2 = U.nickname AND UF.user2 != 'Penny')
	WHERE user1 = 'Penny' OR user2 = 'Penny'

SELECT * FROM GetFriends('Penny')