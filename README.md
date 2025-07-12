# BlogSystem

## 📝 Overview

**BlogSystem** is a lightweight, file-based blogging platform built with **.NET 10 Minimal API** and structured using **Vertical Slice Architecture (VSA)**. It provides features like post creation, scheduled publishing, RSS feeds, image serving, tag and category organization, and basic user authentication — all without relying on a traditional database.

---

## ✨ Features

### User Roles and Permissions

The system supports role-based access control with four distinct user roles:

- Admin
- Editor
- Author
- Viewer

Each role has specific permissions to manage blog posts, users, and access the admin panel. The default role is **Viewer**, which allows reading published posts.

### Post Management

- Create, read, update, and delete Markdown-based blog posts.
- Supports image serving from `/Content/posts/*/assets/` using [ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web).
- Scheduled publishing using [Hangfire](https://www.hangfire.io/) with in-memory storage (non-persistent).
- Generate an RSS 2.0 feed for published posts via `/rss`.
- Save as draft or publish immediately.

### Tag and Category Management

- Assign and organize posts by tags and categories for better filtering and navigation.

---

### Admin Role

- **Post Management**
  Create, read, update, and delete any Markdown-based blog posts.
- **User Management**
  Create, read and update users with role-based access control.

### Editor Role

- **Post Management**
  Create, read, update, and delete any Markdown-based blog posts.

### Author Role

- **Post Management**
  Create, read, update, and delete their own Markdown-based blog posts.

### Viewer Role (default role)

- **Post Viewing**
  Read and view published Markdown-based blog posts.
- **Tag and Category Browsing**
  View posts filtered by tags and categories.

---

## 🚀 Future Enhancements

- A separate page for managing tags and categories.
- Implement server side pagination for admin panel instead of client-side pagination.
- Implement paginations for the viewing public posts.
- Implement search functionality for posts, tags, and categories.

---

## ⚙️ Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Any text editor or IDE (e.g., Visual Studio, VS Code)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Mostafa-Ehab/FileBasedBlogSystem.git
cd FileBasedBlogSystem
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
cd FileBasedBlogSystem
dotnet user-secrets set "JWT_SecretKey" "your-super-secret-jwt-key-here-make-it-long-and-secure"
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Seed the Database with Initial Data

The application uses a file-based approach for posts, tags, and categories. To seed initial data, run the following command:

```bash
dotnet run --project DevTools -- seed
```

### 5. Run the Application

```bash
dotnet run --project BlogSystem
```

> ⚠️ **Note**: Hangfire uses in-memory storage. Scheduled jobs will be cleared on application restart.

---

## 🌐 Usage

### Access the Application

- Frontend: [http://localhost:5000](http://localhost:5000)

### Admin Login

- Admin Panel: [http://localhost:5000/admin](http://localhost:5000/admin/login.html)

To log in to the admin panel, use the following credentials:

- Admin Role:
  - **Username**: `john-doe`
  - **Password**: `password123`
- Author Role:
  - **Username**: `jane-doe`
  - **Password**: `password123`

> No Editors created by default. You can create them via the admin panel.

---

## 📁 Project Structure

```plaintext
/Content/                  → Markdown content and post assets
/Domain/
├── Entities/              → Core domain models (Post, User, etc.)
└── Enums/                 → Domain enums (roles, status, etc.)

/Features/                 → Organized per feature (Vertical Slice)
├── Posts/
│   ├── PostManagement/
│   ├── GetPost/
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
    ├── UpdateUser/
    ├── GetUser/
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
├── Mappings/              → Exctension methods for mapping
└── Middlewares/           → Middleware components

/wwwroot/
├── admin/                 → Admin panel static files
│   ├── index.html          → Admin dashboard
│   ├── logout.html         → Admin logout page
│   ├── login.html          → Admin login page
│   ├── posts.html          → Admin posts management page
│   ├── tags.html           → Admin tags management page
│   └── users.html          → Admin users management page
├── images/                → Image assets
├── scss/                  → SCSS files
├── css/                   → Compiled CSS files
├── js/                    → JavaScript files
│   ├── blog/                → Blog-specific scripts
│   ├── admin/               → Admin panel scripts
│   └── shared/              → Shared scripts
├── index.html             → Homepage
├── post.html              → Single post template
├── tag.html               → Tag-based listing
└── category.html          → Category-based listing
```

---

## 🙌 Acknowledgments

- Built with **.NET 10.0**
- Uses:
  - [ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web)
  - [Hangfire](https://www.hangfire.io/)
  - [Markdig](https://github.com/xoofx/markdig)
