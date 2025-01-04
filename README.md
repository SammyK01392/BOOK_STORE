# BOOK_STORE
# Book Store Admin Panel

This is an Admin Panel for a Book Store built with ASP.NET Core MVC. The application allows admins to manage books, authors, categories, and users through a simple web interface. It also includes an authentication system to ensure only authorized users can access the admin section.

## Features

- **Admin Authentication**: Login and logout functionality for admins.
- **Manage Books**: Add, edit, delete, and view details of books.
- **Manage Authors**: Add, edit, delete, and view details of authors.
- **Dashboard**: Overview of important statistics like total books, authors, users, and orders.
- **Manage Users**: View and manage users of the system.

## Requirements

- **.NET Core 6+**
- **SQL Server** (or another database of your choice)
- **Visual Studio / Visual Studio Code** (for development)
- **Session Storage** for managing login sessions.

## Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/yourusername/book-store-admin.git
   
   **Install dependencies**:

Open the project in Visual Studio or Visual Studio Code and restore the required NuGet packages.

bash
Copy code
dotnet restore
Set up the database:

Create a new SQL Server database and update the connection string in appsettings.json to match your setup.

Apply migrations:

If using Entity Framework, run the migrations to set up the database schema.

bash
Copy code
dotnet ef database update
Run the application:

Start the application using Visual Studio or run the following command:

bash
Copy code
dotnet run
The application will be available at https://localhost:5001 (or the URL provided in the terminal).

Controllers
AdminController
Purpose: Handles admin login, logout, dashboard statistics, and user management.
Actions:
Login(): Displays the login page. If an admin is already logged in, it redirects to the dashboard.
Login(User user): Authenticates an admin user and sets session data.
Logout(): Logs out the admin user by clearing the session data.
Dashboard(): Displays statistics about books, authors, categories, users, and orders.
Users(): Displays a list of all users in the system.
BooksController
Purpose: Manages book-related functionalities, including creating, editing, deleting, and viewing books.
Actions:
Index(int page, int pageSize): Displays a list of books with pagination.
Details(int? id): Shows the details of a specific book.
Create(): Displays the form to create a new book.
Create(BookView book): Handles the POST request to create a new book.
Edit(int? id): Displays the form to edit an existing book.
Edit(int id, Book book): Handles the POST request to update an existing book.
Delete(int? id): Displays the confirmation page to delete a book.
DeleteConfirmed(int id): Deletes the selected book.
AuthorsController
Purpose: Manages author-related functionalities, including adding, editing, and deleting authors.
Actions:
Index(): Displays a list of all authors.
Details(int? id): Shows the details of a specific author.
Create(): Displays the form to create a new author.
Create(Author author): Handles the POST request to create a new author.
Edit(int? id): Displays the form to edit an existing author.
Edit(int id, Author author): Handles the POST request to update an existing author.
Delete(int? id): Displays the confirmation page to delete an author.
DeleteConfirmed(int id): Deletes the selected author.
Database Model
The database models are based on the BookStoreDbContext which includes the following tables:

Users: Stores user data, including login credentials and roles.
Books: Stores book information, including title, description, price, stock, and references to authors and categories.
Authors: Stores author data such as name and biography.
Categories: Stores book categories.
Orders: Stores order data for users.
Configuration
The configuration for the project can be found in the appsettings.json file. The most important configuration settings are:

ConnectionString: Defines the connection to the database.
Logging: Logs application errors and information.
Session Timeout: Defines how long a session is valid.
Contributing
If you would like to contribute to the project, please fork the repository, make your changes, and submit a pull request. All contributions are welcome.

License
This project is licensed under the MIT License - see the LICENSE file for details.

Thank you for checking out this Book Store Admin Panel! Enjoy managing your book store.

markdown
Copy code

### Explanation:

- **Features**: Lists the key features of the application, such as admin authentication, book management, and the dashboard.
- **Installation**: A step-by-step guide on how to set up and run the project locally.
- **Controllers**: Describes the actions in the `AdminController`, `BooksController`, and `AuthorsController` which manage user authentication, book management, and author management, respectively.
- **Database Model**: Briefly describes the main models used in the database (`Users`, `Books`, `Authors`, `Categories`, `Orders`).
- **Contributing**: A note for people interested in contributing to the project.
- **License**: Assumes MIT License for the project. You can modify it to reflect the actual license you are using.

This `README.md` will help guide others on how to set up and contribute to your project while providing an overview of the key controllers and features.



