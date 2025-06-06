# 📚 Library Management API

**Library Management API** is a modular and scalable RESTful API designed to manage library operations, including book inventory, user management, and borrowing records. Built with **ASP.NET Core** and adhering to **Clean Architecture** principles, this project ensures maintainability and testability.

---

## 📦 Features

- 🔄 CRUD operations for books and users
- 📚 Manage borrowing and returning of books
- 🧱 Clean Architecture (Domain, Application, Infrastructure, API)
- 🔐 JWT-based authentication and authorization
- 📄 Layered and scalable project structure

---

## 🛠️ Tech Stack

- **Language:** C#
- **Framework:** ASP.NET Core
- **Architecture:** Clean Architecture
- **Database:** (SQL Server)
- **ORM:** Entity Framework Core
- **Tools:** Swagger (for API documentation), Visual Studio / Visual Studio Code

---

## 📁 Project Structure

```
LibraryManagementAPI/
├── API-Structure/           # Presentation layer (Controllers, Startup)
├── API-Structure.Core/      # Business logic and use cases
├── API-Structure.EF/        # Data access and Entity Framework configurations
├── BookManagementSystem.sln # Solution file
└── README.md                # Project documentation
```

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/MahmoudSami0/LibraryManagementAPI.git
```

### 2. Open the Project

Open `BookManagementSystem.sln` using **Visual Studio 2022** or later.

### 3. Configure the Database

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Your-Connection-String-Here"
}
```

### 4. Configure JWT Authentication

Add JWT settings in `appsettings.json`:

```json
"JwtSettings": {
  "SecretKey": "YourSecretKey",
  "Issuer": "YourIssuer",
  "Audience": "YourAudience",
  "DurationInMinutes": 60
}
```

### 5. Apply Migrations

From the CLI:

```bash
cd API-Structure.EF
dotnet ef database update
```

Or from **Package Manager Console** (ensure `API-Structure.EF` is the startup project):

```powershell
Update-Database
```

### 6. Run the Application

```bash
dotnet run --project API-Structure
```

The API will start on `https://localhost:5001` or `http://localhost:5000`.

---

## 📬 API Endpoints (Examples)

| Method | Endpoint             | Description                  |
|--------|----------------------|------------------------------|
| GET    | `/api/books`         | Retrieve all books           |
| GET    | `/api/books/{id}`    | Retrieve a book by ID        |
| POST   | `/api/books`         | Add a new book               |
| PUT    | `/api/books/{id}`    | Update an existing book      |
| DELETE | `/api/books/{id}`    | Delete a book                |
| GET    | `/api/users`         | Retrieve all users           |
| POST   | `/api/borrow`        | Borrow a book                |
| POST   | `/api/return`        | Return a borrowed book       |

---

## 🤝 Contribution

Contributions are welcome! Feel free to fork the repository and submit pull requests.

1. Fork the repository  
2. Create a new branch  
   ```bash
   git checkout -b feature/your-feature
   ```
3. Commit your changes  
4. Push to your branch  
   ```bash
   git push origin feature/your-feature
   ```
5. Open a pull request

---

## 👤 Author

**Mahmoud Sami**  
GitHub: [@MahmoudSami0](https://github.com/MahmoudSami0)  
Email: ms4805727@gmail.com

---
