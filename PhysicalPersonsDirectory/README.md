# PhysicalPersonsDirectory

## Overview

PhysicalPersonsDirectory is a .NET 8.0 Web API for managing a directory of physical persons. It supports CRUD operations, image uploads, related persons management, search with pagination, reporting, and localization. The project uses Clean Architecture, Entity Framework Core with PostgreSQL, MediatR for command/query handling, and AutoMapper for DTO mapping.

## Features

- **CRUD Operations**: Create, read, update, and delete physical persons.
- **Image Upload**: Upload profile images for persons.
- **Related Persons**: Manage relationships between persons (e.g., family, colleagues).
- **Search & Pagination**: Search persons by criteria (name, personal number, etc.) with pagination.
- **Reporting**: Generate reports on related persons.
- **Localization**: Supports English (`en`) and Georgian (`ka`) via `.resx` files.
- **Repository/Unit of Work**: Implements data access patterns for better maintainability.

## Prerequisites

- **.NET 8.0 SDK**: Ensure the .NET 8.0 SDK is installed.
- **PostgreSQL**: A running PostgreSQL instance (e.g., locally or via Docker).
- **Git**: To clone the repository.
- **curl** or **Postman**: For testing API endpoints.

## Setup Instructions

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/<your-username>/PhysicalPersonsDirectory.git
   cd PhysicalPersonsDirectory
   ```

2. **Configure the Database**:

   - Update the connection string in `PhysicalPersonsDirectory.Api/appsettings.json`:

     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=PhysicalPersonsDb;Username=postgres;Password=yourpassword"
     }
     ```
   - Replace `yourpassword` with your PostgreSQL password.

3. **Apply Migrations**:

   ```bash
   cd PhysicalPersonsDirectory/src/PhysicalPersonsDirectory.Infrastructure
   dotnet ef database update --startup-project ../PhysicalPersonsDirectory.Api
   ```

4. **Run the Application**:

   ```bash
   cd ../PhysicalPersonsDirectory.Api
   dotnet run
   ```

   The API will be available at `http://localhost:5032`.

## Usage

- **Swagger UI**: Access the Swagger UI at `http://localhost:5032/swagger` for API documentation and testing.
- **Localization**: Set the `Accept-Language` header to `en` (English) or `ka` (Georgian) to receive localized responses.

### Example: Create a Physical Person

```bash
curl -X 'POST' \
  'http://localhost:5032/api/PhysicalPersons' \
  -H 'accept: */*' \
  -H 'Accept-Language: en' \
  -H 'Content-Type: application/json' \
  -d '{
    "firstName": "John",
    "lastName": "Gogoladze",
    "gender": "Male",
    "personalNumber": "12345678912",
    "dateOfBirth": "1990-01-01",
    "cityId": 1,
    "phoneNumbers": [
      { "type": "Mobile", "number": "+995555123456" }
    ]
  }'
```

**Expected Response**:

```json
{
  "Id": 1
}
```

## API Endpoints

- **POST /api/PhysicalPersons**: Create a new physical person.
- **GET /api/PhysicalPersons/{id}**: Retrieve a person by ID.
- **PUT /api/PhysicalPersons/{id}**: Update a person.
- **DELETE /api/PhysicalPersons/{id}**: Delete a person.
- **POST /api/PhysicalPersons/{id}/image**: Upload an image for a person.
- **GET /api/PhysicalPersons**: Search persons with pagination and filtering.
- **GET /api/PhysicalPersons/related-report**: Generate a report on related persons.

## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m "Add YourFeature"`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## License

This project is licensed under the MIT License.