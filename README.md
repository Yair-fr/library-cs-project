# Library Management System

A C# console application designed to manage a library inventory system. The system handles multiple book formats, tracking availability, and processing user requests through an interactive command-line menu.

## Authors
* Yair Friedensohn
* Omri Vigodman
* Arad Tal

## Features

### Book Management
* Physical Books: Tracks page counts and available physical copies.
* Digital Books: Stores file sizes and electronic formats.
* Audio Books: Keeps records of audio duration and narrator details.

### Library Operations
* Add Books: Consolidated insertion mechanism for all three media types.
* Borrow & Return: Simple transactional updates using unique numeric Book IDs.
* Inventory Reporting: Separate reporting filters for physical, digital, and audio categories alongside general inventory listings.
* Input Validation: Clean error handling to prevent application crashes from invalid data formats or negative numeric inputs.

## Technical Architecture

The program emphasizes DRY (Don't Repeat Yourself) principles by using unified helper methods to manage repetitive data entry tasks.

* Language: C#
* Paradigm: Object-Oriented Programming (OOP)
* User Interface: Console-based Command Line Interface (CLI)

### Core System Menu Options
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

## Prerequisites

* .NET SDK (.NET Core 3.1, .NET 5.0, .NET 6.0, or newer installed)
* A C# compatible IDE or text editor (Visual Studio, 
