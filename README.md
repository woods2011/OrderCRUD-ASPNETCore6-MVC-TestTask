# Тестовое задание

CRUD приложение, для создания, удаления и редактирования заказов.

## Приложение

Приложение разработано на ASP.NET Core 6.0 (MVC) с использованием Entity Framework Core 6.0 и MS SQL Server.  
Для интеграционного тестирования использовалась SQLite.

## Запуск проекта

Для запуска проекта необходимо установить Docker Desktop и выполнить docker-compose в директории с проектом:

```sh
docker-compose up -d
```

Если же вы хотите запустить проект без Docker, то необходимо перейти в  
Program.cs и изменить предустановленную строку подключения на свою:
```csharp
options.UseSqlServer(builder.Configuration["MvcAppTestConnectionString"]);
```