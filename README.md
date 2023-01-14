# Simple Thermal Camera Website for MLX90640


## instructions for raspberry pi install

### install dotnet

wget https://dot.net/v1/dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 7.0 --runtime aspnetcore

### configure website service

sudo nano /lib/systemd/system/thermalCamera.service
```
[Unit]
Description=Thermal Camera Web App
After=nginx.service

[Service]
Type=simple
User=rob
WorkingDirectory=/home/rob/apps/thermalCamera
ExecStart=/home/rob/.dotnet/dotnet /home/rob/apps/thermalCamera/thermalCamera.dll
Restart=always

[Install]
WantedBy=multi-user.target
```

`sudo systemctl enable thermalCamera`

`sudo systemctl start thermalCamera`

### configure nginx

sudo apt-get install nginx

 sudo nano /etc/nginx/sites-available/default

 edit the location / section to read:
       
 ```
 location / {
    proxy_pass http://localhost:5000;
    proxy_http_version 1.1;
    proxy_set_header Connection keep-alive;
}
```
