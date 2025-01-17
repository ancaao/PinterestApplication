# 🌺 Pinterest Application

A social bookmarking website created using ASP.NET Core, MVC, Entity Framework Core, and Microsoft Identity Framework.

## ✨ Project Overview

Our project is a visual discovery engine for finding inspiration and ideas for specific interests, where users can share their experiences. 

The application features a login mechanism, allowing users to create accounts, log in, post content, create bookmark collections, like and comment on posts. It also includes effective search and sort functionalities, enabling users to find posts by title, description, or keywords and sort them based on the number of likes. Additionally, users can be granted administrator status with additional permissions and functionalities.

## 🧩 Project Structure

The project employs the MVC (Model-View-Controller) design pattern to effectively separate concerns within the application. The project is structured into the following packages:

- **Areas**: Contains user identity management created with the Microsoft Identity Framework, including all login and registration logic.
- **Controllers**: Manages HTTP requests and responses.
- **Data**: Contains the database context and migrations.
- **Models**: Specifies the data structures used by the program.
- **Views**: Uses Razor pages to display data to users.

The database is accessed via Entity Framework Core.

## 🧬 Features

- **📈 Comprehensive Feed**: A feed that displays all posts, featuring functionality to sort posts based on the number of likes and search posts by their titles, descriptions, or keywords.

![WhatsApp Image 2024-06-04 at 02 26 38](https://github.com/ancaao/PinterestApplication/assets/92025959/59ad685f-7ffd-4003-bea4-fdccbdc96cb2)

- **📌 Personal Board**: A dedicated personal board where users can save and organize their favorite posts.

![WhatsApp Image 2024-06-04 at 02 40 50](https://github.com/ancaao/PinterestApplication/assets/92025959/f9e36ec3-935d-48e0-8a30-10e14352c7db)


- **🖍 Account and Post Editing**: Users have the ability to edit their own accounts, posts, and comments, ensuring they can update their information and content as needed.

![WhatsApp Image 2024-06-04 at 02 41 58](https://github.com/ancaao/PinterestApplication/assets/92025959/b71b0cc1-e4d4-4343-a902-f592d3fed1c1)


- **💬 Interactive Engagement**: Users can engage with posts by liking or commenting, fostering interaction and community building within the platform.

![WhatsApp Image 2024-06-04 at 02 31 17](https://github.com/ancaao/PinterestApplication/assets/92025959/53a31dbd-7361-4155-b721-7f83c8e403a6)

- **🔗 Related Post Suggestions**: Each post includes suggestions for similar posts, helping users discover related content that matches their interests.
![WhatsApp Image 2024-06-04 at 02 30 41](https://github.com/ancaao/PinterestApplication/assets/92025959/ee99aa40-3658-4341-bd59-ada25e73dd86)



- **📂 Category-Based Search**: Users can search for posts by specific categories, making it easier to find content relevant to their needs and preferences.

  ![WhatsApp Image 2024-06-04 at 03 01 42](https://github.com/ancaao/PinterestApplication/assets/92025959/32675afd-1e13-486f-b242-7d89e74c019a)





