# Secure Windows Application

## Overview

This is a Windows-native application that demonstrates secure communication via HTTPS using TLS. The system consists of a background Windows service and a Flutter-based Windows UI that allows the user to initiate secure HTTPS requests, view status, and monitor real-time logs. The communication between the UI and service is implemented using a lightweight TCP-based protocol over the local machine.

---

## Build Instructions

### Requirements

- **.NET Framework 4.8** (for the Windows service)
- **Flutter (Windows desktop enabled)**
- **Advanced Installer** (for packaging the installer)
- **Visual Studio 2022** (with C++ Desktop Development Tools and Windows SDK)

### Steps

1. Build the Windows Service:
   - Open the solution in Visual Studio.
   - Target `.NET Framework 4.8`.
   - Compile in `Release` mode.

2. Build the Flutter UI:
   ```bash
   flutter build windows
   ```

3. Package using Advanced Installer:
   - Use a *Professional Project*.
   - Add the compiled service and UI `.exe` as application files.
   - Use Custom Actions to install the certificate and register/start the service.
   - Ensure UAC elevation is enabled.

---

## Installer Usage

- Run the `.msi` installer with administrator privileges.
- It will install:
  - The Windows Service (set to run as `LocalSystem`).
  - The desktop UI and place it in the Start Menu.
  - A certificate into the system's certificate store (via `certutil`).
- After installation, launch the UI from the Start Menu.

---

## Architecture and Design Notes

### Components

- **Service:** A lightweight Windows Service written in C#, listening for commands from the UI and executing HTTPS requests using `HttpClient`.
- **UI:** A Flutter-based Windows desktop app allowing user interaction (server address/port input, connect/disconnect, live logs).
- **Communication:** Implemented over a simple TCP connection using `System.Net.Sockets` in the service and Dart's `Socket` in the UI.

### Key Design Choices

1. **.NET Framework 4.8:**  
   Chosen over .NET Core to maximize compatibility with typical customer environments, where .NET Framework is usually pre-installed. For a commercial product, requiring customers to install .NET Core can be a barrier.

2. **No Dependency Injection:**  
   Given the project's small scope and tight deadline, DI was considered unnecessary overhead. Code readability and simplicity took priority.

3. **Logging Approach:**  
   For a larger system, a logging library (like Serilog or NLog) would be used. Here, a basic static `Logger` class was used to write to both the UI and a file (`service.log`) for simplicity and transparency.

4. **UI Architecture:**  
   Riverpod was used for state management and dependency injection in Flutter. Providers manage logs, connection status, and socket communication in a modular and testable manner.

5. **Socket-Based Communication:**  
   While more complex IPC methods like named pipes or WCF were considered, TCP sockets were used due to their cross-platform nature and ease of integration with Flutter.

---

## Security Considerations

- **TLS 1.2+ enforced** by using HTTPS for all external communication.
- **Certificate chain validation** is built into `HttpClient` by default, but a custom validator can be added if needed (e.g., to allow self-signed certs with logging or explanation).
- Internal communication between UI and service is assumed to be local and does not use TLS, which is a defensible trade-off for this use case. If TLS were required internally, it could be added with a self-signed cert and SslStream.

---

## Tools Used

- **Visual Studio 2022** – C# service development
- **Flutter** – Cross-platform UI
- **Riverpod** – State management and DI in Flutter
- **Advanced Installer** – MSI packaging and custom actions
- **Dart** – Socket communication and Flutter logic
- **C#/.NET** – Background service logic and HTTPS requests

---

## Interpreting the Requirements

The requirement to _"publish a request for a Google search on HTTPS"_ was interpreted as a need to demonstrate the system's ability to make secure outbound requests using TLS, rather than literally perform web searches. We used `https://httpbin.org/post` as a placeholder endpoint.

The UI's "connect/disconnect" functionality was interpreted as starting or stopping the periodic HTTPS request flow from the service.

The system validates certificates as part of the request chain. If self-signed certificates were used, additional logic could be added to explicitly log the validation process.

---

## Logging Output

Logs from the service are written to:
```
%ProgramFiles%\YourAppName\service.log
```
These logs mirror real-time log data sent to the UI, allowing both debugging and monitoring.

---

## Final Thoughts

This project demonstrates secure communication, UI-service architecture, and certificate handling in a structured, practical way. The design reflects a balance of simplicity, clarity, and alignment with real-world practices.