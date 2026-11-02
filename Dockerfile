# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o código da aplicação para o contexto de build
COPY . .

# Restaura apenas as dependências necessárias à aplicação de produção
RUN dotnet restore src/FCG_Notification/FCG_Notification.csproj

# Compila e publica o projeto web
RUN dotnet publish src/FCG_Notification/FCG_Notification.csproj -c Release -o /app

# Etapa 2: imagem final para rodar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Variáveis de ambiente para execução dentro do container
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# A partir do .NET 8 a imagem do ASP.NET escuta na 8080, e não mais na 80 — o
# container roda como usuário sem privilégio, que não pode abrir portas abaixo de 1024.
EXPOSE 8080

# Comando de inicialização
ENTRYPOINT ["dotnet", "FCG_Notification.dll"]