group "default" {
    targets = ["dotnet-app"]
}

target "dotnet-app" {
    context = "../../"
    dockerfile = "./Deployment/build/Dockerfile"
    tags = ["proxmox-fqdn-exporter:latest"]
}