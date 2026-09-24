# Deploy de uma API no Azure na Prática (DIO)

Projeto prático desenvolvido para o desafio **"Como Fazer o Deploy de uma API na Nuvem na Prática"** da trilha *Formação AZ-204 Certification* da [DIO](https://www.dio.me), ministrado por Henrique Eduardo Souza (Microsoft MVP).

## Objetivo

Reproduzir, em código, o cenário apresentado no vídeo: uma **Web API em .NET 8** é containerizada com **Docker**, publicada em um **Azure Container Registry (ACR)** e implantada em um **Azure Web App for Containers**, com uma esteira de **CI/CD** que builda, testa, gera uma nova imagem a cada alteração e atualiza a aplicação em produção **sem downtime** (usando tags de imagem versionadas em vez de `latest`, o que também permite rollback fácil).

## O que foi implementado

- **API** (`Program.cs`): Web API mínima em .NET 8, baseada no template padrão `WeatherForecast`, com o endpoint `GET /api/weatherforecast`. Igual ao vídeo, foi adicionado o campo **Kelvin** (`TemperatureK`) ao lado de Celsius e Fahrenheit. Endpoint `GET /` para health check simples.
- **Dockerfile**: build multi-stage (SDK para build/publish, runtime ASP.NET para a imagem final), expondo a porta `80` (padrão esperado pelo Azure Web App for Containers).
- **Pipeline de CI/CD** (`.github/workflows/ci-cd.yml`), usando GitHub Actions no lugar do Azure DevOps Pipelines usado no vídeo: build/restore com dotnet, login no ACR, `docker build` + `docker push` com uma tag única (`github.run_number`, nunca `latest`), e deploy automático no Azure Web App via `azure/webapps-deploy` — automatizando o passo que no vídeo era feito manualmente (trocar a tag da imagem no Deployment Center do Web App).
- **`.gitignore`** para projetos .NET/Docker.

## Arquitetura (mesma do desafio)

Dev -> git push -> Pipeline (build/test) -> Docker build e push -> Azure Container Registry (ACR) -> Azure Web App for Containers (producao)

## Rodando localmente

```bash
dotnet restore
dotnet run
# API disponível em http://localhost:5000 (ou porta exibida no console)
# Swagger em /swagger (ambiente Development)
```

Ou via Docker:

```bash
docker build -t dio-weather-api .
docker run -p 8080:80 dio-weather-api
curl http://localhost:8080/api/weatherforecast
```

## Publicando no Azure (passo a passo, como no vídeo)

Primeiro, crie um **Resource Group** (ex.: `rg-dio-api-demo`). Em seguida, crie um **Azure Container Registry (ACR)** — SKU *Basic* já atende; após criado, habilite o usuário admin (`Access keys` -> `Admin user: Enabled`), assim como no vídeo. Depois, crie um **Azure Web App for Containers** do tipo *Container* (Linux), apontando inicialmente para qualquer imagem pública (o ACR ainda estará vazio); em **Configuration -> General settings**, habilite `SCM Basic Auth Publishing Credentials` (necessário para o deploy via publish profile).

Configure os seguintes **secrets no GitHub** (Settings -> Secrets and variables -> Actions) para o workflow funcionar: `ACR_LOGIN_SERVER` (ex.: `acrapidemo001.azurecr.io`), `ACR_USERNAME` e `ACR_PASSWORD` (em ACR -> Access keys), `AZURE_WEBAPP_NAME` (nome do Web App) e `AZURE_WEBAPP_PUBLISH_PROFILE` (baixado em Web App -> Overview -> *Get publish profile*).

Depois disso, um `git push` na branch `main` já builda, empacota uma nova imagem com tag versionada, publica no ACR e atualiza o Web App automaticamente. Para comprovar o **zero downtime**, mantenha um `curl`/ping contínuo no endpoint (`/` ou `/api/weatherforecast`) enquanto um novo deploy acontece — a API deve continuar respondendo durante a troca de versão, exatamente como demonstrado no vídeo do desafio.

## Créditos

Desafio e roteiro original: **Henrique Eduardo Souza** (Microsoft MVP), trilha *Formação AZ-204 Certification* — [DIO](https://www.dio.me).
