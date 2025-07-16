docker compose up --build

scp out/* "root@192.168.0.30:/data/promox-fqdn-exporter/"


ssh root@192.168.0.30 "cd /data/promox-fqdn-exporter/ && chmod +x ./Proxmox.Fqdn.Exporter && ./Proxmox.Fqdn.Exporter"