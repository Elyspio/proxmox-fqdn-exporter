$targetHost = "10.0.0.10"

docker buildx bake
docker run --rm -v "$PWD/out:/out" proxmox-fqdn-exporter:latest

scp out/* "root@${targetHost}:/data/promox-fqdn-exporter/"


ssh "root@$targetHost" "cd /data/promox-fqdn-exporter/ && chmod +x ./Proxmox.Fqdn.Exporter && ./Proxmox.Fqdn.Exporter"
