# Proxmox FQDN Exporter

A .NET 9.0 application for exporting Fully Qualified Domain Names (FQDNs) and IP addresses of Proxmox containers and virtual machines. The exporter can update a custom FQDN list for Pi-hole and supports flexible export options.

## Features
- Fetches FQDN and IP addresses from Proxmox containers and VMs
- Exports host lists to console and/or Pi-hole
- Supports custom subnet filtering
- Designed for automation and integration with Pi-hole
- Configurable via `config.json`
- **AOT (Ahead-Of-Time) compilation to native executable for direct deployment on Proxmox host**

## Requirements
- .NET 9.0 SDK
- Proxmox host with access to `pct` and `qm` commands
- (Optional) Pi-hole instance for DNS list updates

## Native Executable (AOT)
This project uses .NET's AOT (Ahead-Of-Time) compilation to produce a native executable. This allows the exporter to run directly on the Proxmox host without requiring a .NET runtime. The deployment scripts and Dockerfile are set up to build and publish a self-contained, native binary for your target platform.

## Configuration
Edit the `config.json` file in the root of the project:

```json
{
  "SubnetFilter": "192.168.0.0/24",
  "Export": {
    "Console": true,
    "Pihole": {
      "Id": 104,
      "ListFilePath": "/etc/pihole/hosts/custom.list",
      "ExecutablePath": "/usr/local/bin/pihole",
      "Type": "Container"
    }
  }
}
```

- `SubnetFilter`: CIDR notation for filtering network interfaces
- `Export.Console`: Output host list to console
- `Export.Pihole`: Pi-hole export settings

## Usage

### Build and Run

```
dotnet build

dotnet run --project Proxmox.Fqdn.Exporter
```

### Native Publish (AOT)
To build a native executable for your target platform (e.g., Linux x64):

```
dotnet publish -c Release -r linux-x64 -p:PublishAot=true --self-contained true -o out
```

The resulting binary in the `out` directory can be copied and run directly on your Proxmox host.

### Docker and Cross-Compilation
A Dockerfile and compose file are provided in the `Deployment/build` directory. If your host supports cross-compilation, you can use Docker Compose to build a native Linux x64 executable (AOT) even from a non-Linux host:

```
cd Deployment/build

docker compose up --build
```

This will produce the native binary in the `out` directory, which can then be deployed to your Proxmox host.

### Deployment Script
A PowerShell script (`run.ps1`) is available for building, copying, and running the exporter on a remote host.

## Project Structure
- `Proxmox.Fqdn.Exporter/` - Main application source code
- `Proxmox.Fqdn.Exporter.Tests/` - Unit tests
- `Deployment/build/` - Docker and deployment scripts

## Extending
- Add new export targets by extending the `Export` configuration and implementing new adapters.
- The codebase uses dependency injection and is modular for easy extension.

## License
See [LICENSE](./LICENSE) for details.
