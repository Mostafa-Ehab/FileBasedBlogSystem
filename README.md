# BlogSystem

## 📝 Overview

**BlogSystem** is a lightweight, file-based blogging platform built with **.NET 10 Minimal API** and structured using **Vertical Slice Architecture (VSA)**. It provides features like post creation, scheduled publishing, RSS feeds, image serving, tag and category organization, and basic user authentication — all without relying on a traditional database.

---

## ✨ Features

- **Post Management**
  Create, read, update, and schedule Markdown-based blog posts.

- **Image Serving**
  Serve images from `/Content/posts/*/assets/` using [ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web) with caching support.

- **RSS Feed**
  Generate an RSS 2.0 feed for published posts via `/posts/rss`.

- **Scheduled Publishing**
  Uses [Hangfire](https://www.hangfire.io/) with in-memory storage to publish posts at scheduled times (non-persistent).

- **Search**
  Perform keyword-based search on post content and tags using regular expressions.

- **Tags & Categories**
  Assign and organize posts by tags and categories for better filtering and navigation.

- **User Authentication**
  Basic user registration, login, and role-based route protection.

- **Static Frontend**
  A simple HTML/CSS/JS frontend served from `/wwwroot`.

---

## ⚙️ Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Any text editor or IDE (e.g., Visual Studio, VS Code)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Mostafa-Ehab/BlogSystem.git
cd BlogSystem
```

### 2. Configure JWT Secret

The application requires a JWT secret key for authentication. You need to configure this in the user secrets file:

**Location**: `%APPDATA%\Microsoft\UserSecrets\e2cfae03-2dbf-4911-838a-c6cf548020c6\secrets.json`

**Windows Path Example**: `C:\Users\{YourUsername}\AppData\Roaming\Microsoft\UserSecrets\e2cfae03-2dbf-4911-838a-c6cf548020c6\secrets.json`

Create the `secrets.json` file with the following content:

```json
{
  "JWT_SecretKey": "your-super-secret-jwt-key-here-make-it-long-and-secure"
}
```

> ⚠️ **Important**:
>
> - Replace `your-super-secret-jwt-key-here-make-it-long-and-secure` with a strong, unique secret key
> - The secret key should be at least 32 characters long for security
> - Never commit this secret to version control

**Alternative method using .NET CLI**:

```bash
cd BlogSystem
dotnet user-secrets set "JWT_SecretKey" "your-super-secret-jwt-key-here-make-it-long-and-secure"
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run --project BlogSystem/BlogSystem.csproj
```

> ⚠️ **Note**: Hangfire uses in-memory storage. Scheduled jobs will be cleared on application restart.

---

## 🌐 Usage

### Access the Application

- Frontend: [http://localhost:5000](http://localhost:5000)

### API Endpoints

| Method | Endpoint                          | Description                                 |
|--------|------------------------------------|---------------------------------------------|
| GET    | `/`                                | Serves the static frontend.                 |
| GET    | `/posts/rss`                       | Returns the RSS feed.                       |
| POST   | `/posts/{slug}/schedule`           | Schedule a post *(auth required)*.      |
| GET    | `/search?q={query}`                | Search posts and tags by keyword.           |
| GET    | `/images/posts/{slug}/{image}`     | Serve images embedded in posts.             |
| POST   | `/users/login`                     | Log in an existing user.                    |

---

## 📁 Project Structure

```plaintext
/Content/                  → Markdown content and post assets
/Domain/
├── Entities/              → Core domain models (Post, User, etc.)
├── Enums/                 → Domain enums (roles, status, etc.)

/Features/                 → Organized per feature (Vertical Slice)
├── Posts/
│   ├── CreatePost/
│   ├── GetPost/
│   ├── UpdatePost/
│   ├── SchedulePost/
│   ├── RSS/
│   └── Data/
├── Tags/
│   ├── CreateTag/
│   ├── GetTag/
│   └── Data/
├── Categories/
│   ├── CreateCategory/
│   ├── GetCategory/
│   └── Data/
└── Users/
    ├── CreateUser/
    ├── Login/
    └── Data/

/Infrastructure/
├── ImageService/          → ImageSharp integration
├── MarkdownService/       → Markdown parsing via Markdig
└── Scheduling/            → Hangfire abstraction layer

/Shared/
├── Exceptions/            → Custom exception handling
├── Extensions/            → Extension methods
├── Helpers/               → Utility classes
├── Mappings/              → AutoMapper profiles
└── Middlewares/           → Middleware components

/wwwroot/
├── index.html             → Homepage
├── post.html              → Single post template
├── tag.html               → Tag-based listing
├── category.html          → Category-based listing
└── css/, js/, images/     → Static assets
```

---

## 🔧 Configuration

- **JWT Authentication**: Configure the `JWT_SecretKey` in user secrets as described in the Getting Started section
- **ImageSharp.Web**: Configured in `Program.cs` for image caching and secure path handling
- **Hangfire**: Uses `InMemoryStorage` by default. For production, consider switching to a persistent provider (e.g., SQL Server)
- **AutoMapper**: Profiles are defined under `/Shared/Mappings/`.

---

## 🙌 Acknowledgments

- Built with **.NET 10.0**
- Uses:
  - [ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web)
  - [Hangfire](https://www.hangfire.io/)
  - [Markdig](https://github.com/xoofx/markdig)
