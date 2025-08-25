<div id="top">
<div align="center">

<img src="/ShoppeWebApp/wwwroot/images/Icons/shoppe_logo512.png" width="200" alt="Project Logo"/>

# Shoppe E-Commerce

## 🚀 Built with the following technologies

![ASP.NET MVC](https://img.shields.io/badge/ASP.NET%20MVC-512BD4?style=flat&logo=.net&logoColor=white)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=flat&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat&logo=css3&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat&logo=javascript&logoColor=black)
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=flat&logo=bootstrap&logoColor=white)


</div>
<br>

---

## Table of Contents

- [Table of Contents](#table-of-contents)
- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Usage](#usage)
    - [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

---

## Overview

The purpose of this project is to design and develop an **e-commerce website** that provides essential functionalities such as:

- User registration and authentication  
- Product management  
- Shopping cart  
- Order placement and management  
- Admin Dashboard for system administration  

The website is designed with a focus on **user-friendliness, security, and scalability**, ensuring that it is easy to use, protects user data, and can be extended in the future.  

### Objectives
- Build an intuitive and user-friendly interface.  
- Implement core functionalities such as user, product, and order management.  
- Support buyers and sellers in performing basic e-commerce transactions efficiently.  
- Ensure system and user data security.  

### Scope and Target Users
The project focuses on developing a basic **e-commerce platform** suitable for small to medium-sized businesses.  
Target users include: **customers, sellers, and system administrators**.  

### Research Methodology
The project applies **system analysis** and **object-oriented design** methodologies.  
It is implemented using web technologies such as **HTML, CSS, JavaScript**, combined with **ASP.NET Core MVC** for the application layer.  
**MySQL** is used as the database management system to store and retrieve data.  

---

## Features

- User registration and authentication  
- Shopping cart management  
- Product and order management  
- Admin Dashboard for system administration  
- User-friendly and responsive UI with Bootstrap  
- Security-focused design to protect user data  
- Scalable architecture for future enhancements  

---

## Project Structure

```sh
└── ShoppeWebApp/
    ├── appsettings.json
    ├── Areas
    │   ├── Admin
    │   ├── Customer
    │   └── Seller
    ├── Constraints
    │   └── AdminConstraint.cs
    ├── Controllers
    │   ├── AuthenticationController.cs
    │   └── HomeController.cs
    ├── Data
    │   └── ShoppeWebAppDbContext.cs
    ├── Migrations
    │   └── [Entity Framework Core migrations...]
    ├── Models
    │   ├── Chitietdonhang.cs
    │   ├── Cuahang.cs
    │   ├── Danhgia.cs
    │   ├── Danhmuc.cs
    │   ├── Donhang.cs
    │   ├── Giohang.cs
    │   ├── Nguoidung.cs
    │   ├── Sanpham.cs
    │   ├── Taikhoan.cs
    │   └── Thongtinlienhe.cs
    ├── Program.cs
    ├── Properties
    │   └── launchSettings.json
    ├── Services
    │   ├── DatabaseChecker.cs
    │   ├── ImageUpload.cs
    │   ├── PagingLoad.cs
    │   ├── PasswordHasher.cs
    │   └── Quantity.cs
    ├── ShoppeWebApp.csproj
    ├── ViewModels
    │   ├── Admin
    │   ├── Authentication
    │   ├── Customer
    │   ├── Seller
    │   └── ErrorViewModel.cs
    ├── Views
    │   ├── _ViewImports.cshtml
    │   ├── _ViewStart.cshtml
    │   ├── Authentication
    │   ├── Home
    │   └── Shared
    └── wwwroot
        ├── css
        ├── icons
        ├── images
        ├── js
        └── shoppe_logo32.ico
```

## Getting Started

Follow the steps below to set up and run the project locally.

---

### Prerequisites

Before you begin, make sure you have the following installed:

- **Programming Language:** C#  
- **.NET SDK:** [.NET 8.0 LTS](https://dotnet.microsoft.com/download/dotnet/8.0)  
- **Package Manager:** [NuGet](https://www.nuget.org/)  
- **Database:** [MySQL](https://dev.mysql.com/downloads/)  
- **IDE (optional but recommended):** [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)  

---

### Installation

1. **Clone the repository**
   ```sh
   git clone https://github.com/GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe.git
   ```


2. **Navigate to the project directory**  
   ```sh
   cd PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe
   ```

3. **Restore dependencies:**  
   ```sh
   dotnet restore
   ```

4. **Install required NuGet packages:**
Run the following commands to install the necessary dependencies:
	```sh
	dotnet add package Hangfire --version 1.8.20
	dotnet add package Hangfire.MySqlStorage --version 2.0.3
	dotnet add package Microsoft.EntityFrameworkCore --version 8.0.15
	dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.15
	dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.3
	```

5. **Configure database connection:**  
   Open the `appsettings.json` file and update the connection string:  

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ShoppeDB;User Id=root;Password=yourpassword;"
   }
   ```

6. **Apply database migrations:**  
   Run the following command to create and apply migrations to the database:

   ```sh
   dotnet ef database update
   ```

### Usage

Run the project with:
```sh
dotnet run
```

### Testing

Shoppe uses the xUnit test framework (replace with your actual one if different). Run the test suite with:
```sh
dotnet test
```

## Contributing

- **💬 [Join the Discussions](https://github.com/GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe/discussions)**: Share your insights, provide feedback, or ask questions.  
- **🐛 [Report Issues](https://github.com/GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe/issues)**: Submit bugs found or log feature requests for the `Shoppe` project.  

<details closed>
<summary>Contributing Guidelines</summary>

1. **Fork the Repository**: Start by forking the project repository to your GitHub account.  
2. **Clone Locally**: Clone the forked repository to your local machine using a git client.  
   ```sh
   git clone https://github.com/GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe.git
   ```
3. **Create a New Branch**: Always work on a new branch, giving it a descriptive name.
   ```sh
   git checkout -b feature/new-feature-x
   ```
4. **Make Your Changes**: Develop and test your changes locally.
5. **Commit Your Changes**: Commit with a clear message describing your updates.
   ```sh
   git commit -m 'Implemented new feature x.'
   ```
6. **Push to LOCAL**: Push the changes to your forked repository.
   ```sh
   git push origin new-feature-x
   ```
7. **Submit a Pull Request**: Create a PR against the original project repository. Clearly describe the changes and their motivations.
8. **Review**: Once your PR is reviewed and approved, it will be merged into the main branch. Congratulations on your contribution!
</details>

<details closed>
<summary>Contributor Graph</summary>
<br>
<p align="left">
   <a href="https://github.com/GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe/graphs/contributors">
      <img src="https://contrib.rocks/image?repo=GuenNhatKing/PBL-Nen-tang-mua-sam-truc-tuyen-Shoppe">
   </a>
</p>
</details>

---

## License

This project is licensed under the terms of the [MIT License](./LICENSE).  
See the [LICENSE](./LICENSE) file for full details.

## Acknowledgments

- Thanks to all contributors who have participated in this project.
- Inspired by various open-source e-commerce platforms and community projects.
- References: 
  - [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)  
  - [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)  
  - [MySQL Documentation](https://dev.mysql.com/doc/)  

<div align="right">

[![][back-to-top]](#top)

</div>

[back-to-top]: https://img.shields.io/badge/-BACK_TO_TOP-151515?style=flat-square


---
