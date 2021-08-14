#rate-limiting

Requirements:
- dotnet CLI

Generate a new self-signed certificate:

```shell
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p crypticpassword
dotnet dev-certs https --trust
```

### TODO:
- SSL certificate verification
- Docker support

Notes:
- Stress testing done with K6.