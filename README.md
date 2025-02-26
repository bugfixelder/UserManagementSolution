# UserManagementSolution
The simplest demo User management that using WPF and WCF

# User Management System
## Overview
The User Management System is a distributed application built using Windows Communication Foundation (WCF) and Windows Presentation Foundation (WPF). It consists of a WCF service (UserService) that provides CRUD (Create, Read, Update, Delete) operations and duplex communication for user management, and a WPF client (UserClient) that interacts with the service to display and manage user data in a user-friendly interface. The system supports real-time updates via duplex callbacks and periodic refreshes, ensuring a responsive and interactive user experience.

This project demonstrates advanced concepts in .NET, including WCF duplex communication, MVVM pattern in WPF, dependency injection, unit testing, and thread safety for multi-threaded access.

## Features
### WCF Service (UserService)
CRUD Operations: Provides asynchronous methods (GetAllUsersAsync, GetUserAsync, AddUserAsync, UpdateUserAsync, DeleteUserAsync) to manage a list of users.
Duplex Communication: Implements duplex callbacks using wsDualHttpBinding to send real-time status updates (OnUserStatusChanged) to connected clients every 10 seconds via a timer.
Singleton Service: Configured with InstanceContextMode.Single and ConcurrencyMode.Multiple to handle multiple concurrent client requests safely, using thread-safe collections (ConcurrentDictionary) and locks for shared state management.
Callback Management: Uses a ConcurrentDictionary to manage multiple client callbacks, ensuring each client receives independent status updates.
### WPF Client (UserClient)
MVVM Pattern: Implements UserViewModel to manage user data, commands, and UI updates, following the Model-View-ViewModel pattern for clean separation of concerns.
Real-Time Updates: Displays user status changes via duplex callbacks from UserService, updating a log text box in real-time using a Dispatcher for UI thread safety.
Periodic Refresh: Uses a timer to refresh the user list every 30 seconds, ensuring the UI stays synchronized with the server state.
CRUD Operations: Supports adding, updating, and deleting users through commands (AddCommand, UpdateCommand, DeleteCommand), interacting with the WCF service asynchronously.
UI Binding: Uses ObservableCollection<User> and INotifyPropertyChanged for two-way data binding with the WPF UI, displaying users in a list and logging status updates in a text box.
### Dependency Injection
Utilizes SimpleInjector as a dependency injection container to manage dependencies such as IDispatcherWrapper, IUserServiceProxy, and ITimerWrapper, ensuring clean and testable code.
Eliminates the need for reflection to inject callback handlers, using a custom UserServiceProxy that implements both IUserServiceProxy and IUserServiceCallback, firing events for status updates.
### Testing
Unit Tests: Includes comprehensive unit tests for both UserService and UserViewModel using xUnit and Moq, covering CRUD operations, callback management, timer logic, and UI updates.
Mocking: Uses interfaces and wrappers (IDispatcherWrapper, ITimerWrapper, IUserServiceProxy) to mock dependencies, ensuring tests are independent of WCF and WPF runtime.
Thread Safety: Tests verify thread-safe operations in singleton WCF service and UI updates on the WPF thread, addressing multi-threaded access and race conditions.
## Resolved Issues
### WCF Service Challenges
Thread Safety in Singleton Mode: Addressed race conditions in UserService by using lock for List<User> and ConcurrentDictionary for callback management, ensuring safe concurrent access with ConcurrencyMode.Multiple.
Duplex Callback Management: Solved issues with sharing callbacks in singleton mode by implementing a ConcurrentDictionary to manage multiple client callbacks, avoiding overwrites and ensuring each client receives updates independently.
WCF Test Client Compatibility: Resolved red method warnings in WCF Test Client by ensuring proper implementation of async methods and configuring wsDualHttpBinding and basicHttpBinding in App.config.
Port Access Issues: Fixed AddressAccessDeniedException on port 8733 by running Visual Studio or the service host with Administrator privileges or using netsh to register the port.
### WPF Client Challenges
Async Initialization: Handled asynchronous initialization in UserViewModel by calling InitializeAsync in MainWindow_Loaded, avoiding issues with async constructors and ensuring proper UI updates.
UI Thread Safety: Ensured UI updates (e.g., LogText, Users) are performed on the UI thread using Dispatcher or IDispatcherWrapper, preventing InvalidOperationException in WPF.
Timer and Callback Handling: Managed periodic refreshes with Timer and WCF callbacks by creating ITimerWrapper and IUserServiceProxy wrappers, resolving NotSupportedException in Moq tests and ensuring thread-safe updates.
Null Reference in Logs: Fixed Assert.Contains failures in tests by initializing LogText with an empty string in UserViewModel constructor, preventing null values and ensuring correct logging.
### Testing Challenges
Mocking Sealed Classes: Resolved NotSupportedException for mocking Dispatcher and Timer by creating IDispatcherWrapper and ITimerWrapper interfaces, enabling Moq-based unit tests without runtime dependencies.
Async Event Testing: Fixed NullReferenceException and verification issues in tests by using SetupAdd with Callback instead of RaiseAsync for sync events like UserStatusChanged, ensuring proper event handling.
Internal Access: Configured InternalsVisibleTo to allow UserService.Tests and UserClient.Tests to access internal members, ensuring comprehensive test coverage without exposing public fields.
## Getting Started
### Prerequisites
.NET Framework 4.8 or higher (or .NET Core/.NET 5+ if migrating)
Visual Studio 2019/2022 with WCF and WPF support
WCF Service Host: Ensure UserService runs on port 8733 (may require Administrator privileges or netsh configuration)
### Installation
Clone the repository:
git clone <repository-url>
cd UserManagementSystem
Open the solution in Visual Studio:
UserManagementSolution.sln
Build and run:
Start UserService (WCF project) first, either via Visual Studio or command line with Administrator privileges:
cd UserService\bin\Debug
UserService.exe
Then run UserClient (WPF project) to launch the UI.
### Configuration
Ensure App.config in UserService has the correct base address (http://localhost:8733/UserService/) and bindings (wsDualHttpBinding for duplex, basicHttpBinding for testing).
Update MainWindow.xaml.cs or App.xaml.cs if DI container configurations need adjustment.
### Usage
Launch UserService to host the WCF service.
Launch UserClient to interact with the WPF UI:
View the list of users in the UI.
Add, update, or delete users using the buttons, triggering CRUD operations via WCF.
Observe real-time status updates in the log text box, updated every 10 seconds from WCF callbacks and every 30 seconds from the client timer.
### UsageContributing
Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request with your changes. Ensure tests pass and add new tests for new features.
### UsageLicense
This project is licensed under the MIT License
