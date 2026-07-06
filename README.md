# TaskManager – MiniCRM Task Management System

## 📌 Overview

TaskManager is a lightweight MiniCRM-style task management system built using ASP.NET Core Razor Pages and Entity Framework Core with SQLite.

The application follows layered architecture principles and implements core CRM concepts such as:

- Objects (Projects, Tasks, Users)
- Record Pages
- List Views with filtering
- Related Lists (Comments, Tags)
- Quick Actions (Assign, Change Status, Add Comment, Add Tag)
- Business workflow rules
- Role-based access control

---

# 🧱 Technology Stack

- C# / .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core
- SQLite (single portable DB file)
- Cookie Authentication
- Bootstrap 5
- VS Code + dotnet CLI
- EF Core Migrations

---

# 🏗 Architecture

The system follows a layered architecture:

## Layers

### 1️⃣ Domain Layer
Contains:
- Entities (AppUser, Project, TaskItem, Comment, Tag, TaskTag)
- Enums (TaskStatus)

### 2️⃣ Data Layer
- AppDbContext
- Fluent API configuration
- Migrations
- DbSeeder

### 3️⃣ Services Layer
- CurrentUserService (authentication abstraction)
- PasswordHasher
- TaskWorkflowService (business rules)

### 4️⃣ UI Layer
- Razor Pages
- ViewModels
- Layout
- Bootstrap UI

---

# 🔐 Authentication & Authorization

Authentication is implemented using cookie authentication.

Users are seeded automatically.

## Seeded Accounts

| Role      | Email            | Password        |
|-----------|------------------|-----------------|
| Admin     | admin@local      | Admin123!       |
| Zach      | zach@local       | Zach123!        |
| Dimitris  | dimitris@local   | Dimitris123!    |

---

## Access Rules

### 👤 Normal Users
- Can see only their own Projects
- Can see only Tasks under their Projects
- Cannot access Users page

### 🛡 Admin
- Can see all Projects
- Can see all Tasks
- Can access Users list page
- Can manage everything

---

# 📊 Data Model

## Relationships

- User 1 — * Project (Owner)
- Project 1 — * TaskItem
- TaskItem 1 — * Comment
- TaskItem * — * Tag (via TaskTag)
- TaskItem has AssignedToUserId (nullable)

---

# 🔄 Business Rules – Task Workflow

Allowed status transitions:

- ToDo → InProgress
- ToDo → Blocked
- InProgress → Done
- InProgress → Blocked
- Blocked → InProgress
- Done → (no transitions allowed)

This logic is implemented in:
`TaskWorkflowService`

---

# 📦 Setup Instructions

## 1️⃣ Requirements

- .NET 8 SDK
- VS Code (recommended)

---

## 2️⃣ Restore Dependencies

```bash
dotnet restore
dotnet tool restore

3️⃣ Create Database
dotnet tool run dotnet-ef database update

4️⃣ Run Application
dotnet run

Application will start at:

https://localhost:xxxx
🗃 Database

The system uses SQLite.

Database file:

taskmanager.db

Portable – can be submitted with project.

🧪 Testing Scenarios

Login as Admin → verify full access.
Login as Zach → verify “My records” only.
Login as Dimitris → verify isolation.
Try illegal status transition → blocked.
Add/remove tags → persist correctly.
Add comments → timestamp + user saved.

🚀 Features Summary

✔ Projects CRUD
✔ Tasks CRUD
✔ Quick Status Changes
✔ Task Assignment
✔ Comments (Related List)
✔ Tags (Many-to-Many)
✔ Workflow Enforcement
✔ Role-based Access
✔ Admin User List

📈 Future Improvements

Soft delete
Audit logs
Project sharing
File attachments
REST API version
Docker deployment

📄 License

Academic project for Software Engineering course.


---

# ✅ D) RUP REPORT – Full Academic Version

---

# 📘 RUP DOCUMENT STRUCTURE

---

# 1️⃣ Inception Phase

## 1.1 Vision

The goal of this project is to design and implement a lightweight CRM-style Task Management system that demonstrates separation of concerns, layered architecture, workflow enforcement, and role-based access control.

---

## 1.2 Stakeholders

- Development Team
- System Administrator
- End Users (Team Members)
- Academic Supervisor

---

## 1.3 Business Case

Organizations require structured tracking of projects and tasks with controlled workflows and collaboration mechanisms. The system provides:

- Clear task ownership
- Controlled state transitions
- Centralized project management
- Role-based governance

---

## 1.4 High-Level Use Cases

- Login
- Logout
- Create Project
- Edit Project
- Delete Project
- Create Task
- Assign Task
- Change Task Status
- Add Comment
- Add Tag
- View Users (Admin)

---

# 2️⃣ Elaboration Phase

## 2.1 Architecture Overview

Layered architecture:

Presentation → Services → Data → Domain

Separation of concerns:
- Business rules isolated in TaskWorkflowService
- Auth isolated in CurrentUserService
- Data access isolated in DbContext

---

## 2.2 Domain Model (Class Diagram)

Classes:

AppUser  
Project  
TaskItem  
Comment  
Tag  
TaskTag  

Relationships as described in README.

---

## 2.3 ER Diagram

Entities:
Users  
Projects  
Tasks  
Comments  
Tags  
TaskTags  

Primary/Foreign keys documented.

---

## 2.4 Sequence Diagram – Change Status

User → UI → Task Page  
→ TaskWorkflowService.ValidateTransition()  
→ DbContext.SaveChanges()  
→ Return result  

---

## 2.5 Activity Diagram – Status Workflow

Start  
Check current status  
Check allowed transition  
If valid → update  
Else → reject  

---

# 3️⃣ Construction Phase

## 3.1 Implementation Strategy

Step-by-step modular implementation:

1. Authentication
2. Projects module
3. Tasks module
4. Workflow enforcement
5. Comments module
6. Tags module
7. Admin module

---

## 3.2 Testing Strategy

Manual testing per module:
- CRUD validation
- Security validation
- Workflow validation
- Edge cases

---

# 4️⃣ Transition Phase

## 4.1 Deployment

- SQLite portable DB
- dotnet run
- No external infrastructure required

---

## 4.2 User Manual

Login → Create Project → Create Task → Assign → Change Status → Add Comments → Add Tags.

Admin → Access Users list.

---

## 4.3 Future Work

- Multi-project collaboration
- REST API
- Microservices version
- Azure deployment

---

# 🎯 Academic Strength Points

✔ Clear separation of concerns  
✔ Business logic in service layer  
✔ Enforced workflow  
✔ Many-to-many implementation  
✔ Role-based access  
✔ Layered architecture  
✔ EF Migrations  
✔ Portable DB  

---
