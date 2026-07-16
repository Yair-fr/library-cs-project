# Library Management System

A C# console application designed to manage a library inventory system. The system handles multiple book formats, tracking availability, and processing user requests through an interactive command-line menu.

## Authors
* Yair Friedensohn
* Omri Vigodman
* Arad Tal

# Library Management System

---

**A polymorphic C# console application for managing multiple book types (Physical, Digital, AudioBook) with MySQL persistence.**
---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites & Setup](#prerequisites--setup)
3. [Database Configuration](#database-configuration)
4. [Core Classes & Methods](#core-classes--methods)
5. [Usage Guide](#usage-guide)
6. [Menu Operations](#menu-operations)
7. [Design Patterns Used](#design-patterns-used)
8. [Error Handling](#error-handling)
9. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

### Class Hierarchy

```
Book (Abstract Base Class)
├── PhysicalBook (Pages, AvailableCopies)
├── DigitalBook (FileSizeMB, Format)
└── AudioBook (DurationMinutes, Narrator)
```

**Design Pattern:** Repository Pattern + Strategy Pattern (polymorphic Borrow/Return behavior)

### Key Components

| Component | Purpose |
|-----------|---------|
| `Book.cs` | Abstract base class defining common book properties (Id, Title, Author) |
| `PhysicalBook.cs` | Implements borrowable inventory logic with copy tracking |
| `DigitalBook.cs` | Non-borrowable digital format with file metadata |
| `AudioBook.cs` | Non-borrowable audio format with narrator & duration |
| `IBookRepository.cs` | Interface defining CRUD contract |
| `MySqlBookRepository.cs` | MySQL implementation with polymorphic serialization |
| `Program.cs` | Console UI with menu-driven operations |

---

## Prerequisites & Setup

### System Requirements

- **.NET Framework:** .NET 8.0+ or .NET Framework 4.8+
- **MySQL Server:** 5.7+ or 8.0+
- **NuGet Packages:** `MySql.Data` (version 8.0+)

### Installation Steps

1. **Clone/Extract Project**
   ```bash
   cd LibraryManagementSystem
   ```

2. **Install MySQL NuGet Package**
   ```bash
   dotnet add package MySql.Data
   ```

3. **Verify MySQL is Running**
   ```bash
   mysql -u root -p
   ```

4. **Update Connection String in `Program.cs` (Line 25)**
   ```csharp
   string connectionString = "server=YOUR_SERVER;database=library_db;user=YOUR_USER;password=YOUR_PASSWORD;";
   ```

5. **Compile & Run**
   ```bash
   dotnet build
   dotnet run
   ```

---

## Database Configuration

### Schema Setup

Execute this SQL to create the required database and table:

```sql
-- Create Database
CREATE DATABASE IF NOT EXISTS library_db;
USE library_db;

-- Create Books Table
CREATE TABLE books (
    id INT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    author VARCHAR(255) NOT NULL,
    type VARCHAR(20) NOT NULL, -- PHYSICAL, DIGITAL, AUDIO
    pages INT NULL,
    available_copies INT NULL,
    file_size_mb DOUBLE NULL,
    format VARCHAR(50) NULL,
    duration_minutes INT NULL,
    narrator VARCHAR(255) NULL
);
```

### Column Mapping by Book Type

| Type | Pages | Copies | FileSizeMB | Format | Duration | Narrator |
|------|-------|--------|-----------|--------|----------|----------|
| **PHYSICAL** | ✓ | ✓ | — | — | — | — |
| **DIGITAL** | — | — | ✓ | ✓ | — | — |
| **AUDIO** | — | — | — | — | ✓ | ✓ |

---

## Core Classes & Methods

### 1. **Book.cs** (Abstract Base Class)

#### Properties

```csharp
public int Id              // Unique book identifier (immutable after construction)
public string Title        // Book title (defaults to "No title" if null)
public string Author       // Author name (defaults to "UN-KNOWN" if null)
```

#### Virtual Methods

| Method | Signature | Purpose |
|--------|-----------|---------|
| `Borrow()` | `public virtual bool Borrow()` | Returns `true` by default; overridden by `PhysicalBook` |
| `Return()` | `public virtual bool Return()` | Returns `true` by default; overridden by `PhysicalBook` |
| `PrintInfo()` | `public virtual void PrintInfo()` | Outputs formatted: `Id; Title; Author` |

---

### 2. **PhysicalBook.cs**

#### Constructor
```csharp
PhysicalBook(int id, string title, string author, int pages, int availableCopies)
```

#### Properties

| Property | Type | Validation | Notes |
|----------|------|-----------|-------|
| `Pages` | `int` | ≥ 0 | Read-only after construction |
| `AvailableCopies` | `int` | ≥ 0 | Modified by Borrow/Return |

#### Key Methods

**`Borrow()`** — Decrements copy count if available
```csharp
public override bool Borrow()
{
    if (AvailableCopies > 0)
    {
        AvailableCopies--;
        return true;   // Success
    }
    return false;      // No copies available
}
```

**`Return()`** — Increments copy count unconditionally
```csharp
public override bool Return()
{
    AvailableCopies++;
    return true;       // Always succeeds (no overborrow check)
}
```

**`PrintInfo()`** — Outputs full physical book details
```
Physical Book; [Id]; [Title]; [Author]; [Pages]; [AvailableCopies]
```

---

### 3. **DigitalBook.cs**

#### Constructor
```csharp
DigitalBook(int id, string title, string author, double fileSizeMB, string format)
```

#### Properties

| Property | Type | Validation | Notes |
|----------|------|-----------|-------|
| `FileSizeMB` | `double` | ≥ 0.0 | Defaults to 0.0 if negative |
| `Format` | `string` | Not null/empty | Defaults to "no format" if empty |

#### Key Methods

**`PrintInfo()`** — Outputs format: `Digital Book; [Id]; [Title]; [Author]; [FileSizeMB]; [Format]`

**Note:** `Borrow()` and `Return()` inherit base behavior (always return `true`, no state change)

---

### 4. **AudioBook.cs**

#### Constructor
```csharp
AudioBook(int id, string title, string author, int durationMinutes, string narrator)
```

#### Properties

| Property | Type | Validation | Notes |
|----------|------|-----------|-------|
| `DurationMinutes` | `int` | ≥ 0 | Defaults to 0 if negative |
| `Narrator` | `string` | Not null/empty | Defaults to "UNKNOWN NARRATOR" if empty |

#### Key Methods

**`PrintInfo()`** — Outputs format: `Audio Book; [Id]; [Title]; [Author]; [DurationMinutes]; [Narrator]`

**Note:** `Borrow()` and `Return()` inherit base behavior (no state change)

---

### 5. **IBookRepository.cs** (Interface)

Contract that all repository implementations must follow:

```csharp
public interface IBookRepository
{
    bool Add(Book book);              // Insert new book → returns success status
    Book? GetById(int id);            // Retrieve single book by ID → nullable
    Book[] GetAll();                  // Fetch all books → fixed-size array
    bool Update(Book book);           // Update existing book → returns success status
    bool Delete(int id);              // Remove book by ID → returns success status
}
```

---

### 6. **MySqlBookRepository.cs** (MySQL Implementation)

#### Constructor
```csharp
MySqlBookRepository(string connectionString)
```
Accepts full MySQL connection string.

#### Method Breakdown

**`Add(Book book)` — Insert Polymorphic Book Types**

Logic:
1. Pattern-match on book type (switch statement with `case PhysicalBook p:`)
2. Extract type-specific fields (Pages/Copies, FileSizeMB/Format, Duration/Narrator)
3. Set nullable parameters to `DBNull.Value` for irrelevant columns
4. Execute parameterized INSERT query
5. Return `true` if exactly 1 row affected

**Returns:** `bool` — `true` if insertion succeeded; `false` on error or duplicate ID

```csharp
Example:
PhysicalBook pb = new PhysicalBook(1, "Harry Potter", "J.K. Rowling", 309, 5);
bool success = repository.Add(pb);
```

---

**`GetById(int id)` — Retrieve Single Book**

Logic:
1. Execute SELECT query filtering by ID
2. If result exists, call `CreateBookFromReader()` to deserialize
3. Return reconstructed book object or `null`
4. Catch `MySqlException` and return `null` silently

**Returns:** `Book?` — Book object or `null` if not found/error

```csharp
Example:
Book? book = repository.GetById(1);
if (book is PhysicalBook pb)
    Console.WriteLine($"Available: {pb.AvailableCopies}");
```

---
**`GetAll()` — Retrieve All Books Using the Database Row Count**

**Logic:**
1. Call `GetTotalBooksCount()` to determine the total number of rows in the `books` table.
2. If the count is `0`, allocate an array of size `1` to avoid creating a zero-length working array.
3. Execute the `SELECT` query to retrieve all books.
4. For each row, create a `Book` object using `CreateBookFromReader()`.
5. Store each book in the array and increment `currentIndex`.
6. After all rows have been processed, create a new array whose length equals `currentIndex`.
7. Copy the loaded books into the new array.
8. Return the trimmed array.
9. If a database error occurs, return an empty array (`new Book[2]`) with null values.

```csharp
public int GetTotalBooksCount()
{
    string countSql = "SELECT COUNT(*) FROM books";

    try
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(countSql, conn))
        {
            conn.Open();

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
            }

            return 0;
        }
    }
    catch (MySqlException)
    {
        // Return -1 if a database error occurs
        return -1;
    }
}
```

```csharp
public Book[] GetAll()
{
    int totalRows = GetTotalBooksCount();

     switch (totalRows)
     {
         case -1: return new Book[2]; // Flag for conneciton error in GetTotalBooksCount
         case 0: totalRows++; break;
         default: break;
     }

    Book[] books = new Book[totalRows];
    int currentIndex = 0;

    string selectSql = "SELECT id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator FROM books";

    try
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(selectSql, conn))
        {
            conn.Open();

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Book? currentBook = CreateBookFromReader(reader);

                    if (currentBook != null)
                    {
                        books[currentIndex] = currentBook;
                        currentIndex++;
                    }
                }
            }
        }
    }
    catch (MySqlException)
    {
        return new Book[1];
    }

     if (currentIndex > 0)
     {

         // Return an array containing only the loaded books
         Book[] result = new Book[currentIndex];

         for (int i = 0; i < currentIndex; i++)
         {
             result[i] = books[i];
         }

         return result;
     }
     else
     {
         return new Book[1];
     }
}
```

**Returns:** `Book[]` — An array containing exactly the books retrieved from the database. If a database error occurs, an empty array (`new Book[2]`) is returned.

```csharp
Example:

    static void PrintAllBooksUI(IBookRepository repository)
    {
        Book[] bk = repository.GetAll();
        if(bk.Length == 2 && bk[0] == null && bk[1] == null) // Edge case cover connection error in count rows
        {
            Console.WriteLine(DatabaseErrorMessage);
        }
        foreach (Book item in bk)
        {
            if (item == null)
            {
                break;
            }
            else
            {
                item.PrintInfo(); ;
            }
        }
    }
```

---

---

**`Update(Book book)` — Modify Existing Book**

Logic:
1. Check if book is `PhysicalBook` (only type that changes on Borrow/Return)
2. If yes: execute UPDATE to set `available_copies` where ID matches
3. If no (DigitalBook/AudioBook): return `true` (no-op, state never changes)
4. Return `true` if 1 row updated; `false` otherwise

**Returns:** `bool` — `true` if update succeeded or was a no-op

```csharp
Example:
PhysicalBook pb = repository.GetById(1) as PhysicalBook;
pb.Borrow();
repository.Update(pb);  // Persists copy count change
```

---

**`Delete(int id)` — Remove Book**

Logic:
1. Execute DELETE query filtering by ID
2. Return `true` if exactly 1 row deleted
3. Return `false` if 0 rows or error

**Returns:** `bool` — `true` if deletion succeeded

```csharp
Example:
bool deleted = repository.Delete(5);
```

---

**`CreateBookFromReader(MySqlDataReader reader)` — Deserialize Database Row**

Logic:
1. Read required columns (ID, Title, Author, Type)
2. Switch on `Type` string (PHYSICAL/DIGITAL/AUDIO)
3. **For each type:**
   - Read type-specific columns with null-safety (`reader.IsDBNull()` check)
   - Instantiate appropriate subclass
   - Return constructed object
4. Return `null` for unknown types

**Returns:** `Book?` — Correctly typed book object or `null`

**Example Row Mapping:**
```
DB Row: (1, "Harry Potter", "J.K. Rowling", "PHYSICAL", 309, 5, NULL, NULL, NULL, NULL)
→ new PhysicalBook(1, "Harry Potter", "J.K. Rowling", 309, 5)
```

---

### 7. **Program.cs** (Console UI)

#### Menu Structure

```
1. Add physical book
2. Add digital book
3. Add audio book
4. List all books
5. Borrow book
6. Return book
7. Digital book report
8. Audio book report
9. Physical book report
10. Exit
```

#### Helper Methods

**`GetInput(string prompt)`** — Thread-safe console input
```csharp
string userChoice = GetInput("Enter book ID: ");
// Returns empty string "" if null (graceful fallback)
```

**`TryReadNonNegativeInt(string input, out int value)`** — Validate integer ≥ 0
```csharp
if (!TryReadNonNegativeInt(userInput, out int id))
    Console.WriteLine("Invalid input");
```

**`TryReadNonNegativeDouble(string input, out double value)`** — Validate double ≥ 0.0
```csharp
if (!TryReadNonNegativeDouble(sizeInput, out double size))
    Console.WriteLine("Invalid input");
```

---

#### Core Menu Operations

**`ExecuteAddBook(IBookRepository, string choice)`** — Add Book by Type

Unified flow:
1. Read common fields (ID, Title, Author)
2. Switch on book type, read type-specific fields
3. Validate all inputs via `TryRead*` methods
4. Construct appropriate subclass
5. Call `repository.Add()` and report result

```csharp
// Flow for Physical Book:
// Input: ID=1, Title="Book", Author="Author", Pages=300, Copies=5
// → new PhysicalBook(1, "Book", "Author", 300, 5)
// → repository.Add(pb)
```

---

**`BorrowBookUI(IBookRepository)`** — Borrow Book by ID

Logic:
1. Prompt user for book ID
2. Fetch book via `repository.GetById(id)`
3. Check type:
   - **PhysicalBook:** Call `Borrow()`, if `true`, update via `repository.Update()`
   - **DigitalBook/AudioBook:** Always return success (no state change)
4. Report result to user

**Behavior:**
- Physical: Decrements copy count if available, persists to DB
- Digital/Audio: Always succeeds, no DB update needed

```csharp
Book? book = repository.GetById(3);
if (book is PhysicalBook pb)
{
    if (pb.Borrow())  // AvailableCopies--
        repository.Update(pb);  // Persist to DB
}
```

---

**`ReturnBookUI(IBookRepository)`** — Return Book by ID

Logic:
1. Prompt user for book ID
2. Fetch book via `repository.GetById(id)`
3. Check type:
   - **PhysicalBook:** Call `Return()`, always updates via `repository.Update()`
   - **DigitalBook/AudioBook:** Always return success (no state change)
4. Report result

**Behavior:**
- Physical: Increments copy count unconditionally, persists to DB
- Digital/Audio: Always succeeds, no DB update needed

---

**`AudioBookReport()`** — Aggregate Audio Book Statistics

Iterates through all books, filters `AudioBook` instances:
- **Total Duration:** Sum of all `DurationMinutes`
- **Max Duration:** Highest single `DurationMinutes`

Output format: `[TotalMinutes]; [MaxMinutes]`

```
Example: 720; 480  (Total 720 min, Max single book 480 min)
```

---

**`DigitalBookReport()`** — Aggregate Digital Book Statistics

Iterates through all books, filters `DigitalBook` instances:
- **Total Size:** Sum of all `FileSizeMB`
- **Max Size:** Largest single `FileSizeMB`

Output format: `[TotalMB]; [MaxMB]`

```
Example: 150.5; 50.2  (Total 150.5 MB, Max single book 50.2 MB)
```

---

**`PhysicalBookReport()`** — List Out-of-Stock Physical Books

Iterates through all books, filters `PhysicalBook` where `AvailableCopies == 0`:
- Outputs ID and Title of each unavailable book
- Joins multiple results with `"; "`
- Outputs nothing if all books in stock

Output format: `[Id]; [Title]; [Id2]; [Title2]; ...`

```
Example: 1; Harry Potter; 3; Twilight
(Books with IDs 1 and 3 have no copies available)
```

---

## Usage Guide

### Scenario 1: Add a Physical Book

```
Menu: Select Option 1

Enter ID: 1
Enter Title: Harry Potter and the Philosopher's Stone
Enter Author: J.K. Rowling
Enter Pages: 309
Enter Available Copies: 5

Output: Book added successfully
```

**Validation:**
- ID, Pages, Copies must be non-negative integers
- Title/Author cannot be null (fallback to defaults if empty)

---

### Scenario 2: Borrow a Physical Book

```
Menu: Select Option 5

Enter Book ID to Borrow: 1

Output: Book borrowed successfully!
```

**Behind the scenes:**
1. Fetches book ID 1 from database
2. Checks if it's PhysicalBook (yes)
3. Calls `Borrow()` → checks `AvailableCopies > 0` → decrements to 4
4. Calls `repository.Update()` → SQL UPDATE available_copies to 4
5. Reports success

**If copies = 0:**
```
Output: (No output from Borrow logic, fails silently)
```

---

### Scenario 3: Return a Book

```
Menu: Select Option 6

Enter Book ID to Return: 1

Output: Book returned successfully!
```

**Behind the scenes:**
1. Fetches book ID 1
2. Checks if PhysicalBook (yes)
3. Calls `Return()` → increments copies to 5
4. Calls `repository.Update()` → SQL UPDATE persists change
5. Reports success

---

### Scenario 4: View All Books

```
Menu: Select Option 4

Output:
Physical Book; 1; Harry Potter; J.K. Rowling; 309; 5
Digital Book; 2; E-Book; Author; 2.5; PDF
Audio Book; 3; Audiobook; Narrator; 480; James Earl Jones
```

---

### Scenario 5: Generate Digital Book Report

```
Menu: Select Option 7

Output: 2.5; 2.5
```

(Only 1 digital book in DB, so total = max)

---

## Design Patterns Used

| Pattern | Implementation | Benefit |
|---------|----------------|---------|
| **Repository Pattern** | `IBookRepository` interface + `MySqlBookRepository` | Decouples data access from business logic; enables mock testing |
| **Strategy Pattern** | Polymorphic `Borrow()`/`Return()` | Different behavior per book type without conditional logic in caller |
| **Factory Pattern (Implicit)** | `CreateBookFromReader()` | Reconstructs correct subclass from database rows |
| **Template Method** | Base `PrintInfo()` + overrides | Standardizes output format across all book types |

---

## Error Handling

### Database Errors

All `MySqlRepository` methods catch `MySqlException` and return safe defaults:

| Method | Return Value on Error |
|--------|----------------------|
| `Add()` | `false` |
| `GetById()` | `null` |
| `GetAll()` | Empty array `[]` |
| `Update()` | `false` |
| `Delete()` | `false` |

**Console Output:**
```csharp
if (!repository.Add(book))
    Console.WriteLine("Database error occurred. Operation failed.");
```

---

### Input Validation

**Constants defined in `Program.cs`:**
```csharp
const string InvalidInputMessage = "Invalid input. Please try again.";
const string DatabaseErrorMessage = "Database error occurred. Operation failed.";
```

**Validation flow:**
```
User Input → TryRead*() method → bool success
↓ false → Print InvalidInputMessage, return
↓ true → Proceed with operation
```

---

### Null Safety

**Nullable reference types used throughout:**
```csharp
Book? book = repository.GetById(id);  // May be null
if (book == null) { /* handle */ }
```

**Property defaults:**
- Title: `"No title"` if null/empty
- Author: `"UN-KNOWN"` if null/empty
- Narrator: `"UNKNOWN NARRATOR"` if null/empty
- Format: `"no format"` if null/empty

---

## Troubleshooting

### Connection Failed

**Error:** `Unable to connect to any of the specified MySQL hosts`

**Solution:**
1. Verify MySQL is running: `mysql -u root -p`
2. Check connection string in `Program.cs` line 25
3. Confirm database `library_db` exists: `SHOW DATABASES;`
4. Test credentials separately

```bash
mysql -u root -p admin -h localhost
```

---

### Table Not Found

**Error:** `Table 'library_db.books' doesn't exist`

**Solution:**
Execute the schema setup SQL from [Database Configuration](#database-configuration) section.

---

### Duplicate Key Error

**Error:** `Duplicate entry '1' for key 'PRIMARY'`

**Solution:** Choose a different ID when adding a new book. IDs must be unique.

---

### Reports Showing Incorrect Values

**Symptom:** Audio report shows wrong total duration

**Possible Cause:** Database contains `NULL` values for type-specific columns

**Debug:**
```sql
SELECT * FROM books WHERE type='AUDIO';
```

Ensure `duration_minutes` is populated for all AUDIO books.

---


### Project Authors: Yair Friedensohn & Omri Vigodman & Arad Tal
