## Description

This is a full-stack web application for managing tasks, built using .NET for the backend and PostgreSQL for the database. The application features secure JWT-based authentication.

### Running Locally
To run the application locally:

1. Add the `aspnetapp.pfx` certificate to the `src/TaskManager.Backend/TaskManagerApi/certificates` folder.
2. Place the `.env` file in the `src/TaskManager.Backend` folder. [Download example .env file](https://drive.google.com/file/d/1NEuj7b5lsgqk0zx-oALrS4Bp8cmedziP/view?usp=sharing).
3. Navigate to the `src/TaskManager.Backend` directory and run Docker Compose through TaskManager.Backend.sln.

### API URLs
https://taskmanager-api-germanywestcentral-001.azurewebsites.net/

## User API Endpoint Examples

### Register
```bash
[POST] /users/register
```
**Request Body**:
```bash
{
  "userName": "example",
  "email": "example@gmail.com",
  "password": "123456;QWERTY",
  "confirmPassword": "123456;QWERTY"
}
```

### Login
```bash
[POST] /users/login
```
**Request Body**:
```bash
{
  "login": "example",
  "password": "123456QWERTY"
}
```

### Refresh Token
```bash
[POST] /users/refresh
```
**Request Body**:
```bash
{
  "accessToken": "{{accessToken}}",
  "refreshToken": "{{refreshToken}}"
}
```

### Update User
```bash
[PUT] /users/update
```
**Request Body**:
```bash
{
  "userName": "example",
  "oldEmail": "example@gmail.com",
  "newEmail": "example1@gmail.com",
  "oldPassword": "123456QWERTY",
  "newPassword": ""
}
```
## Task API Endpoint Examples

### Get Task By Id
```bash
[GET] /tasks/{{taskId}}
```
**Authorization Required**: `Bearer {{accessToken}}`

### Get User Tasks
```bash
[GET] /tasks?pageNumber=1&pageSize=1&status=0&dueDate=2055-08-07T14:30:45.4656254Z&priority=0
```
**Authorization Required**: `Bearer {{accessToken}}`

### Create Task
```bash
[POST] /tasks
```
**Request Body**:
```bash
{
  "title": "example",
  "description": "example",
  "status": 0,
  "priority": 0,
   "dueDate":"2055-08-07T14:30:45.4656254Z"
}
```
**Authorization Required**: `Bearer {{accessToken}}`

### Update Task
```bash
[PUT] /tasks/{{taskId}}
```
**Request Body**:
```bash
{
  "title": "newExample",
  "description": "newExample",
  "status": 0,
  "priority": 0,
  "dueDate":"2055-08-07T14:30:45.4656254Z"
}
```
**Authorization Required**: `Bearer {{accessToken}}`

### Delete Task By Id
```bash
[GET] /tasks/{{taskId}}
```
**Authorization Required**: `Bearer {{accessToken}}`


