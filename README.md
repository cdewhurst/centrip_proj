# Secure Windows Application
## Overview
This is a Windows-native application that demonstrates secure communication via HTTPS using TLS. The system consists of a background Windows service and a Flutter-based Windows UI that allows the user to initiate secure HTTPS requests, view status, and monitor real-time logs. The communication between the UI and service is implemented using a lightweight TCP-based protocol over the local machine.
## GitHub locationThe project is available on GitHub at `git@github.com:cdewhurst/centrip_proj.git`
## Using the systemOnce installed, the application's UI is available on the Start Menu under the title "My Secure UI App". The service is installed and running as "Secure Beacon"
## Interpreting the Requirements
The requirement to _"publish a request for a Google search on HTTPS"_ was interpreted as a need to demonstrate the system's ability to make secure outbound requests using TLS, rather than literally perform web searches. I used `https://httpbin.org/post` as a placeholder endpoint and for testing.
The system does not sign outgoing requests, as might be required for mutual TLS (mTLS) connections. Doing this and testing would probably have required that a server was also written, which seemed to be outside the scope of the project. However, had this been needed, it could have been done using .NET's `X509Certificate` attached to the PostRequest by specifying it's usage in an `HttpClientHandler`.
The UI's "connect/disconnect" functionality was interpreted as starting or stopping the periodic HTTPS request flow from the service.
The system validates certificates as part of the request chain. Additional, but unnecessary logic has been added to explicitly log the certificate validation process.

## Build Instructions
### Requirements
- **.NET Framework 4.8** (for the Windows service)- **Flutter (Windows desktop enabled)**- **Advanced Installer** (for packaging the installer)- **Visual Studio 2022** (with C++ Desktop Development Tools and Windows SDK, to allow Flutter builds, even though this isn't built inside Visual Studio)
### Steps
1. Build the Windows Service:   - Open the solution in Visual Studio 2022.   - Target `.NET Framework 4.8`.   - Compile in `Release` mode.
2. Build the Flutter UI:   ```bash   flutter build windows --release   ```
3. Package using Advanced Installer:   - Open the SecureBeacon.aip project   - Build the project. It will pick up the output of the above two projects.
## Installer Usage
- Run the `.msi` installer.- It will install:  - The Windows Service (set to run as `LocalSystem`).  - The desktop UI and place it in the Start Menu.  - A self-signed certificate into the system's certificate store (via `certutil`).    - Ths isn't required by the system, but this was working before decideing on how to interpret the requirements. This was left there to demonstrate a capability, but forms no active part of the system.- After installation, launch the UI from the Start Menu.
## Architecture and Design Notes
### Components
- **Service:** A lightweight Windows Service written in C#, listening for commands from the UI and executing HTTPS requests using `HttpClient`.- **UI:** A Flutter-based Windows desktop app allowing user interaction (server address/port input, status, connect/disconnect, live logs).- **Communication:** Implemented over a simple TCP connection using `System.Net.Sockets` in the service and Dart's `Socket` in the UI.
### Key Design Choices
1. **.NET Framework 4.8:**     Chosen over .NET Core to maximize compatibility with typical customer environments, where .NET Framework is usually pre-installed. For a commercial product, requiring customers to install .NET Core can be a barrier. The project could, however, have been written in .NET Core, demonstrating some of the syntactic sugar available in newer versions of C#.
2. **No Dependency Injection:**     Given the project's small scope and tight deadline, DI was considered unnecessary overhead within the service. Code readability and simplicity took priority.
3. **Logging Approach:**     For a larger system, a logging library (like Serilog or NLog) would be used. Here, a basic static `Log` class was used to write to both the UI via a TCP connection and a file (`service.log`) for simplicity and transparency.
4. **UI Architecture:**     Riverpod was used for state management and dependency injection in Flutter. Providers manage logs, connection status, and socket communication in a modular and testable manner.
5. **Socket-Based Communication:**     While more complex IPC methods like named pipes or WCF were considered, TCP sockets were used due to their cross-platform nature and ease of integration with Flutter.
6. **Use of LocalSystem:**     The service initially targetted `NetworkService` for security best practices (minimal privileges). Unfortunately, installation and communication issues arose and the service would not install correctly without elevated privileges. As a result, the service runs under `LocalSystem`. While this has broader privileges, the implementation ensures te service does not perform any sensitive operations outside its intended responsibilities.
6. **No Persistent Settings:**     The service and UI currently do not persst settings between runs. This simplifies the implementation, but adding support for user-editable config files or registry settings could be explored were the service required to start sending out requests when it starts.
## Security Considerations
- **TLS 1.2+ enforced** by using HTTPS for all external communication. No excess code is required above using `HttpClient.PostAsync` which automatically uses TLS 1.2+ for any call to `HTTPS`.- **Certificate chain validation** is built into `HttpClient` by default, but a custom validator was added as this was my interpretation of the requirement to validate the cert chain (e.g., to allow self-signed certs with logging or explanation). This is typically not needed to validate a non-self-signed cert though.- Internal communication between UI and service is assumed to be local and does not use TLS, which is a defensible trade-off for this use case. If TLS were required internally, it could be added with a self-signed cert and `SslStream`.
---
## Tools Used
- **Visual Studio 2022** – C# service development- **Flutter** – Cross-platform UI- **Riverpod** – State management and DI in Flutter- **Advanced Installer** – MSI packaging and custom actions- **Dart** – Socket communication and Flutter logic- **C#/.NET** – Background service logic and HTTPS requests
## Logging Output
Logs from the service are written to:
```%ProgramFiles%\SecureBeaconService\service.log```
These logs mirror real-time log data sent to the UI, allowing both debugging and monitoring.
## Final Thoughts
This project demonstrates secure communication, UI-service architecture, and certificate handling in a structured, practical way. The design reflects a balance of simplicity, clarity, and alignment with real-world practices.