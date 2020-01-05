FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq
WORKDIR /app
EXPOSE 80

ENV ConnectionStrings:DefaultConnection="Server=tcp:birder.database.windows.net,1433;Initial Catalog=BirderDb;Persist Security Info=False;User ID=dipper;Password=ToryBoy1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src
COPY ["Birder/Birder.csproj", "Birder/"]
RUN dotnet restore "Birder/Birder.csproj"
COPY . .
WORKDIR "/src/Birder"
RUN dotnet build "Birder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Birder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Birder.dll"]


