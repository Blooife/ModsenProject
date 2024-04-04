# Task for Modsen Company

## Как запустить проект

1. **Клонируйте репозиторий:**

   ```sh
   git clone https://github.com/Blooife/ModsenProject

## Запустить клиент

1. **Перейдите в папку frontapp**
   ```sh
   cd frontapp
2. **Установите зависимости**
    ```sh
   npm install
3. **Запустите проект**
    ```sh
   npm start
4. **В браузере перейдите**
   ```sh
   http://localhost:3000

## Запустить сервер

1. **Перейдите в папку Api**
   ```sh
   cd Api
2. **Запустите сервер**
    ```sh
   dotnet run
3. **В браузере перейдите**
   ```sh
   http://localhost:5152/swagger/index.html
4. **Для корректной работы в appsettings.json укажите строку подключения к PostgreSql базе данных**
5. **Создать базу данных через CLI**
   ```sh
   dotnet ef database update --project Infrastructure\Infrastructure.csproj --startup-project Api\Api.csproj --context AppDbContext --configuration Debug 20240404004625_initial

   
