/*
  Ручной сброс данных в SQL Server Management Studio (не для PostgreSQL / pgAdmin).
  Выполни в базе sql17 (или как у тебя называется каталог), затем при необходимости
  снова запусти приложение — оно подхватит пустые таблицы и сидер добавит пользователей и мойки.

  Порядок важен из-за внешних ключей.
*/

DELETE FROM [Bookings];
DELETE FROM [Users];
DELETE FROM [CarWashes];

INSERT INTO [CarWashes] ([Name], [Address])
VALUES
  (N'Майская', N'Майская, 51'),
  (N'Песочная', N'Песочная улица, 38е'),
  (N'Автозаводская', N'Автозаводская, 1Б');

INSERT INTO [Users] ([Login], [Password])
VALUES
  (N'ivan', N'1234'),
  (N'olga', N'1234');
