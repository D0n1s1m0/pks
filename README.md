# Carsharing — единый проект КР №1 + КР №2

Это **один Java/Maven-проект**, который сохраняет интерактивное консольное приложение КР №1 и добавляет к нему графический интерфейс JavaFX для КР №2. Внутри нет двух отдельных копий исходников: консоль и JavaFX используют общие модели, правила, сервисы, репозитории и подключение JDBC.

## Самый простой запуск в Windows

1. Установи и запусти **Docker Desktop**. Установка Java 21 также необходима.
2. Распакуй весь архив в обычную папку.
3. Дважды нажми `start.bat`.

Скрипт автоматически запустит PostgreSQL в Docker, дождётся готовности базы, а затем соберёт и запустит JavaFX. При первом запуске Maven автоматически скачает Maven, если он не установлен, а затем скачает необходимые зависимости проекта. Поэтому при первом запуске нужен доступ к Интернету.

Для запуска исходного консольного меню КР №1 нажми `start-console.bat`. Для остановки только контейнера базы данных запусти `stop-database.bat`. Данные PostgreSQL сохраняются в Docker volume; обычная остановка контейнера их не удаляет.

## Запуск в Linux / macOS

Нужны Java 21, Docker с Docker Compose и доступ к Интернету при первой сборке. Из каталога проекта:

```bash
./start.sh
```

Консольная версия:

```bash
./start-console.sh
```

Лаунчер `mvnw` использует установленный Maven, если он есть, иначе автоматически скачивает Maven 3.9.16. Затем Maven загружает библиотеки, необходимые для сборки.

## Что есть в одном проекте

- **КР №1 — консольный интерфейс:** `ConsoleMain`, `ConsoleMenu`, операции с клиентами, автомобилями и арендой, поиск, фильтрация/сортировка, статистика и экспорт Excel/CSV.
- **КР №2 — JavaFX:** `Main`, FXML-формы, CSS-оформление, таблицы `TableView`, поиск, фильтры, сортировка, CRUD для автомобилей и клиентов, создание/завершение/отмена аренды, статистика и экспорт `.xlsx`/`.csv` через `FileChooser`.
- **Общая бизнес-логика:** `model`, `service`, `repository`, `exception` и `util`; оба интерфейса работают с одной PostgreSQL.
- **База данных:** `docker-compose.yml` запускает локальную PostgreSQL в контейнере. При первой инициализации пустого Docker volume выполняется `src/main/resources/sql/schema.sql`, который создаёт таблицы и добавляет демонстрационные записи (5 клиентов, 10 автомобилей и 10 аренд).
- **Материалы:** `rentals_export.xlsx`, `docs/er-diagram.png` и исходник диаграммы `docs/er-diagram.dot`.

## Как устроено подключение к БД

Настройки по умолчанию лежат в `src/main/resources/database.properties` и соответствуют базе из `docker-compose.yml`:

```properties
db.url=jdbc:postgresql://localhost:55432/carsharing
db.user=postgres
db.password=postgres
```

Эта конфигурация предназначена для локального учебного запуска на своём компьютере. Если требуется подключить существующую БД КР №1 вместо демонстрационной, поменяй только значения `db.url`, `db.user`, `db.password` в `database.properties` (либо задай переменные окружения `DB_URL`, `DB_USER`, `DB_PASSWORD`). Java-код менять не нужно. Схема внешней БД должна соответствовать таблицам `users`, `cars`, `rentals` и типу `rental_status`, который использует проект.

**Важно о сохранности данных:** SQL-файл `schema.sql` содержит `DROP TABLE` и `DROP TYPE`. Docker выполняет его автоматически только при первоначальном создании пустого хранилища. Не запускай этот SQL вручную на своей БД с нужными данными: он удаляет существующие таблицы. Не используй `docker compose down -v`, если требуется сохранить данные.

## Запуск командами Maven вручную

Если Maven уже установлен:

```bash
mvn clean javafx:run
```

Консольный режим:

```bash
mvn clean compile exec:java -Dexec.mainClass=com.carsharing.ConsoleMain
```

Проверка компиляции, тестов и сборка:

```bash
mvn clean compile
mvn test
mvn package
```

Команды можно выполнять через `mvnw` / `mvnw.cmd`, если Maven не установлен.

## Структура проекта

```text
karshering-main/
├── pom.xml
├── docker-compose.yml
├── start.bat / start-console.bat / stop-database.bat
├── start.ps1 / start-console.ps1
├── start.sh / start-console.sh
├── mvnw / mvnw.cmd
├── README.md
├── rentals_export.xlsx
├── docs/
│   ├── er-diagram.dot
│   └── er-diagram.png
└── src/
    ├── main/java/com/carsharing/
    │   ├── Main.java                 # JavaFX entry point
    │   ├── ConsoleMain.java          # console entry point
    │   ├── menu/                     # console UI
    │   ├── ui/                       # JavaFX controllers
    │   ├── model/                    # shared domain models and enum
    │   ├── service/                  # shared business rules
    │   ├── repository/               # shared JDBC data access
    │   ├── exception/                # domain exceptions
    │   └── util/                     # database and export utilities
    └── main/resources/
        ├── database.properties
        ├── fxml/
        ├── css/app.css
        └── sql/schema.sql
```

## Известные условия запуска

- Docker Desktop / Docker Engine должны быть запущены.
- Требуется установленная Java 21.
- Для первоначальной загрузки Maven и библиотек нужен Интернет.
- По умолчанию PostgreSQL опубликована на порту `55432`. Если этот порт уже занят, освободи его или измени опубликованный порт в `docker-compose.yml` и соответствующий адрес в `database.properties`.
- Полная сборка JavaFX зависит от разрешения Maven-зависимостей для ОС, на которой запускается проект.
